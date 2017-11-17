using TheraEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.GameModes;
using System.ComponentModel;
using TheraEngine.Worlds.Actors;

namespace TheraEngine.Worlds.Maps
{
    public class MapSettings : FileObject
    {
        protected bool _visibleByDefault;
        protected List<IActor> _defaultActors = new List<IActor>();
        protected Vec3 _spawnPosition;
        protected BaseGameMode _defaultGameMode;

        public MapSettings(params IActor[] actors)
        {
            _visibleByDefault = true;
            _spawnPosition = Vec3.Zero;
            _defaultActors = actors.ToList();
        }
        public MapSettings(bool visible, Vec3 spawnOrigin, params IActor[] actors)
        {
            _visibleByDefault = visible;
            _spawnPosition = spawnOrigin;
            _defaultActors = actors.ToList();
        }

        [TSerialize]
        public bool VisibleByDefault
        {
            get => _visibleByDefault;
            set => _visibleByDefault = value;
        }
        [TSerialize]
        public Vec3 SpawnPosition
        {
            get => _spawnPosition;
            set => _spawnPosition = value;
        }
        [TSerialize]
        public List<IActor> DefaultActors
        {
            get => _defaultActors;
            set => _defaultActors = value;
        }
        [TSerialize]
        public BaseGameMode DefaultGameMode
        {
            get => _defaultGameMode;
            set => _defaultGameMode = value;
        }
    }
}
