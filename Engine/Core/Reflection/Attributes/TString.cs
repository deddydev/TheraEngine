using System;

namespace TheraEngine.Core.Reflection.Attributes
{
    public class TStringAttribute : Attribute
    {
        public bool MultiLine { get; set; }
        public bool Path { get; set; }
        public bool Unicode { get; set; }
        public TStringAttribute(bool multiLine = false, bool path = false, bool unicode = false)
        {
            MultiLine = multiLine;
            Path = path;
            Unicode = unicode;
        }
    }
}
