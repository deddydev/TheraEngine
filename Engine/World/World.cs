using System.Collections.Generic;
using System.Collections;
using BulletSharp;
using System;

namespace CustomEngine.Worlds
{
    public class World : IRenderable, ILoadable, IEnumerable<Actor>
    {
        protected List<Map> _spawnedMaps = new List<Map>();
        protected List<Actor> _spawnedActors;

        protected List<Map> _allMaps;

        private DiscreteDynamicsWorld _bulletScene;
        public WorldDefaults _defaults;
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
            _bulletScene.Gravity = _settings.Gravity;
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
        public delegate void PhysicsTick(float subDeltaTime);
        public event PhysicsTick PrePhysicsTick;
        public event PhysicsTick PostPhysicsTick;
        public void Update()
        {
            float delta = Engine.UpdateDelta / Engine.PhysicsSubsteps;
            for (int i = 0; i < Engine.PhysicsSubsteps; i++)
            {
                PrePhysicsTick?.Invoke(delta);
                _bulletScene.StepSimulation(delta);
                PostPhysicsTick?.Invoke(delta);
            }
        }
        public void Render()
        {
            foreach (Actor actor in _spawnedActors)
                if (actor.IsSpawned)
                    actor.Render();
        }
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value)
                    return;

                if (_visible = value)
                {
//                     foreach (Map m in _allMaps)
//                         m.Visible = m.
                }
                else
                {
                    foreach (Actor a in _spawnedActors)
                        DespawnActor(a);
                }
            }
        }

        public void SerializeState(string path)
        {
            
        }
        public void ExportMapFile(string path)
        {

        }

        #region ILoadable Interface
        private bool _isLoaded;
        private string _filePath;
        public void Unload()
        {
            if (!_isLoaded)
                return;

            _isLoaded = false;
        }
        public void Load()
        {
            if (_isLoaded)
                return;
            foreach (Map m in _allMaps)
                if (m.VisibleByDefault)
                    m.Load();
            _isLoaded = true;
        }
        public string FilePath { get { return _filePath; } }
        public bool IsLoaded { get { return _isLoaded; } }
        #endregion

        public Actor this[int index]
        {
            get { return _spawnedActors[index]; }
            set { _spawnedActors[index] = value; }
        }
        public IEnumerator<Actor> GetEnumerator() { return ((IEnumerable<Actor>)_spawnedActors).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Actor>)_spawnedActors).GetEnumerator(); }
    }
}
