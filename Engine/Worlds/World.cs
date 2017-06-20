using System.Collections.Generic;
using System.Collections;
using BulletSharp;
using System;
using TheraEngine.Files;
using TheraEngine.Rendering;
using TheraEngine.Input;
using System.ComponentModel;
using TheraEngine.GameModes;

namespace TheraEngine.Worlds
{
    public interface IWorld
    {
        WorldSettings Settings { get; set; }
    }
    [FileClass("WORLD", "World")]
    public unsafe class World : FileObject, IEnumerable<IActor>, IDisposable
    {
        static World()
        {
            PersistentManifold.ContactProcessed += PersistentManifold_ContactProcessed;
            PersistentManifold.ContactDestroyed += PersistentManifold_ContactDestroyed;
        }
        
        public World()
        {
            if (_settings == null)
                _settings = new WorldSettings();
        }
        public World(WorldSettings settings)
        {
            _settings = settings;
        }

        internal DiscreteDynamicsWorld _physicsScene;
        [Serialize("Settings")]
        protected WorldSettings _settings;

        public DiscreteDynamicsWorld PhysicsScene => _physicsScene;
        public WorldSettings Settings
        {
            get => _settings;
            set => _settings = value;
        }
        private class PhysicsDriverPair
        {
            public PhysicsDriverPair(PhysicsDriver driver0, PhysicsDriver driver1)
            {
                _driver0 = driver0;
                _driver1 = driver1;
            }
            public PhysicsDriver _driver0, _driver1;
        }
        private static void PersistentManifold_ContactProcessed(ManifoldPoint cp, CollisionObject body0, CollisionObject body1)
        {
            PhysicsDriver driver0 = (PhysicsDriver)body0.UserObject;
            PhysicsDriver driver1 = (PhysicsDriver)body1.UserObject;
            cp.UserPersistentData = new PhysicsDriverPair(driver0, driver1);
            driver0.ContactStarted(driver1, cp);
        }
        private static void PersistentManifold_ContactDestroyed(object userPersistantData)
        {
            PhysicsDriverPair drivers = (PhysicsDriverPair)userPersistantData;
            drivers._driver0.ContactEnded(drivers._driver1);
            drivers._driver1.ContactEnded(drivers._driver0);
        }
        private void OnGravityChanged(Vec3 oldGravity)
        {
            _physicsScene.Gravity = _settings.Gravity;
        }
        //private class CustomOvelapFilter : OverlapFilterCallback
        //{
        //    public override bool NeedBroadphaseCollision(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
        //    {
        //        if (proxy0 == null || 
        //            proxy1 == null)
        //            return false;

        //        bool collides = 
        //            (proxy0.CollisionFilterGroup & proxy1.CollisionFilterMask) != 0 &&
        //            (proxy1.CollisionFilterGroup & proxy0.CollisionFilterMask) != 0;

        //        return collides;
        //    }
        //}

        public BaseGameMode GetGameMode()
            => Settings?.GameMode;
        public T GetGameMode<T>() where T : class, IGameMode
            => Settings?.GameMode as T;

        public int ActorCount => _settings.State.SpawnedActors.Count;
        public WorldState State => _settings.State;
        
        public void SpawnActor(IActor actor)
        {
            if (!_settings.State.SpawnedActors.Contains(actor))
                _settings.State.SpawnedActors.Add(actor);
            actor.OnSpawned(this);
        }
        public void DespawnActor(IActor actor)
        {
            if (_settings.State.SpawnedActors.Contains(actor))
                _settings.State.SpawnedActors.Remove(actor);
            actor.OnDespawned();
        }
        
        public void StepSimulation(float delta)
            => _physicsScene?.StepSimulation(delta, 7, (float)(Engine.TargetRenderPeriod * Engine.TimeDilation));
        
        public IActor this[int index]
        {
            get => _settings.State.SpawnedActors[index];
            set => _settings.State.SpawnedActors[index] = value;
        }
        public virtual void EndPlay()
        {
            _settings.GameMode?.EndGameplay();
            foreach (Map m in _settings.Maps)
                m.EndPlay();
            Dispose();
        }
        public virtual void BeginPlay()
        {
            CreatePhysicsScene();
            _settings.AmbientSound?.Play(_settings.AmbientParams);
            foreach (Map m in _settings.Maps)
                m.BeginPlay();
            _settings.GameMode?.BeginGameplay();
            //foreach (PhysicsDriver d in Engine._queuedCollisions)
            //    d.AddToWorld();
            //Engine._queuedCollisions.Clear();
        }
        /// <summary>
        /// Moves the origin to preserve float precision when traveling large distances from the origin.
        /// </summary>
        public void RebaseOrigin(Vec3 newOrigin)
        {
            foreach (IActor a in State.SpawnedActors)
                a.RebaseOrigin(newOrigin);
        }
        BroadphaseInterface _physicsBroadphase;
        CollisionConfiguration _collisionConfig;
        CollisionDispatcher _collisionDispatcher;
        ConstraintSolver _constraintSolver;
        WorldDebugDrawer _physicsDebugDrawer;
        private void CreatePhysicsScene()
        {
            _physicsBroadphase = new DbvtBroadphase();
            _collisionConfig = new DefaultCollisionConfiguration();
            _collisionDispatcher = new CollisionDispatcher(_collisionConfig);
            _constraintSolver = new SequentialImpulseConstraintSolver();
            _physicsScene = new DiscreteDynamicsWorld(_collisionDispatcher, _physicsBroadphase, _constraintSolver, _collisionConfig)
            {
                Gravity = _settings.Gravity
            };
            _physicsScene.DebugDrawer = _physicsDebugDrawer = new WorldDebugDrawer()
            {
                DebugMode = DebugDrawModes.DrawAabb
            };
            _physicsScene.DispatchInfo.UseContinuous = true;
            //_physicsScene.PairCache.SetOverlapFilterCallback(new CustomOvelapFilter());
            _settings.GravityChanged += OnGravityChanged;

        }
        public void Dispose()
        {
            if (_physicsScene != null)
            {
                //Remove and dispose of constraints
                int i;
                for (i = _physicsScene.NumConstraints - 1; i >= 0; --i)
                {
                    TypedConstraint constraint = _physicsScene.GetConstraint(i);
                    _physicsScene.RemoveConstraint(constraint);
                    constraint.Dispose();
                }

                //Remove the rigidbodies from the dynamics world and delete them
                for (i = _physicsScene.NumCollisionObjects - 1; i >= 0; --i)
                {
                    CollisionObject obj = _physicsScene.CollisionObjectArray[i];
                    if (obj is RigidBody body && body.MotionState != null)
                        body.MotionState.Dispose();
                    _physicsScene.RemoveCollisionObject(obj);
                    obj.Dispose();
                }
                
                _physicsScene.Dispose();
                _physicsScene = null;

                _constraintSolver.Dispose();
                _constraintSolver = null;

                _physicsBroadphase.Dispose();
                _physicsBroadphase = null;

                _collisionDispatcher.Dispose();
                _collisionDispatcher = null;

                _collisionConfig.Dispose();
                _collisionConfig = null;

                _physicsDebugDrawer.Dispose();
                _physicsDebugDrawer = null;
            }
        }
        public IEnumerator<IActor> GetEnumerator() => State.SpawnedActors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => State.SpawnedActors.GetEnumerator();

        //public event Action<LocalPlayerController> LocalPlayerAdded;
        //internal protected virtual void OnLocalPlayerAdded(LocalPlayerController controller)
        //{
        //    LocalPlayerAdded?.Invoke(controller);
        //}
    }
}
