using CustomEngine.Files;
using CustomEngine.GameModes;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;

namespace CustomEngine.Worlds
{
    public class WorldSettings : FileObject
    {
        public override ResourceType ResourceType { get { return ResourceType.WorldSettings; } }

        public BoundingBox OriginRebaseBounds { get { return _originRebaseBounds; } }
        public BoundingBox WorldBounds { get { return _worldBounds; } set { _worldBounds = value; } }
        public WorldState State { get { return _state; } set { _state = value; } }

        private BoundingBox _worldBounds = BoundingBox.FromMinMax(new Vec3(-5000.0f), new Vec3(5000.0f));
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
            _originRebaseBounds = new BoundingBox(distance * 2.0f);
        }
        public override void Read(VoidPtr address, VoidPtr strings)
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

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct WorldSettingsHeader
    {
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}
