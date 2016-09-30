using CustomEngine.World.Maps;
using System.Collections.Generic;

namespace CustomEngine.World
{
    public class Map
    {
        private bool _visible;
        private bool _visibleByDefault;
        private List<Actor> _defaultActors = new List<Actor>();
        private MapSettings _settings;
    }
}
