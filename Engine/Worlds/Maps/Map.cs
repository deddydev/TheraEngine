using CustomEngine.Worlds.Maps;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;

namespace CustomEngine.Worlds
{
    public class Map : FileObject
    {
        public Map(bool visible, Vec3 spawnOrigin, MapSettings settings, params Actor[] actors)
        {
            _visibleByDefault = _visible = visible;
            _settings = settings;
            _position = spawnOrigin;
            _defaultActors = actors.ToList();
        }

        private bool _visible;
        private bool _visibleByDefault;
        private List<Actor> _defaultActors = new List<Actor>();
        private MapSettings _settings;
        private Vec3 _position;

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
            get { return _position; }
#if EDITOR
            set { _position = value; }
#endif
        }
        [State]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
    }
}
