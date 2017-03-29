using CustomEngine.Files;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.InteropServices;

namespace CustomEngine
{
    public enum EngineQuality
    {
        Lowest,
        Low,
        Medium,
        High,
        Highest
    }
    public enum RenderLibrary
    {
        OpenGL,
        Direct3D11,
    }
    public enum AudioLibrary
    {
        OpenAL,
        DirectSound,
    }
    public enum InputLibrary
    {
        OpenTK,
        XInput,
        Raw,
    }
    [Serializable]
    public class UserSettings : FileObject
    {
        public bool VSync = true;
        public EngineQuality TextureQuality = EngineQuality.Highest;
        public EngineQuality ModelQuality = EngineQuality.Highest;
        public EngineQuality SoundQuality = EngineQuality.Highest;

        //Preferred libraries - will use whichever is available if the preferred one is not.
        public RenderLibrary RenderLibrary = RenderLibrary.OpenGL;
        public AudioLibrary AudioLibrary = AudioLibrary.OpenAL;
        public InputLibrary InputLibrary = InputLibrary.OpenTK;
        
        public static UserSettings FromXML(string filePath)
        {
            return FromXML<UserSettings>(filePath);
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
