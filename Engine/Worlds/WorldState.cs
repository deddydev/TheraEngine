using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Files;
using TheraEngine.Rendering;
using TheraEngine.Actors;

namespace TheraEngine.Worlds
{
    public class WorldState : TFileObject
    {
        public List<Map> SpawnedMaps => _spawnedMaps;
        public EventList<IActor> SpawnedActors => _spawnedActors;

        public Scene3D Scene { get; set; } = null;

        [TSerialize("SpawnedMaps")]
        private List<Map> _spawnedMaps;
        [TSerialize("SpawnedActors")]
        private EventList<IActor> _spawnedActors;

        public Dictionary<Type, HashSet<int>> _actorMap;

        public WorldState()
        {
            _spawnedMaps = new List<Map>();
            _spawnedActors = new EventList<IActor>();
            _actorMap = new Dictionary<Type, HashSet<int>>();

            _spawnedActors.PostAdded += _spawnedActors_Added;
            _spawnedActors.PostInserted += _spawnedActors_Inserted;

            _spawnedActors.PreAddedRange += _spawnedActors_AddedRange;
            _spawnedActors.PostInsertedRange += _spawnedActors_InsertedRange;
        }

        #region Spawned Actors
        private void _spawnedActors_Added(IActor item)
        {
            _spawnedActors_Inserted(item, _spawnedActors.Count - 1);
        }
        private void _spawnedActors_Inserted(IActor item, int index)
        {
            Type t = item.GetType();
            if (!_actorMap.ContainsKey(t))
                _actorMap[t] = new HashSet<int>() { index };
            else
                _actorMap[t].Add(index);
        }
        private void _spawnedActors_AddedRange(IEnumerable<IActor> items)
        {
            _spawnedActors_InsertedRange(items, _spawnedActors.Count);
        }
        private void _spawnedActors_InsertedRange(IEnumerable<IActor> items, int index)
        {
            int i = 0;
            foreach (IActor item in items)
            {
                Type t = item.GetType();
                if (!_actorMap.ContainsKey(t))
                    _actorMap[t] = new HashSet<int>() { index + i };
                else
                    _actorMap[t].Add(index + i);
                ++i;
            }
        }
        #endregion

        [PostDeserialize]
        private void CreateActorMap()
        {
            _actorMap = new Dictionary<Type, HashSet<int>>();
            for (int i = 0; i < _spawnedActors.Count; ++i)
            {
                Type t = _spawnedActors[i].GetType();
                if (!_actorMap.ContainsKey(t))
                    _actorMap[t] = new HashSet<int>() { i };
                else
                    _actorMap[t].Add(i);
            }
        }

        public IEnumerable<T> GetSpawnedActorsOfType<T>() where T : IActor
        {
            Type t = typeof(T);
            if (_actorMap.ContainsKey(t))
                return _actorMap[t].Select(x => (T)_spawnedActors[x]);
            return null;
        }
        public IEnumerable<IActor> GetSpawnedActorsOfType(Type actorType)
        {
            if (_actorMap.ContainsKey(actorType))
                return _actorMap[actorType].Select(x => _spawnedActors[x]);
            return null;
        }
    }
}
