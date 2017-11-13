using System.Collections.Generic;
using System.Collections;
using BulletSharp;
using System;
using TheraEngine.Files;
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
        public World()
        {
            _settings = new WorldSettings();
            _state = new WorldState();
        }
        public World(WorldSettings settings)
        {
            _settings = settings;
            _state = new WorldState();
        }
        public World(WorldSettings settings, WorldState state)
        {
            _settings = settings;
            _state = state;
        }

        internal DiscreteDynamicsWorld _physicsScene;
        [TSerialize("Settings")]
        protected SingleFileRef<WorldSettings> _settings;
        [TSerialize("State")]
        protected SingleFileRef<WorldState> _state;

        public DiscreteDynamicsWorld PhysicsScene => _physicsScene;
        public SingleFileRef<WorldSettings> Settings
        {
            get => _settings;
            set => _settings = value;
        }
        public SingleFileRef<WorldState> State
        {
            get => _state;
            set => _state = value;
        }

        private void OnGravityChanged(Vec3 oldGravity)
        {
            _physicsScene.Gravity = _settings.File.Gravity;
        }
        private class CustomOvelapFilter : OverlapFilterCallback
        {
            public override bool NeedBroadphaseCollision(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
            {
                if (proxy0 == null || proxy1 == null)
                    return false;

                bool collides =
                    (proxy0.CollisionFilterGroup & proxy1.CollisionFilterMask) != 0 &&
                    (proxy1.CollisionFilterGroup & proxy0.CollisionFilterMask) != 0;

                return collides;
            }
        }

        public BaseGameMode GetGameMode()
            => Settings?.File?.GameModeOverride?.File;
        public T GetGameMode<T>() where T : class, IGameMode
            => Settings?.File?.GameModeOverride?.File as T;

        [Browsable(false)]
        public int SpawnedActorCount => State.File.SpawnedActors.Count;

        /// <summary>
        /// Adds an actor to the scene.
        /// </summary>
        public void SpawnActor(IActor actor)
        {
            if (!State.File.SpawnedActors.Contains(actor))
                State.File.SpawnedActors.Add(actor);

            actor.Spawned(this);
            Engine.PrintLine("Spawned " + actor.Name);
        }
        /// <summary>
        /// Adds an actor to the scene.
        /// </summary>
        public void SpawnActor(IActor actor, Vec3 position)
        {
            if (!State.File.SpawnedActors.Contains(actor))
                State.File.SpawnedActors.Add(actor);
            actor.Spawned(this);
            actor.RebaseOrigin(-position);
        }
        /// <summary>
        /// Removes an actor from the scene.
        /// </summary>
        public void DespawnActor(IActor actor)
        {
            if (State.File.SpawnedActors.Contains(actor))
                State.File.SpawnedActors.Remove(actor);
            actor.Despawned();
            Engine.PrintLine("Despawned " + actor.Name);
        }
        
        internal void StepSimulation(float delta)
            => _physicsScene?.StepSimulation(delta, 7, (float)(Engine.RenderPeriod * Engine.TimeDilation));
        
        public IActor this[int index]
        {
            get => State.File.SpawnedActors[index];
            set => State.File.SpawnedActors[index] = value;
        }
        /// <summary>
        /// Moves the origin to preserve float precision when traveling large distances from the origin.
        /// Provide any world point and that point will become the new (0,0,0).
        /// </summary>
        public void RebaseOrigin(Vec3 newOrigin)
        {
            foreach (IActor a in State.File.SpawnedActors)
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
                Gravity = _settings.File.Gravity
            };
            _physicsScene.DebugDrawer = _physicsDebugDrawer = new WorldDebugDrawer()
            {
                DebugMode = 
                DebugDrawModes.DrawNormals | 
                //DebugDrawModes.DrawAabb | 
                DebugDrawModes.DrawConstraints | 
                DebugDrawModes.DrawConstraintLimits | 
                DebugDrawModes.DrawContactPoints// | 
                //DebugDrawModes.DrawWireframe |
                //DebugDrawModes.FastWireframe
            };
            //_physicsScene.DispatchInfo.UseContinuous = true;
            //_physicsScene.DispatchInfo.AllowedCcdPenetration = 0.1f;
            _physicsScene.PairCache.SetOverlapFilterCallback(new CustomOvelapFilter());
            _settings.File.GravityChanged += OnGravityChanged;

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

                _constraintSolver?.Dispose();
                _constraintSolver = null;

                _physicsBroadphase?.Dispose();
                _physicsBroadphase = null;

                _collisionDispatcher?.Dispose();
                _collisionDispatcher = null;

                _collisionConfig?.Dispose();
                _collisionConfig = null;

                _physicsDebugDrawer?.Dispose();
                _physicsDebugDrawer = null;
            }
        }
        public IEnumerator<IActor> GetEnumerator() => State.File.SpawnedActors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => State.File.SpawnedActors.GetEnumerator();

        protected override void OnUnload() => Dispose();
        public virtual void EndPlay()
        {
            foreach (Map m in _settings.File.Maps)
                m.EndPlay();
        }
        public virtual void BeginPlay()
        {
            foreach (Map m in _settings.File.Maps)
                m.BeginPlay();
        }
        internal protected virtual void Initialize()
        {
            CreatePhysicsScene();

            Engine.TimeDilation = _settings.File.TimeDilation;

            foreach (Map m in _settings.File.Maps)
            {
                if (m.Settings.VisibleByDefault)
                {
                    m.Initialize();
                }
            }
        }

        //public event Action<LocalPlayerController> LocalPlayerAdded;
        //internal protected virtual void OnLocalPlayerAdded(LocalPlayerController controller)
        //{
        //    LocalPlayerAdded?.Invoke(controller);
        //}
    }

    //internal class CustomOvelapFilter : OverlapFilterCallback
    //{
    //    public override bool NeedBroadphaseCollision(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
    //    {
    //        return 
    //            ((short)proxy0.CollisionFilterGroup & (short)proxy1.CollisionFilterMask) != 0 &&
    //            ((short)proxy1.CollisionFilterGroup & (short)proxy0.CollisionFilterMask) != 0;
    //    }
    //}
}
