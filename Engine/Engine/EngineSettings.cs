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
    public class EngineSettings
    {
        public string _transitionWorldPath;
        public string _openingWorldPath;
        public string _contentPath;
        public EngineQuality _textureQuality;
        public EngineQuality _modelQuality;
        public EngineQuality _soundQuality;
        public bool _vsync;
        
        public void SaveXML(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            using (StreamWriter writer = new StreamWriter(path))
            {
                XmlSerializer serializer = new XmlSerializer(GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }
        public static EngineSettings FromXML(string path)
        {
            if (!File.Exists(path))
                return null;
            using (FileStream stream = File.OpenRead(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(EngineSettings));
                return serializer.Deserialize(stream) as EngineSettings;
            }
        }
    }
}
