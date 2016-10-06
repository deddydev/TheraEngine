using System;
using System.IO;
using System.Xml.Serialization;

namespace CustomEngine
{
    [Serializable]
    public class EngineSettings
    {
        public string _transitionWorldPath;
        public string _openingWorldPath;
        public string _contentPath;

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
