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
    [FileExt("wsta")]
    [FileDef("World State")]
    public class WorldState : TFileObject
    {
        public List<Map> SpawnedMaps => _spawnedMaps;
        public EventList<IActor> SpawnedActors => _spawnedActors;

        public BaseScene Scene { get; set; } = null;

        [TSerialize("SpawnedMaps")]
        private List<Map> _spawnedMaps;
        [TSerialize("SpawnedActors")]
        private EventList<IActor> _spawnedActors;

        public Dictionary<Type, HashSet<int>> _actorTypeMap;
        public Dictionary<string, IActor> _actorNameMap;

        public WorldState()
        {
            _spawnedMaps = new List<Map>();
            _spawnedActors = new EventList<IActor>(false, false);
            _actorTypeMap = new Dictionary<Type, HashSet<int>>();
            _actorNameMap = new Dictionary<string, IActor>();

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
            if (!_actorTypeMap.ContainsKey(t))
                _actorTypeMap[t] = new HashSet<int>() { index };
            else
                _actorTypeMap[t].Add(index);
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
                if (!_actorTypeMap.ContainsKey(t))
                    _actorTypeMap[t] = new HashSet<int>() { index + i };
                else
                    _actorTypeMap[t].Add(index + i);
                ++i;
            }
        }
        #endregion

        [PostDeserialize]
        internal void CreateActorMap()
        {
            _actorTypeMap = new Dictionary<Type, HashSet<int>>();
            _actorNameMap = new Dictionary<string, IActor>();

            for (int i = 0; i < _spawnedActors.Count; ++i)
            {
                IActor actor = _spawnedActors[i];
                Type t = actor.GetType();
                if (!_actorTypeMap.ContainsKey(t))
                    _actorTypeMap[t] = new HashSet<int>() { i };
                else
                    _actorTypeMap[t].Add(i);
                _actorNameMap.Add(actor.Name, actor);
            }
        }

        public IEnumerable<T> GetSpawnedActorsOfType<T>() where T : IActor
        {
            Type t = typeof(T);
            if (_actorTypeMap.ContainsKey(t))
                return _actorTypeMap[t].Select(x => (T)_spawnedActors[x]);
            return null;
        }
        public IEnumerable<IActor> GetSpawnedActorsOfType(Type actorType)
        {
            if (_actorTypeMap.ContainsKey(actorType))
                return _actorTypeMap[actorType].Select(x => _spawnedActors[x]);
            return null;
        }
    }
}
