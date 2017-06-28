using TheraEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using TheraEngine.GameModes;
using TheraEngine.Audio;
using System.ComponentModel;

namespace TheraEngine.Worlds.Maps
{
    public class MapSettings : FileObject
    {
        [Serialize]
        public bool _visibleByDefault;
        [Serialize]
        private List<IActor> _defaultActors = new List<IActor>();
        [Serialize]
        public Vec3 _spawnPosition;
        [Serialize]
        public BaseGameMode _defaultGameMode;

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
        
        public bool VisibleByDefault
        {
            get => _visibleByDefault;
            set => _visibleByDefault = value;
        }
        public Vec3 SpawnPosition
        {
            get => _spawnPosition;
            set => _spawnPosition = value;
        }
        public List<IActor> DefaultActors
        {
            get => _defaultActors;
            set => _defaultActors = value;
        }
    }
}
