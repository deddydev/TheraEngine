using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            using (StreamWriter writer = new StreamWriter(path))
            {
                XmlSerializer serializer = new XmlSerializer(GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }
        public static EngineSettings FromXML(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(EngineSettings));
                return serializer.Deserialize(stream) as EngineSettings;
            }
        }
    }
}
