﻿using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace System
{
    /// <summary>
    /// Use this to serialize data in a manner that is more efficient or that the
    /// automatic serializer can't achieve. Label this method and the field/property
    /// with the same name and this method will be called to serialize that field/property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomSerializeMethod : SerializationAttribute
    {
        public string Name { get; set; }
        public CustomSerializeMethod(string name, EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All)
            : base(runForFormats) => Name = name;
    }
    /// <summary>
    /// Use this to deserialize data in a manner that is more efficient or that the
    /// automatic deserializer can't achieve. Label this method and the field/property
    /// with the same name and this method will be called to deserialize that field/property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomDeserializeMethod : SerializationAttribute
    {
        public string Name { get; set; }
        public CustomDeserializeMethod(string name, EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All)
            : base(runForFormats) => Name = name;
        
    }
}
