using System.Collections.Generic;
using System.Collections;
using BulletSharp;
using System;
using System.Threading.Tasks;
using System.Linq;
using CustomEngine.Rendering.Models.Materials;
using System.Runtime.InteropServices;
using CustomEngine.Files;
using System.Xml;
using CustomEngine.Rendering;
using System.IO;

namespace CustomEngine.Worlds
{
    public interface IWorld
    {
        WorldSettings Settings { get; set; }
    }
    [ObjectHeader()]
    [FileHeader()]
    public unsafe class World : FileObject, IEnumerable<IActor>
    {
        static World()
        {
            PersistentManifold.ContactProcessed += PersistentManifold_ContactProcessed;
            PersistentManifold.ContactDestroyed += PersistentManifold_ContactDestroyed;
        }

        [Serialize("PhysicsWorld")]
        internal DiscreteDynamicsWorld _physicsScene;
        public WorldSettings _settings;

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
            driver0.BeginOverlap(driver1);
            driver1.BeginOverlap(driver0);
            cp.UserPersistentData = new PhysicsDriverPair(driver0, driver1);
        }
        private static void PersistentManifold_ContactDestroyed(object userPersistantData)
        {
            PhysicsDriverPair drivers = (PhysicsDriverPair)userPersistantData;
            drivers._driver0.EndOverlap(drivers._driver1);
            drivers._driver1.EndOverlap(drivers._driver0);
        }
        private void CreatePhysicsScene()
        {
            BroadphaseInterface broadphase = new DbvtBroadphase();
            DefaultCollisionConfiguration config = new DefaultCollisionConfiguration();
            CollisionDispatcher dispatcher = new CollisionDispatcher(config);
            SequentialImpulseConstraintSolver solver = new SequentialImpulseConstraintSolver();
            _physicsScene = new DiscreteDynamicsWorld(dispatcher, broadphase, solver, config)
            {
                Gravity = _settings.State.Gravity
            };
            _physicsScene.PairCache.SetOverlapFilterCallback(new CustomOvelapFilter());
            _settings.State.GravityChanged += OnGravityChanged;
            
        }
        private void OnGravityChanged(Vec3 oldGravity)
        {
            _physicsScene.Gravity = _settings.State.Gravity;
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
        /// <summary>
        /// Moves the origin to preserve float precision when traveling large distances from the origin.
        /// </summary>
        public void RebaseOrigin(Vec3 newOrigin)
        {
            foreach (IActor a in State.SpawnedActors)
                a.RebaseOrigin(newOrigin);
        }

        public int ActorCount => _settings.State.SpawnedActors.Count;
        public WorldState State => _settings.State;

        public void SpawnActor(IActor actor, Matrix4 transform)
        {
            if (!_settings.State.SpawnedActors.Contains(actor))
                _settings.State.SpawnedActors.Add(actor);
            //actor.Transform.TranslateAbsolute(worldPosition);
            actor.OnSpawned(this);
        }
        public void DespawnActor(IActor actor)
        {
            if (_settings.State.SpawnedActors.Contains(actor))
                _settings.State.SpawnedActors.Remove(actor);
            actor.OnDespawned();
        }
        public void StepSimulation(float delta)
        {
            _physicsScene.StepSimulation(delta);

            for (int i = 0; i < _physicsScene.Dispatcher.NumManifolds; ++i)
            {
                PersistentManifold m = _physicsScene.Dispatcher.GetManifoldByIndexInternal(i);
                
            }
        }
        public IActor this[int index]
        {
            get => _settings.State.SpawnedActors[index];
            set => _settings.State.SpawnedActors[index] = value;
        }
        public virtual void EndPlay()
        {
            foreach (Map m in _settings._defaultMaps)
                m.EndPlay();
            _physicsScene = null;
        }
        public virtual void BeginPlay()
        {
            CreatePhysicsScene();
            foreach (Map m in _settings._defaultMaps)
                m.BeginPlay();
            foreach (PhysicsDriver d in Engine._queuedCollisions)
                d.AddToWorld();
            Engine._queuedCollisions.Clear();
        }
        protected override int OnCalculateSize(StringTable table)
        {
            return 0;
        }
        public override void Write(VoidPtr address, StringTable table)
        {
            Header* h = (Header*)address;
        }
        public override void Read(VoidPtr address, VoidPtr strings)
        {

        }
        public override void Write(XmlWriter writer)
        {

        }
        public override void Read(XMLReader reader)
        {

        }

        public IEnumerator<IActor> GetEnumerator() => State.SpawnedActors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => State.SpawnedActors.GetEnumerator();

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct Header
        {
            public const int Size = 4;

            public buint _mapCount;
            public WorldSettings.Header _settings;
            public PhysicsWorldState _physicsState;
            
            public FileRefHeader* MapOffsets { get { return (FileRefHeader*)Address; } }
            public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct PhysicsWorldState
        {
            public const int Size = 4;

            public buint _collisionObjectCount;
            
            public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        }
    }
}
