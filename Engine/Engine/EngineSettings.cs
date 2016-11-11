using CustomEngine.Worlds;
using System;
using System.IO;
using System.Xml.Serialization;

namespace CustomEngine
{
    [Serializable]
    public class EngineSettings : FileObject
    {
        [XmlElement(ElementName = "TransitionWorld")]
        public FileRef<World> TransitionWorld;
        [XmlElement(ElementName = "OpeningWorld")]
        public FileRef<World> OpeningWorld;
        [XmlElement(ElementName = "ContentFolder")]
        public string ContentPath;
        
        public static EngineSettings FromXML(string filePath)
        {
            return FromXML<EngineSettings>(filePath);
        }
    }
}
