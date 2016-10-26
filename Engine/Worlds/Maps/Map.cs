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
        public Map(MapSettings settings)
        {
            _settings = settings;
        }
        private MapSettings _settings;
        public MapSettings Settings { get { return _settings; } set { _settings = value; } }
    }
}
