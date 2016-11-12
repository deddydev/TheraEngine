using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Worlds.Maps
{
    public class MapSettings : FileObject
    {
        public bool _visibleByDefault;
        public List<Actor> _defaultActors;
        public Vec3 _spawnPosition;

        public MapSettings(params Actor[] actors)
        {
            _visibleByDefault = true;
            _spawnPosition = Vec3.Zero;
            _defaultActors = actors.ToList();
        }
        public MapSettings(bool visible, Vec3 spawnOrigin, params Actor[] actors)
        {
            _visibleByDefault = visible;
            _spawnPosition = spawnOrigin;
            _defaultActors = actors.ToList();
        }

        public void SetDefaultActors(params Actor[] actors)
        {
            _defaultActors = actors.ToList();
        }

        [Default]
        public bool VisibleByDefault
        {
            get { return _visibleByDefault; }
#if EDITOR
            set { _visibleByDefault = value; }
#endif
        }
        [Default]
        public Vec3 SpawnPosition
        {
            get { return _spawnPosition; }
#if EDITOR
            set { _spawnPosition = value; }
#endif
        }
    }
}
