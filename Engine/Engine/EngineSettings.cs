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

        public bool SkinOnGPU = false;
        public SingleFileRef<World> TransitionWorld;
        public SingleFileRef<World> OpeningWorld;
        public string ContentPath;

        public override void Read(XmlReader reader)
        {
            base.Read(reader);
        }
        public override void Write(XmlWriter writer)
        {
            base.Write(writer);
            TransitionWorld.Write(writer);
        }
    }
}
