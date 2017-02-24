using System;
using System.IO;
using System.Xml;
using CustomEngine.Files;

namespace CustomEngine.Particles
{
    public class Emitter : FileObject
    {
        public override ResourceType ResourceType { get { return ResourceType.Emitter; } }

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
