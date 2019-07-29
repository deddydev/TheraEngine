using System.Runtime.Serialization;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace System
{
    /// <summary>
    /// Use this to serialize data in a manner that is more efficient or that the
    /// automatic serializer can't achieve. Label this method and the field/property
    /// with the same name and this method will be called to serialize that field/property.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomMemberSerializeMethod : SerializationAttribute
    {
        public string Name { get; set; }
        public CustomMemberSerializeMethod() : base() { }
        public CustomMemberSerializeMethod(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Name = info.GetString(nameof(Name));
        }
        public CustomMemberSerializeMethod(string name, EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All)
            : base(runForFormats) => Name = name;
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Name), Name);
        }
    }
    /// <summary>
    /// Use this to deserialize data in a manner that is more efficient or that the
    /// automatic deserializer can't achieve. Label this method and the field/property
    /// with the same name and this method will be called to deserialize that field/property.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomMemberDeserializeMethod : SerializationAttribute
    {
        public string Name { get; set; }
        public CustomMemberDeserializeMethod() : base() { }
        public CustomMemberDeserializeMethod(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Name = info.GetString(nameof(Name));
        }
        public CustomMemberDeserializeMethod(string name, EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All)
            : base(runForFormats) => Name = name;
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Name),Name);
        }
    }
}
