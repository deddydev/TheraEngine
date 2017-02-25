using CustomEngine.Files;
using CustomEngine.Worlds;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace CustomEngine
{
    public class EngineSettings : FileObject
    {
        public override ResourceType ResourceType { get { return ResourceType.EngineSettings; } }

        public bool SkinOnGPU = true;
        public SingleFileRef<World> TransitionWorld;
        public SingleFileRef<World> OpeningWorld;
        public string ContentPath;

        public override void Read(XMLReader reader)
        {

        }
        public override void Write(XmlWriter writer)
        {

            TransitionWorld.Write(writer);
        }

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }
    }
}
