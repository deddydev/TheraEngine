using CustomEngine.World.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.World
{
    public class Map
    {
        private bool _loaded;
        private bool _rendering;
        private bool _renderByDefault;
        private List<Actor> _defaultActors = new List<Actor>();
        private MapSettings _settings;
    }
}
