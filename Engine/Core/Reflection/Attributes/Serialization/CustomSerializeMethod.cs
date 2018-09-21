namespace System
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomBinarySerializeSizeMethod : Attribute
    {
        public string Name { get; set; }
        public CustomBinarySerializeSizeMethod(string name) => Name = name;
    }
    /// <summary>
    /// Use this to serialize data in a manner that is more efficient or that the
    /// automatic serializer can't achieve. Label this method and the field/property
    /// with the same name and this method will be called to serialize that field/property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomBinarySerializeMethod : Attribute
    {
        public string Name { get; set; }
        public CustomBinarySerializeMethod(string name) => Name = name;
    }
    /// <summary>
    /// Use this to serialize data in a manner that is more efficient or that the
    /// automatic serializer can't achieve. Label this method and the field/property
    /// with the same name and this method will be called to serialize that field/property.
    /// <para>Method syntax is 'bool Serialize(XmlWriter writer, ESerializeFlags flags)'</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomXMLSerializeMethod : Attribute
    {
        public string Name { get; set; }
        public CustomXMLSerializeMethod(string name) => Name = name;
    }
    /// <summary>
    /// Use this to deserialize data in a manner that is more efficient or that the
    /// automatic deserializer can't achieve. Label this method and the field/property
    /// with the same name and this method will be called to deserialize that field/property.
    /// <para>Method syntax is 'bool Deserialize(XMLReader reader)'</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomXMLDeserializeMethod : Attribute
    {
        public string Name { get; set; }
        public CustomXMLDeserializeMethod(string name) => Name = name;
    }
}
