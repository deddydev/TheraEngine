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
        private int _order = -1;
        private object[] _arguments;
        private SerializeFormatFlag _runForFormats;
        public PostDeserialize(int order = -1, SerializeFormatFlag runForFormats = SerializeFormatFlag.All, params object[] arguments)
        {
            _order = order;
            _arguments = arguments;
            _runForFormats = runForFormats;
        }

        public int Order { get => _order; set => _order = value; }
        public object[] Arguments { get => _arguments; set => _arguments = value; }
        public SerializeFormatFlag RunForFormats { get => _runForFormats; set => _runForFormats = value; }
    }
    /// <summary>
    /// Called before a class is deserialized.
    /// Can be used for setup before all values are set.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PreDeserialize : Attribute
    {
        private int _order = -1;
        private object[] _arguments;
        private SerializeFormatFlag _runForFormats;
        public PreDeserialize(int order = -1, SerializeFormatFlag runForFormats = SerializeFormatFlag.All, params object[] arguments)
        {
            _order = order;
            _arguments = arguments;
            _runForFormats = runForFormats;
        }

        public int Order { get => _order; set => _order = value; }
        public object[] Arguments { get => _arguments; set => _arguments = value; }
        public SerializeFormatFlag RunForFormats { get => _runForFormats; set => _runForFormats = value; }
    }

}
