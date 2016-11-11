using CustomEngine.GameModes;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Worlds
{
    public class WorldSettings : FileObject
    {
        public Box OriginRebaseBounds { get { return _originRebaseBounds; } }
        public Box WorldBounds { get { return _worldBounds; } set { _worldBounds = value; } }
        public WorldState State { get { return _state; } set { _state = value; } }

        private Box _worldBounds = new Box(new Vec3(-5000.0f), new Vec3(5000.0f));
        private Box _originRebaseBounds;
        public List<Map> _maps;
        public WorldState _state;

        public List<Material> CollectDefaultMaterials()
        {
            foreach (Map m in _maps)
            {
                
            }
            return null;
        }

        public WorldSettings(string name, WorldState state, params Map[] maps)
        {
            _maps = maps.ToList();
            _originRebaseBounds = _worldBounds;
            _name = name;
            _state = state;
        }
        public WorldSettings(string name, WorldState state)
        {
            _originRebaseBounds = _worldBounds;
            _maps = new List<Map>();
            _name = name;
            _state = state;
        }
        public WorldSettings(string name)
        {
            _originRebaseBounds = _worldBounds;
            _maps = new List<Map>();
            _name = name;
            _state = new WorldState();
        }

        public void SetOriginRebaseDistance(float distance)
        {
            _originRebaseBounds = new Box(distance, distance, distance);
        }
        protected override void Read(VoidPtr address)
        {
            foreach (Map m in _maps)
                if (m.Settings.VisibleByDefault)
                    m.Load();
            _state._activeMaterials = new HashSet<Material>(CollectDefaultMaterials());
        }

        public static WorldSettings FromXML(string filePath)
        {
            return FromXML<WorldSettings>(filePath);
        }

        public static string GetTag() { return "WSET"; }
    }
}
