﻿using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Core.Files;
using TheraEngine.Rendering;
using TheraEngine.Actors;
using TheraEngine.GameModes;
using TheraEngine.ComponentModel;

namespace TheraEngine.Worlds
{
    [TFileExt("wsta")]
    [TFileDef("World State")]
    public class WorldState : TFileObject
    {
        [TSerialize]
        public List<IMap> SpawnedMaps { get; }
        [TSerialize]
        public EventList<IActor> SpawnedActors { get; }

        public IScene Scene { get; internal set; } = null;
        public IGameMode GameMode { get; internal set; } = null;

        public Dictionary<Type, HashSet<int>> _actorTypeMap;
        public Dictionary<string, IActor> _actorNameMap;

        public WorldState()
        {
            SpawnedMaps = new List<IMap>();
            SpawnedActors = new EventList<IActor>(false, false);
            _actorTypeMap = new Dictionary<Type, HashSet<int>>();
            _actorNameMap = new Dictionary<string, IActor>();

            SpawnedActors.PostAdded += _spawnedActors_Added;
            SpawnedActors.PostInserted += _spawnedActors_Inserted;

            SpawnedActors.PreAddedRange += _spawnedActors_AddedRange;
            SpawnedActors.PostInsertedRange += _spawnedActors_InsertedRange;
        }

        #region Spawned Actors
        private void _spawnedActors_Added(IActor item)
        {
            _spawnedActors_Inserted(item, SpawnedActors.Count - 1);
        }
        private void _spawnedActors_Inserted(IActor item, int index)
        {
            Type t = item.GetType();
            if (!_actorTypeMap.ContainsKey(t))
                _actorTypeMap[t] = new HashSet<int>() { index };
            else
                _actorTypeMap[t].Add(index);
        }
        private bool _spawnedActors_AddedRange(IEnumerable<IActor> items)
        {
            _spawnedActors_InsertedRange(items, SpawnedActors.Count);
            return true;
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

        [TPostDeserialize]
        internal void CreateActorMap()
        {
            _actorTypeMap = new Dictionary<Type, HashSet<int>>();
            _actorNameMap = new Dictionary<string, IActor>();

            for (int i = 0; i < SpawnedActors.Count; ++i)
            {
                IActor actor = SpawnedActors[i];
                Type t = actor.GetType();
                if (!_actorTypeMap.ContainsKey(t))
                    _actorTypeMap[t] = new HashSet<int>() { i };
                else
                    _actorTypeMap[t].Add(i);
                _actorNameMap.Add(actor.Name, actor);
            }
        }

        public IEnumerable<T> GetSpawnedActorsOfType<T>() where T : class, IActor
        {
            Type t = typeof(T);
            return _actorTypeMap.ContainsKey(t) ? _actorTypeMap[t].Select(x => (T)SpawnedActors[x]) : null;
        }
        public IEnumerable<IActor> GetSpawnedActorsOfType(Type actorType)
            => _actorTypeMap.ContainsKey(actorType) ? _actorTypeMap[actorType].Select(x => SpawnedActors[x]) : null;

        public void EndPlay()
        {
            int lastCount;
            while ((lastCount = SpawnedActors.Count) > 0)
            {
                var actor = SpawnedActors[0];
                actor?.Despawn();
                if (SpawnedActors.Count == lastCount)
                    SpawnedActors.RemoveAt(0);
            }
            SpawnedActors.Clear();

            foreach (var m in SpawnedMaps)
                if (m.IsVisible)
                    m.EndPlay();
            SpawnedMaps.Clear();
        }
    }
}
