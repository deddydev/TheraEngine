using System;
using System.Runtime.Serialization;
using TheraEngine.Core.Files;

namespace TheraEngine.Core.Reflection.Attributes.Serialization
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class SerializationAttribute : Attribute, ISerializable
    {
        public EProprietaryFileFormatFlag RunForFormats { get; }
        public SerializationAttribute(EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All)
        {
            RunForFormats = runForFormats;
        }
        protected SerializationAttribute(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
                throw new ArgumentNullException(nameof(info));

            RunForFormats = (EProprietaryFileFormatFlag)info.GetUInt16(nameof(RunForFormats));
        }
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue(nameof(RunForFormats), (ushort)RunForFormats);
        }
    }
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class PrePostSerializationAttribute : SerializationAttribute
    {
        public int Order { get; set; }
        public object[] Arguments { get; set; }

        public PrePostSerializationAttribute(int order = -1, EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All, params object[] arguments)
            : base(runForFormats)
        {
            Order = order;
            Arguments = arguments;
        }
        protected PrePostSerializationAttribute(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info is null)
                throw new ArgumentNullException(nameof(info));

            Order = info.GetInt32(nameof(Order));
            Arguments = info.GetValue(nameof(Arguments), typeof(object[])) as object[];
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
                throw new ArgumentNullException(nameof(info));

            base.GetObjectData(info, context);
            info.AddValue(nameof(Order), Order);
            info.AddValue(nameof(Arguments), Arguments);
        }
    }
    /// <summary>
    /// Called before a class is serialized.
    /// </summary>
    [Serializable]
    public class TPreSerialize : PrePostSerializationAttribute
    {
        public TPreSerialize(int order = -1, EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All, params object[] arguments)
            : base(order, runForFormats, arguments) { }
        protected TPreSerialize(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
    /// <summary>
    /// Called after a class has just been serialized.
    /// </summary>
    [Serializable]
    public class TPostSerialize : PrePostSerializationAttribute
    {
        public TPostSerialize(int order = -1, EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All, params object[] arguments)
            : base(order, runForFormats, arguments) { }
        protected TPostSerialize(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
    /// <summary>
    /// Called before a class is deserialized.
    /// Useful for setup before all values are set.
    /// </summary>
    [Serializable]
    public class TPreDeserialize : PrePostSerializationAttribute
    {
        public TPreDeserialize(int order = -1, EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All, params object[] arguments)
            : base(order, runForFormats, arguments) { }
        protected TPreDeserialize(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
    /// <summary>
    /// Called after a class has just been deserialized.
    /// Useful for finalization after all values have been set.
    /// </summary>
    [Serializable]
    public class TPostDeserialize : PrePostSerializationAttribute
    {
        public TPostDeserialize(int order = -1, EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All, params object[] arguments)
            : base(order, runForFormats, arguments) { }
        protected TPostDeserialize(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
