using CustomEngine.Files;
using CustomEngine.GameModes;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CustomEngine.Worlds
{
    public class WorldSettings : FileObject
    {
        public BoundingBox OriginRebaseBounds { get { return _originRebaseBounds; } }
        public BoundingBox WorldBounds { get { return _worldBounds; } set { _worldBounds = value; } }
        public WorldState State { get { return _state; } set { _state = value; } }

        private BoundingBox _worldBounds = new BoundingBox(new Vec3(-5000.0f), new Vec3(5000.0f));
        private BoundingBox _originRebaseBounds;
        public List<Map> _defaultMaps;
        public WorldState _state;

        public List<Material> CollectDefaultMaterials()
        {
            foreach (Map m in _defaultMaps)
            {
                if (m.Settings.VisibleByDefault)
                {

                }
            }
            return null;
        }

        public WorldSettings(string name, WorldState state, params Map[] maps)
        {
            _defaultMaps = maps.ToList();
            _originRebaseBounds = _worldBounds;
            _name = name;
            _state = state;
        }
        public WorldSettings(string name, WorldState state)
        {
            _originRebaseBounds = _worldBounds;
            _defaultMaps = new List<Map>();
            _name = name;
            _state = state;
        }
        public WorldSettings(string name)
        {
            _originRebaseBounds = _worldBounds;
            _defaultMaps = new List<Map>();
            _name = name;
            _state = new WorldState();
        }

        public void SetOriginRebaseDistance(float distance)
        {
            _originRebaseBounds = new BoundingBox(Vec3.Zero, distance, distance, distance);
        }
        public override void Read(VoidPtr address)
        {
            //foreach (Map m in _maps)
            //    if (m.Settings.VisibleByDefault)
            //        m.Load();
            //_state._activeMaterials = new HashSet<Material>(CollectDefaultMaterials());
        }

        public static WorldSettings FromXML(string filePath)
        {
            return FromXML<WorldSettings>(filePath);
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct WorldSettingsHeader
    {
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}
