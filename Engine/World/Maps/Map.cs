using CustomEngine.Worlds.Maps;
using System.Collections.Generic;
using CustomEngine.Rendering.Models;
using System;

namespace CustomEngine.Worlds
{
    public class Map : ILoadable
    {
        private bool _visible;
        private bool _visibleByDefault;
        private List<Actor> _defaultActors = new List<Actor>();
        private MapSettings _settings;
        private Vector3 _transform;

        #region ILoadable Interface
        private bool _isLoaded;
        private string _filePath;
        public void Unload()
        {
            _isLoaded = false;
        }
        public void Load()
        {
            _isLoaded = true;
        }
        public string FilePath { get { return _filePath; } }
        public bool IsLoaded { get { return _isLoaded; } }

        public System.Boolean VisibleByDefault { get; internal set; }
        #endregion
    }
}
