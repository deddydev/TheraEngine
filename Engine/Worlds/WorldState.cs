using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Core.Files;
using TheraEngine.Rendering;
using TheraEngine.Actors;
using TheraEngine.GameModes;

namespace TheraEngine.Worlds
{
    [TFileExt("wsta")]
    [TFileDef("World State")]
    public class WorldState : TFileObject
    {
        [TSerialize]
        public List<Map> SpawnedMaps { get; }
        [TSerialize]
        public EventList<BaseActor> SpawnedActors { get; }

        public BaseScene Scene { get; internal set; } = null;
        public BaseGameMode GameMode { get; internal set; } = null;

        public Dictionary<Type, HashSet<int>> _actorTypeMap;
        public Dictionary<string, BaseActor> _actorNameMap;

        public WorldState()
        {
            SpawnedMaps = new List<Map>();
            SpawnedActors = new EventList<BaseActor>(false, false);
            _actorTypeMap = new Dictionary<Type, HashSet<int>>();
            _actorNameMap = new Dictionary<string, BaseActor>();

            SpawnedActors.PostAdded += _spawnedActors_Added;
            SpawnedActors.PostInserted += _spawnedActors_Inserted;

            SpawnedActors.PreAddedRange += _spawnedActors_AddedRange;
            SpawnedActors.PostInsertedRange += _spawnedActors_InsertedRange;
        }

        #region Spawned Actors
        private void _spawnedActors_Added(BaseActor item)
        {
            _spawnedActors_Inserted(item, SpawnedActors.Count - 1);
        }
        private void _spawnedActors_Inserted(BaseActor item, int index)
        {
            Type t = item.GetType();
            if (!_actorTypeMap.ContainsKey(t))
                _actorTypeMap[t] = new HashSet<int>() { index };
            else
                _actorTypeMap[t].Add(index);
        }
        private bool _spawnedActors_AddedRange(IEnumerable<BaseActor> items)
        {
            _spawnedActors_InsertedRange(items, SpawnedActors.Count);
            return true;
        }
        private void _spawnedActors_InsertedRange(IEnumerable<BaseActor> items, int index)
        {
            int i = 0;
            foreach (BaseActor item in items)
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

        [TPostDeserialize]
        internal void CreateActorMap()
        {
            _actorTypeMap = new Dictionary<Type, HashSet<int>>();
            _actorNameMap = new Dictionary<string, BaseActor>();

            for (int i = 0; i < SpawnedActors.Count; ++i)
            {
                BaseActor actor = SpawnedActors[i];
                Type t = actor.GetType();
                if (!_actorTypeMap.ContainsKey(t))
                    _actorTypeMap[t] = new HashSet<int>() { i };
                else
                    _actorTypeMap[t].Add(i);
                _actorNameMap.Add(actor.Name, actor);
            }
        }

        public IEnumerable<T> GetSpawnedActorsOfType<T>() where T : BaseActor
        {
            Type t = typeof(T);
            return _actorTypeMap.ContainsKey(t) ? _actorTypeMap[t].Select(x => (T)SpawnedActors[x]) : null;
        }
        public IEnumerable<BaseActor> GetSpawnedActorsOfType(Type actorType)
            => _actorTypeMap.ContainsKey(actorType) ? _actorTypeMap[actorType].Select(x => SpawnedActors[x]) : null;
    }
}
