using System;
using System.IO;
using System.Xml.Serialization;

namespace CustomEngine
{
    [Serializable]
    public class EngineSettings : FileObject
    {
        public string TransitionWorldPath;
        public string OpeningWorldPath;
        public string ContentPath;
        
        public static EngineSettings FromXML(string filePath)
        {
            return FromXML<EngineSettings>(filePath);
        }

        public override void Unload()
        {
            throw new NotImplementedException();
        }

        public override void Load(VoidPtr address)
        {

        }
    }
}
