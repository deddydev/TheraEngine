using System;
using System.ComponentModel;

namespace TheraEngine.Core.Reflection.Attributes.Serialization
{
    /// <summary>
    /// Called after a class has just been deserialized.
    /// Can be used for setup after all values have been set.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PostDeserialize : Attribute
    {
        public PostDeserialize(int order = -1, ESerializeFormatFlag runForFormats = ESerializeFormatFlag.All, params object[] arguments)
        {
            Order = order;
            Arguments = arguments;
            RunForFormats = runForFormats;
        }

        public int Order { get; set; } = -1;
        public object[] Arguments { get; set; }
        public ESerializeFormatFlag RunForFormats { get; set; }
    }
    /// <summary>
    /// Called before a class is deserialized.
    /// Can be used for setup before all values are set.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PreDeserialize : Attribute
    {
        public PreDeserialize(int order = -1, ESerializeFormatFlag runForFormats = ESerializeFormatFlag.All, params object[] arguments)
        {
            Order = order;
            Arguments = arguments;
            RunForFormats = runForFormats;
        }

        public int Order { get; set; } = -1;
        public object[] Arguments { get; set; }
        public ESerializeFormatFlag RunForFormats { get; set; }
    }

}
