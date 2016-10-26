using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Worlds.Maps
{
    public class MapSettings : FileObject
    {
        private bool _visibleByDefault;
        private List<Actor> _defaultActors;
        private Vec3 _spawnPosition;

        public MapSettings(bool visible, Vec3 spawnOrigin, params Actor[] actors)
        {
            _visibleByDefault = visible;
            _defaultActors = actors.ToList();
            _spawnPosition = spawnOrigin;
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
