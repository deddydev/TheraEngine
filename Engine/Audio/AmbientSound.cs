using System;
using System.IO;
using System.Xml;
using CustomEngine.Files;

namespace CustomEngine.Audio
{
    public class AmbientSound : SoundDataBase
    {
        AmbientSound()
        {
            _looped = true;
        }

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
    }
}
