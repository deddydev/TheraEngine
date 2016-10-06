using System.Collections.Generic;
using System.Collections;
using BulletSharp;
using System;
using System.Threading.Tasks;

namespace CustomEngine.Worlds
{
    public class World : ObjectBase, IRenderable, ILoadable
    {
        protected List<Map> _spawnedMaps = new List<Map>();
        protected List<Actor> _spawnedActors;

        protected List<Map> _allMaps = new List<Map>();

        private DiscreteDynamicsWorld _bulletScene;
        public WorldSettings _settings;
        private string _name;
        private bool _visible;

        public int ActorCount { get { return _spawnedActors.Count; } }

        public World(string filePath)
        {
            _filePath = filePath;
            _isLoaded = false;

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
        public void Render()
        {
            foreach (Actor actor in _spawnedActors)
                if (actor.IsSpawned)
                    actor.Render();
        }
        public void SerializeState(string path)
        {
            
        }
        public void ExportMapFile(string path)
        {

        }

        #region ILoadable Interface
        private bool _isLoaded, _isLoading;
        private string _filePath;
        public async Task Unload()
        {
            if (!_isLoaded)
                return;

            _isLoaded = false;
        }
        public async Task Load()
        {
            if (_isLoaded)
                return;
            _isLoading = true;
            FileMap map = FileMap.FromFile(_filePath);
            
            foreach (Map m in _allMaps)
                if (m.VisibleByDefault)
                    await Task.Run(m.Load);
            _isLoading = false;
            _isLoaded = true;
        }
        public string FilePath { get { return _filePath; } }
        public bool IsLoading { get { return _isLoading; } }
        public bool IsLoaded { get { return _isLoaded; } }
        #endregion

        public Actor this[int index]
        {
            get { return _spawnedActors[index]; }
            set { _spawnedActors[index] = value; }
        }
    }
}
