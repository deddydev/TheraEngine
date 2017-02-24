using System;
using System.IO;
using System.Xml;
using CustomEngine.Files;

namespace CustomEngine.Worlds.Actors.Components
{
    public class InteractionComponent : LogicComponent
    {
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
