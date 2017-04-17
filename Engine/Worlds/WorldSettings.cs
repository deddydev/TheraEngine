using CustomEngine.Files;
using CustomEngine.GameModes;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using CustomEngine.Audio;

namespace CustomEngine.Worlds
{
    public class WorldSettings : FileObject
    {
        public BoundingBox OriginRebaseBounds
            => _originRebaseBounds;
        public BoundingBox Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }
        public WorldState State
        {
            get => _state;
            set => _state = value;
        }
        public SoundFile AmbientSound
        {
            get => _ambientSound;
            set => _ambientSound = value;
        }
        public List<Map> Maps
        {
            get => _maps;
            set => _maps = value;
        }
        public AudioParameters AmbientParams
        {
            get => _ambientParams;
            set => _ambientParams = value;
        }

        [Serialize("Bounds")]
        private BoundingBox _bounds = BoundingBox.FromMinMax(-500.0f, 500.0f);
        [Serialize("OriginRebaseBounds")]
        private BoundingBox _originRebaseBounds = BoundingBox.FromMinMax(float.MinValue, float.MaxValue);
        [Serialize("Maps")]
        private List<Map> _maps = new List<Map>();
        [Serialize("State")]
        private WorldState _state;
        [Serialize("AmbientSound")]
        private SoundFile _ambientSound = new SoundFile();
        [Serialize("AmbientParams")]
        private AudioParameters _ambientParams = new AudioParameters()
        {
            SourceRelative = new UsableValue<bool>(true, false, true),
            Gain = new UsableValue<float>(0.6f, 1.0f, true),
            Loop = new UsableValue<bool>(true, false, true),
        };

        public List<Material> CollectDefaultMaterials()
        {
            foreach (Map m in _maps)
            {
                if (m.Settings.VisibleByDefault)
                {

                }
            }
            return null;
        }

        public WorldSettings()
        {

        }
        public WorldSettings(string name, WorldState state, params Map[] maps)
        {
            _maps = maps.ToList();
            _originRebaseBounds = _bounds;
            _name = name;
            _state = state;
        }
        public WorldSettings(string name, WorldState state)
        {
            _originRebaseBounds = _bounds;
            _maps = new List<Map>();
            _name = name;
            _state = state;
        }
        public WorldSettings(string name)
        {
            _originRebaseBounds = _bounds;
            _maps = new List<Map>();
            _name = name;
            _state = new WorldState();
        }

        public void SetOriginRebaseDistance(float distance)
        {
            _originRebaseBounds = new BoundingBox(distance);
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
    }
}
