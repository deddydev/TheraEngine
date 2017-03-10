using OpenTK;
using System.Reflection;
using System;
using CustomEngine.Files;
using System.IO;
using System.Xml;

namespace CustomEngine.Audio
{
    public class StaticSoundEffect : SoundDataBase
    {
        private Vector3 _worldPosition;
        private float _loudness;

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }
    }
}
