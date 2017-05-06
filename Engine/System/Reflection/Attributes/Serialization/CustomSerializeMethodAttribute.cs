using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Use this to serialize data in a manner that is more efficient or that the
    /// automatic serializer can't achieve. Label this method and the field/property
    /// with the same name and this method will be called to serialize that field/property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomSerializeMethod : Attribute
    {
        string _name;
        public CustomSerializeMethod(string name)
        {
            _name = name;
        }

        public string Name { get => _name; set => _name = value; }
    }   
    /// <summary>
    /// Use this to deserialize data in a manner that is more efficient or that the
    /// automatic deserializer can't achieve. Label this method and the field/property
    /// with the same name and this method will be called to deserialize that field/property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomDeserializeMethod : Attribute
    {
        string _name;
        public CustomDeserializeMethod(string name)
        {
            _name = name;
        }

        public string Name { get => _name; set => _name = value; }
    }
}
