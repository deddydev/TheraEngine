using System;

namespace TheraEngine.Core.Files.Serialization
{
    public class SerializeAttribute : SerializeElementContent
    {
        public SerializeAttribute() { }
        public SerializeAttribute(string name, object value) : base(value) { Name = name; }

        public string Name { get; set; }

        public static SerializeAttribute FromString(string name, string value)
        {
            SerializeAttribute attrib = new SerializeAttribute(name, null);
            attrib.SetValueAsString(value);
            return attrib;
        }
        public static SerializeAttribute FromString(string name, string value, Type objectType, out bool parseSucceeded)
        {
            SerializeAttribute attrib = new SerializeAttribute(name, null);
            parseSucceeded = attrib.SetValueAsString(value, objectType);
            return attrib;
        }
    }
}
