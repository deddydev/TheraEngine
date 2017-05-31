using System.Collections.Generic;
using System.Collections;
using BulletSharp;
using System;
using CustomEngine.Files;
using CustomEngine.Rendering;
using CustomEngine.Input;
using System.ComponentModel;
using CustomEngine.GameModes;

namespace CustomEngine.Worlds
{
    public interface IWorld
    {
        WorldSettings Settings { get; set; }
    }
    [FileClass("WORLD", "World")]
    public unsafe class World : FileObject, IEnumerable<IActor>
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
        private void CreatePhysicsScene()
        {
            BroadphaseInterface broadphase = new DbvtBroadphase();
            DefaultCollisionConfiguration config = new DefaultCollisionConfiguration();
            CollisionDispatcher dispatcher = new CollisionDispatcher(config);
            SequentialImpulseConstraintSolver solver = new SequentialImpulseConstraintSolver();
            _physicsScene = new DiscreteDynamicsWorld(dispatcher, broadphase, solver, config)
            {
                Gravity = _settings.Gravity
            };
            _physicsScene.PairCache.SetOverlapFilterCallback(new CustomOvelapFilter());
            _settings.GravityChanged += OnGravityChanged;
            
        }
        private void OnGravityChanged(Vec3 oldGravity)
        {
            _physicsScene.Gravity = _settings.Gravity;
        }
        private class CustomOvelapFilter : OverlapFilterCallback
        {
            public override bool NeedBroadphaseCollision(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
            {
                if (proxy0 == null || 
                    proxy1 == null)
                    return false;

                bool collides = 
                    (proxy0.CollisionFilterGroup & proxy1.CollisionFilterMask) != 0 &&
                    (proxy1.CollisionFilterGroup & proxy0.CollisionFilterMask) != 0;
                
                return collides;
            }
        }

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
            => _physicsScene?.StepSimulation(delta);
        
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
            _physicsScene = null;
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

        public IEnumerator<IActor> GetEnumerator() => State.SpawnedActors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => State.SpawnedActors.GetEnumerator();
    }
}
