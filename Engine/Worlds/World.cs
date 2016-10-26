﻿using System.Collections.Generic;
using System.Collections;
using BulletSharp;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace CustomEngine.Worlds
{
    public class World : FileObject, IRenderable
    {
        protected List<Map> _spawnedMaps = new List<Map>();
        protected List<Actor> _spawnedActors = new List<Actor>();

        private DiscreteDynamicsWorld _bulletScene;
        public WorldSettings _settings;
        public WorldState _state;
        private string _name;
        private bool _visible;

        public int ActorCount { get { return _spawnedActors.Count; } }

        public World(string filePath)
        {
            _filePath = filePath;
            _isLoaded = false;
            CreatePhysicsScene();
        }

        public World(WorldSettings settings)
        {
            _filePath = null;
            _isLoaded = true;
            CreatePhysicsScene();

            _settings = settings;
        }

        private void CreatePhysicsScene()
        {
            BroadphaseInterface broadphase = new DbvtBroadphase();
            DefaultCollisionConfiguration config = new DefaultCollisionConfiguration();
            CollisionDispatcher dispatcher = new CollisionDispatcher(config);
            SequentialImpulseConstraintSolver solver = new SequentialImpulseConstraintSolver();
            _bulletScene = new DiscreteDynamicsWorld(dispatcher, broadphase, solver, config);
        }

        public void RebaseOrigin(Vec3 newOrigin)
        {
            foreach (Actor a in _spawnedActors)
                a.OnOriginRebased(newOrigin);
        }
        public void SpawnActor(Actor actor)
        {
            if (!_spawnedActors.Contains(actor))
                _spawnedActors.Add(actor);
            actor.OnSpawned(this);
        }
        public void DespawnActor(Actor actor)
        {
            if (_spawnedActors.Contains(actor))
                _spawnedActors.Remove(actor);
            actor.OnDespawned();
        }
        public void StepSimulation(float delta) { _bulletScene.StepSimulation(delta); }
        public void Render(float delta)
        {
            foreach (Actor actor in _spawnedActors)
                actor.Render(delta);
        }
        public async Task SaveState(string path)
        {
            await Task.Delay(100);
        }
        public async Task LoadState(string path)
        {
            await Task.Delay(100);
        }
        #region ILoadable Interface
        public override void Unload()
        {
            if (!_isLoaded)
                return;

            _isLoaded = false;
        }
        public override void Load()
        {
            if (_isLoaded)
                return;
            _isLoading = true;
            if (!string.IsNullOrEmpty(_filePath))
            {
                FileMap map = FileMap.FromFile(_filePath);
                foreach (Map m in _settings._maps)
                    if (m.VisibleByDefault)
                        m.Load();
            }
            _isLoading = false;
            _isLoaded = true;
        }
        #endregion

        public Actor this[int index]
        {
            get { return _spawnedActors[index]; }
            set { _spawnedActors[index] = value; }
        }
    }
}
