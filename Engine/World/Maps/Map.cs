using CustomEngine.Worlds.Maps;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Reflection;

namespace CustomEngine.Worlds
{
    public class Map : ObjectBase, ILoadable
    {
        private bool _visible;
        private bool _visibleByDefault;
        private List<Actor> _defaultActors = new List<Actor>();
        private MapSettings _settings;
        private Vec3 _position;

        public bool VisibleByDefault { get { return _visibleByDefault; } set { _visibleByDefault = value; } }

        #region ILoadable Interface
        private bool _isLoaded, _isLoading;
        private string _filePath;
        public async Task Unload()
        {
            _isLoaded = false;
        }
        public async Task Load()
        {
            _isLoading = true;

            _isLoading = false;
            _isLoaded = true;
        }
        public string FilePath { get { return _filePath; } }
        public bool IsLoading { get { return _isLoading; } }
        public bool IsLoaded { get { return _isLoaded; } }
        #endregion
    }
}
