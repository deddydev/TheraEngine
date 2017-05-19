using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomBinarySerializeSizeMethod : Attribute
    {
        string _name;
        public CustomBinarySerializeSizeMethod(string name)
        {
            _name = name;
        }
        public string Name
        {
            get => _name;
            set => _name = value;
        }
    }
    /// <summary>
    /// Use this to serialize data in a manner that is more efficient or that the
    /// automatic serializer can't achieve. Label this method and the field/property
    /// with the same name and this method will be called to serialize that field/property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomBinarySerializeMethod : Attribute
    {
        string _name;
        public CustomBinarySerializeMethod(string name)
        {
            _name = name;
        }
        public string Name
        {
            get => _name;
            set => _name = value;
        }
    }
    /// <summary>
    /// Use this to serialize data in a manner that is more efficient or that the
    /// automatic serializer can't achieve. Label this method and the field/property
    /// with the same name and this method will be called to serialize that field/property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomXMLSerializeMethod : Attribute
    {
        string _name;
        public CustomXMLSerializeMethod(string name)
        {
            _name = name;
        }
        public string Name
        {
            get => _name;
            set => _name = value;
        }
    }
    /// <summary>
    /// Use this to deserialize data in a manner that is more efficient or that the
    /// automatic deserializer can't achieve. Label this method and the field/property
    /// with the same name and this method will be called to deserialize that field/property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomXMLDeserializeMethod : Attribute
    {
        string _name;
        public CustomXMLDeserializeMethod(string name)
        {
            _name = name;
        }
        public string Name
        {
            get => _name;
            set => _name = value;
        }
    }
}
