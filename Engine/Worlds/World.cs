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
    public unsafe class World : FileObject
    {
        private DiscreteDynamicsWorld _bulletScene;
        public WorldSettings _settings;

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
            _bulletScene = new DiscreteDynamicsWorld(dispatcher, broadphase, solver, config);
            _bulletScene.Gravity = _settings.State.Gravity;
        }
        public void SetTimeDilation(float multiplier)
        {
            _settings.State.TimeDilation = multiplier;
        }
        public void RebaseOrigin(Vec3 newOrigin)
        {
            foreach (Actor a in _settings.State.SpawnedActors)
                a.OnOriginRebased(newOrigin);
        }
        public int ActorCount { get { return _settings.State.SpawnedActors.Count; } }

        public void SpawnActor(Actor actor, Vec3 worldPosition)
        {
            if (!_settings.State.SpawnedActors.Contains(actor))
                _settings.State.SpawnedActors.Add(actor);
            actor.Transform.AddTranslation(worldPosition);
            actor.OnSpawned(this);
        }
        public void DespawnActor(Actor actor)
        {
            if (_settings.State.SpawnedActors.Contains(actor))
                _settings.State.SpawnedActors.Remove(actor);
            actor.OnDespawned();
        }
        public void StepSimulation(float delta) { _bulletScene.StepSimulation(delta); }
        public Actor this[int index]
        {
            get { return _settings.State.SpawnedActors[index]; }
            set { _settings.State.SpawnedActors[index] = value; }
        }
        public virtual void EndPlay()
        {
            foreach (Map m in _settings._maps)
                m.EndPlay();
            _bulletScene = null;
        }
        public virtual void BeginPlay()
        {
            CreatePhysicsScene();
            foreach (Map m in _settings._maps)
                m.BeginPlay();
        }
        public static string GetTag() { return "WRLD"; }
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
