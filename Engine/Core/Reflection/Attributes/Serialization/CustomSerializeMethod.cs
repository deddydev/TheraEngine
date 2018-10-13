namespace System
{
    /// <summary>
    /// Use this to serialize data in a manner that is more efficient or that the
    /// automatic serializer can't achieve. Label this method and the field/property
    /// with the same name and this method will be called to serialize that field/property.
    /// <para>Method syntax is 'bool Serialize(XmlWriter writer, ESerializeFlags flags)'</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomSerializeMethod : Attribute
    {
        public string Name { get; set; }
        public CustomSerializeMethod(string name) => Name = name;
    }
    /// <summary>
    /// Use this to deserialize data in a manner that is more efficient or that the
    /// automatic deserializer can't achieve. Label this method and the field/property
    /// with the same name and this method will be called to deserialize that field/property.
    /// <para>Method syntax is 'bool Deserialize(XMLReader reader)'</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomDeserializeMethod : Attribute
    {
        public string Name { get; set; }
        public CustomDeserializeMethod(string name) => Name = name;
    }
}
