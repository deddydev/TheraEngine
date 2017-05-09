using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using CustomEngine.GameModes;
using CustomEngine.Audio;
using System.ComponentModel;

namespace CustomEngine.Worlds.Maps
{
    public class MapSettings : FileObject
    {
        [Serialize]
        public bool _visibleByDefault;
        [Serialize]
        public List<IActor> _defaultActors;
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

        public void SetDefaultActors(params IActor[] actors)
        {
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
    }
}
