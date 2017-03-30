using CustomEngine.Files;
using CustomEngine.Worlds;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace CustomEngine
{
    public enum ShadingStyle
    {
        Forward = 0,
        Deferred = 1,
    }
    [ObjectHeader()]
    [FileHeader(ManualBinSerialize = false, ManualXmlSerialize = false)]
    public class EngineSettings : FileObject
    {
        [Category("Performance")]
        [Serialize]
        public ShadingStyle ShadingStyle = ShadingStyle.Forward;
        [Category("Performance")]
        [Serialize]
        public bool SkinOnGPU = false;
        [Category("Performance")]
        [Serialize]
        public bool UseIntegerWeightingIds = false;

        [Category("Debug")]
        [Serialize]
        public bool RenderCameraFrustums = true;
        [Category("Debug")]
        [Serialize]
        public bool RenderSkeletons = true;
        [Category("Debug")]
        [Serialize]
        public bool RenderOctree = true;
        [Category("Debug")]
        [Serialize]
        public bool RenderQuadtree = true;

        [Category("Frames Per Second")]
        [Serialize("Capped", XmlCategoryGrouping = "FramesPerSecond"/*, IsXmlAttribute = true*/)]
        public bool CapFPS = false;
        [Category("Frames Per Second")]
        [Serialize("Target", XmlCategoryGrouping = "FramesPerSecond", SerializeIf = "CapFPS == true")]
        public float TargetFPS = 60.0f;

        [Category("Updates Per Second")]
        [Serialize("Capped", XmlCategoryGrouping = "UpdatesPerSecond"/*, IsXmlAttribute = true*/)]
        public bool CapUPS = false;
        [Category("Updates Per Second")]
        [Serialize("Target", XmlCategoryGrouping = "UpdatesPerSecond", SerializeIf = "CapUPS == true")]
        public float TargetUPS = 120.0f;

        [Category("Game")]
        [Serialize]
        public SingleFileRef<World> TransitionWorld;
        [Category("Game")]
        [Serialize]
        public SingleFileRef<World> OpeningWorld;
        [Category("Game")]
        [Serialize]
        public string ContentPath = Engine.StartupPath + Engine.ContentFolderRel;

        public override void Read(XMLReader reader)
        {
            if (!reader.Name.Equals("EngineSettings", true))
                throw new Exception();

            while (reader.BeginElement())
            {
                if (reader.Name.Equals("contentPath", true))
                    ContentPath = reader.ReadElementString();
                else if (reader.Name.Equals("skinOnGPU", true))
                    SkinOnGPU = bool.Parse(reader.ReadElementString());
                else if (reader.Name.Equals("useIntegerWeightingIds", true))
                    UseIntegerWeightingIds = bool.Parse(reader.ReadElementString());
                else if (reader.Name.Equals("renderSkeletons", true))
                    RenderSkeletons = bool.Parse(reader.ReadElementString());
                else if (reader.Name.Equals("renderCameraFrustums", true))
                    RenderCameraFrustums = bool.Parse(reader.ReadElementString());
                else if (reader.Name.Equals("shadingStyle", true))
                    ShadingStyle = (ShadingStyle)Enum.Parse(typeof(ShadingStyle), reader.ReadElementString());
                else if (reader.Name.Equals("FPS", true))
                {
                    //Capped?
                    reader.ReadAttribute();
                    if (reader.Name.Equals("capped", true) && (CapFPS = bool.Parse((string)reader.Value)))
                        TargetFPS = float.Parse(reader.ReadElementString());
                }
                else if (reader.Name.Equals("UPS", true))
                {
                    //Capped?
                    reader.ReadAttribute();
                    if (reader.Name.Equals("capped", true) && (CapUPS = bool.Parse((string)reader.Value)))
                        TargetUPS = float.Parse(reader.ReadElementString());
                }
                reader.EndElement();
            }
        }
        public override void Write(XmlWriter writer)
        {
            writer.WriteStartElement("EngineSettings");
            writer.WriteElementString("contentPath", ContentPath.ToString());
            writer.WriteElementString("skinOnGPU", SkinOnGPU.ToString());
            writer.WriteElementString("useIntegerWeightingIds", UseIntegerWeightingIds.ToString());
            writer.WriteElementString("renderSkeletons", RenderSkeletons.ToString());
            writer.WriteElementString("renderCameraFrustums", RenderCameraFrustums.ToString());
            writer.WriteElementString("shadingStyle", ShadingStyle.ToString());
            writer.WriteStartElement("FPS");
            writer.WriteAttributeString("capped", CapFPS.ToString());
            if (CapFPS)
                writer.WriteString(TargetFPS.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("UPS");
            writer.WriteAttributeString("capped", CapUPS.ToString());
            if (CapUPS)
                writer.WriteString(TargetUPS.ToString());
            writer.WriteEndElement();
            TransitionWorld?.Write(writer, false);
            OpeningWorld?.Write(writer, false);
            writer.WriteEndElement();
        }

        public override void Write(VoidPtr address, StringTable table)
        {
            
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            
        }

        protected override int OnCalculateSize(StringTable table)
        {
            if (TransitionWorld.File != null)
                table.Add(TransitionWorld.RefPathAbsolute);
            if (OpeningWorld.File != null)
                table.Add(OpeningWorld.RefPathAbsolute);
            return Header.Size;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct Header
        {
            public const int Size = 0x18;
            
            private Bin32 _flags;
            public buint _contentPathString;
            public bfloat _framesPerSecond;
            public bfloat _updatesPerSecond;
            public FileRefHeader _openingWorld;
            public FileRefHeader _transitionWorld;

            public ShadingStyle ShadingStyle
            {
                get => (ShadingStyle)(_flags[0] ? 1 : 0);
                set => _flags[0] = value != 0;
            }
            public bool CapFPS
            {
                get => _flags[1];
                set => _flags[1] = value;
            }
            public bool CapUPS
            {
                get => _flags[2];
                set => _flags[2] = value;
            }
            public bool SkinOnGPU
            {
                get => _flags[3];
                set => _flags[3] = value;
            }
            public bool UseIntegerWeightingIds
            {
                get => _flags[4];
                set => _flags[4] = value;
            }
            public bool RenderCameraFrustums
            {
                get => _flags[5];
                set => _flags[5] = value;
            }
            public bool RenderSkeletons
            {
                get => _flags[6];
                set => _flags[6] = value;
            }
            public bool RenderOctree
            {
                get => _flags[7];
                set => _flags[7] = value;
            }
            public bool RenderQuadtree
            {
                get => _flags[8];
                set => _flags[8] = value;
            }

            public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        }
    }
}
