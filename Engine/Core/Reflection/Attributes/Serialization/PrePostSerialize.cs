﻿using System;
using TheraEngine.Core.Files;

namespace TheraEngine.Core.Reflection.Attributes.Serialization
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class SerializationAttribute : Attribute
    {
        public EProprietaryFileFormatFlag RunForFormats { get; }
        public SerializationAttribute(EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All)
        {
            RunForFormats = runForFormats;
        }
    }
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
    }
    /// <summary>
    /// Called after a class has just been serialized.
    /// </summary>
    public class PostSerialize : PrePostSerializationAttribute
    {
        public PostSerialize(int order = -1, EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All, params object[] arguments)
            : base(order, runForFormats, arguments) { }
    }
    /// <summary>
    /// Called before a class is serialized.
    /// </summary>
    public class PreSerialize : PrePostSerializationAttribute
    {
        public PreSerialize(int order = -1, EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All, params object[] arguments)
            : base(order, runForFormats, arguments) { }
    }
    /// <summary>
    /// Called after a class has just been deserialized.
    /// Useful for finalization after all values have been set.
    /// </summary>
    public class PostDeserialize : PrePostSerializationAttribute
    {
        public PostDeserialize(int order = -1, EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All, params object[] arguments)
            : base(order, runForFormats, arguments) { }
    }
    /// <summary>
    /// Called before a class is deserialized.
    /// Useful for setup before all values are set.
    /// </summary>
    public class PreDeserialize : PrePostSerializationAttribute
    {
        public PreDeserialize(int order = -1, EProprietaryFileFormatFlag runForFormats = EProprietaryFileFormatFlag.All, params object[] arguments)
            : base(order, runForFormats, arguments) { }
    }
}
