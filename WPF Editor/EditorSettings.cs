using System;
using System.IO;
using System.Xml.Serialization;

namespace TheraEditor
{
    [Serializable]
    public class EditorSettings
    {
        public string _contentMonitorPath;
        public bool _outlineHovered;
        public bool _outlineSelected;
        
        public void SaveXML(string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                XmlSerializer serializer = new XmlSerializer(GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }
        public static EditorSettings FromXML(string path)
        {
            if (!File.Exists(path))
                return null;
            using (FileStream stream = File.OpenRead(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(EditorSettings));
                return serializer.Deserialize(stream) as EditorSettings;
            }
        }
    }
}
