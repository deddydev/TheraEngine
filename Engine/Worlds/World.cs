using System.Collections.Generic;
using System.Collections;
using BulletSharp;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace CustomEngine.Worlds
{
    public class World : ObjectBase, IRenderable
    {
        private DiscreteDynamicsWorld _bulletScene;
        public WorldSettings _settings;

        public WorldSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        public World(string filePath)
        {
            _settings = WorldSettings.FromXML(filePath);
            CreatePhysicsScene();
        }
        public World(WorldSettings settings)
        {
            _settings = settings;
            CreatePhysicsScene();
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
        public void SetTimeMultiplier(float mult)
        {
            _settings.State.TimeMultiplier = mult;
        }
        public void RebaseOrigin(Vec3 newOrigin)
        {
            foreach (Actor a in _settings.State.SpawnedActors)
                a.OnOriginRebased(newOrigin);
        }
        public int ActorCount { get { return _settings.State.SpawnedActors.Count; } }

        public bool IsLoaded { get { return _settings.IsLoaded; } }
        public bool IsLoading { get { return _settings.IsLoading; } }

        public void SpawnActor(Actor actor)
        {
            if (!_settings.State.SpawnedActors.Contains(actor))
                _settings.State.SpawnedActors.Add(actor);
            actor.OnSpawned(this);
        }
        public void DespawnActor(Actor actor)
        {
            if (_settings.State.SpawnedActors.Contains(actor))
                _settings.State.SpawnedActors.Remove(actor);
            actor.OnDespawned();
        }
        public void StepSimulation(float delta) { _bulletScene.StepSimulation(delta); }
        public void Render(float delta)
        {
            foreach (Actor actor in _settings.State.SpawnedActors)
                actor.Render(delta);
        }
        public Actor this[int index]
        {
            get { return _settings.State.SpawnedActors[index]; }
            set { _settings.State.SpawnedActors[index] = value; }
        }

        public void Load()
        {
            
        }
        public void Unload()
        {

        }
    }
}
