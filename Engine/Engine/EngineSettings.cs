using CustomEngine.Files;
using CustomEngine.Worlds;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace CustomEngine
{
    public enum ShadingStyle
    {
        Forward,
        Deferred,
    }
    public class EngineSettings : FileObject
    {
        public override ResourceType ResourceType => ResourceType.EngineSettings;

        public bool SkinOnGPU = false;
        public bool UseIntegerWeightingIds = false;
        public bool RenderCameraFrustums = true;
        public bool RenderSkeletons = true;
        public bool CapFPS = true;
        public float TargetFPS = 30.0f;
        public bool CapUPS = true;
        public float TargetUPS = 90.0f;
        public ShadingStyle ShadingStyle = ShadingStyle.Deferred;

        public SingleFileRef<World> TransitionWorld;
        public SingleFileRef<World> OpeningWorld;
        public string ContentPath = Engine.StartupPath + Engine.ContentFolderRel;

        public override void Read(XMLReader reader)
        {

        }
        public override void Write(XmlWriter writer)
        {
            writer.WriteStartElement("EngineSettings");
            writer.WriteElementString("ContentPath", ContentPath.ToString());
            writer.WriteElementString("SkinOnGPU", SkinOnGPU.ToString());
            writer.WriteElementString("UseIntegerWeightingIds", UseIntegerWeightingIds.ToString());
            writer.WriteElementString("RenderSkeletons", RenderSkeletons.ToString());
            writer.WriteElementString("RenderCameraFrustums", RenderCameraFrustums.ToString());
            writer.WriteStartElement("FPS");
            writer.WriteAttributeString("Capped", CapFPS.ToString());
            writer.WriteString(TargetFPS.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("FPS");
            writer.WriteAttributeString("Capped", CapFPS.ToString());
            writer.WriteString(TargetFPS.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("UPS");
            writer.WriteAttributeString("Capped", CapUPS.ToString());
            writer.WriteString(TargetUPS.ToString());
            writer.WriteEndElement();
            TransitionWorld?.Write(writer, false);
            OpeningWorld?.Write(writer, false);
            writer.WriteEndElement();
        }

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }
    }
}
