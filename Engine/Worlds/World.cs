using System.Collections.Generic;
using System.Collections;
using BulletSharp;
using System;
using System.Threading.Tasks;
using System.Linq;
using CustomEngine.Rendering.Models.Materials;
using System.Runtime.InteropServices;
using CustomEngine.Files;

namespace CustomEngine.Worlds
{
    public interface IWorld
    {
        WorldSettings Settings { get; set; }
    }
    public unsafe class World : FileObject, IEnumerable<Actor>
    {
        internal DiscreteDynamicsWorld _physicsScene;
        public WorldSettings _settings;

        public DiscreteDynamicsWorld PhysicsScene
        {
            get { return _physicsScene; }
        }
        public WorldSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }
        private void CreatePhysicsScene()
        {
            BroadphaseInterface broadphase = new DbvtBroadphase();
            DefaultCollisionConfiguration config = new DefaultCollisionConfiguration();
            CollisionDispatcher dispatcher = new CollisionDispatcher(config);
            SequentialImpulseConstraintSolver solver = new SequentialImpulseConstraintSolver();
            _physicsScene = new DiscreteDynamicsWorld(dispatcher, broadphase, solver, config);
            _physicsScene.Gravity = _settings.State.Gravity;
        }
        /// <summary>
        /// Moves the origin to preserve float precision when traveling large distances from the origin.
        /// </summary>
        public void RebaseOrigin(Vec3 newOrigin)
        {
            foreach (Actor a in State.SpawnedActors)
                a.RebaseOrigin(newOrigin);
        }
        public int ActorCount { get { return _settings.State.SpawnedActors.Count; } }

        public WorldState State { get { return _settings.State; } }

        public void SpawnActor(Actor actor, Matrix4 transform)
        {
            if (!_settings.State.SpawnedActors.Contains(actor))
                _settings.State.SpawnedActors.Add(actor);
            //actor.Transform.TranslateAbsolute(worldPosition);
            actor.OnSpawned(this);
        }
        public void DespawnActor(Actor actor)
        {
            if (_settings.State.SpawnedActors.Contains(actor))
                _settings.State.SpawnedActors.Remove(actor);
            actor.OnDespawned();
        }
        public void StepSimulation(float delta) { _physicsScene.StepSimulation(delta); }
        public Actor this[int index]
        {
            get { return _settings.State.SpawnedActors[index]; }
            set { _settings.State.SpawnedActors[index] = value; }
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
        }
        public IEnumerator<Actor> GetEnumerator() { return State.SpawnedActors.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return State.SpawnedActors.GetEnumerator(); }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct WorldHeader
    {
        public VoidPtr Address { get { fixed(void* ptr = &this) return ptr; } }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct WorldSettingsHeader
    {
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}
