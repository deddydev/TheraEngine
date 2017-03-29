using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using CustomEngine.GameModes;

namespace CustomEngine.Worlds.Maps
{
    public class MapSettings : FileObject
    {
        [Serialize]
        public bool _visibleByDefault;
        [Serialize]
        public List<IActor> _defaultActors;
        [Serialize]
        public Vec3 _spawnPosition;
        [Serialize]
        public GameMode _defaultGameMode;

        public MapSettings(params IActor[] actors)
        {
            _visibleByDefault = true;
            _spawnPosition = Vec3.Zero;
            _defaultActors = actors.ToList();
        }
        public MapSettings(bool visible, Vec3 spawnOrigin, params IActor[] actors)
        {
            _visibleByDefault = visible;
            _spawnPosition = spawnOrigin;
            _defaultActors = actors.ToList();
        }

        public void SetDefaultActors(params IActor[] actors)
        {
            _defaultActors = actors.ToList();
        }

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
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

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }
        
        public bool VisibleByDefault
        {
            get { return _visibleByDefault; }
#if EDITOR
            set { _visibleByDefault = value; }
#endif
        }
        public Vec3 SpawnPosition
        {
            get { return _spawnPosition; }
#if EDITOR
            set { _spawnPosition = value; }
#endif
        }
    }
}
