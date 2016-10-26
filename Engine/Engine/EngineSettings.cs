using System;
using System.IO;
using System.Xml.Serialization;

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
    [Serializable]
    public class EngineSettings : FileObject
    {
        public string TransitionWorldPath;
        public string OpeningWorldPath;
        public string ContentPath;
        public EngineQuality TextureQuality;
        public EngineQuality ModelQuality;
        public EngineQuality SoundQuality;
        public bool VSync;
        
        public static EngineSettings FromXML(string filePath)
        {
            return FromXML<EngineSettings>(filePath);
        }
    }
}
