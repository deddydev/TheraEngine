using CustomEngine;
using CustomEngine.Files;

namespace System
{
    /// <summary>
    /// This property is only used by the editor, NOT the game.
    /// </summary>
    public class EditorOnly : Attribute
    {
        
    }
    /// <summary>
    /// This property can be animated.
    /// </summary>
    public class Animatable : Attribute
    {

    }

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

    /// <summary>
    /// This attribute means the field should be serialized upon saving.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Serialize : Attribute
    {
        private int _order = -1;
        private string _nameOverride = null;
        private bool _isXmlAttribute = false;
        private object _defaultValue = null;
        private string _xmlCategoryGrouping = null;
        private string _serializeIf = null;
        private bool _useCategory = true;
        private bool _ignoreIfNull = true;
        private bool _external = false;

        /// <summary>
        /// The order this field should be serialized in.
        /// Defaults to the order this field appears in in the class.
        /// </summary>
        public int Order { get => _order; set => _order = value; }
        /// <summary>
        /// The name this field should be serialized with.
        /// Defaults to the name of the field.
        /// </summary>
        public string NameOverride { get => _nameOverride; set => _nameOverride = value; }
        /// <summary>
        /// Determines if this object should be serialized as an attribute or element.
        /// </summary>
        public bool IsXmlAttribute { get => _isXmlAttribute; set => _isXmlAttribute = value; }
        /// <summary>
        /// Determines what the default value of this field is.
        /// </summary>
        public object DefaultValue { get => _defaultValue; set => _defaultValue = value; }
        /// <summary>
        /// Groups a set of fields together in one tag under this name.
        /// </summary>
        public string OverrideXmlCategory { get => _xmlCategoryGrouping; set => _xmlCategoryGrouping = value; }
        /// <summary>
        /// Determines if the field should be serialized using an expression using information from other fields.
        /// </summary>
        public string SerializeIf { get => _serializeIf; set => _serializeIf = value; }
        /// <summary>
        /// Determines if the element should be grouped into a category,
        /// either using the category attribute or the OverrideXmlCategory property.
        /// </summary>
        public bool UseCategory { get => _useCategory; set => _useCategory = value; }
        /// <summary>
        /// Determines if this field should not be written if it is null.
        /// </summary>
        public bool IgnoreIfNull { get => _ignoreIfNull; set => _ignoreIfNull = value; }
        public bool External { get => _external; set => _external = value; }

        public Serialize() { }
        public Serialize(string nameOverride)
        {
            _nameOverride = nameOverride;
        }
        public Serialize(int order, string nameOverride)
        {
            _order = order;
            _nameOverride = nameOverride;
        }
        public Serialize(string nameOverride, int order)
        {
            _order = order;
            _nameOverride = nameOverride;
        }
    }
    public class ObjectHeader : Attribute
    {
        public ObjectHeader()
        {

        }
    }
    public enum SerializeFormat
    {
        Binary,
        XML,
        //JSON,
        //Text
    }
    [Flags]
    public enum SerializeFormatFlag
    {
        None    = 0b0000,
        Binary  = 0b0001,
        XML     = 0b0010,
        //JSON    = 0b0100,
        //Text    = 0b1000,
        All     = 0b1111,
    }
    public class FileClass : Attribute
    {
        public FileClass(string binaryTag, string extension, string userFriendlyName, bool manualBinSerialize = false, bool manualXmlSerialize = false, SerializeFormat preferredFormat = SerializeFormat.Binary)
        {
            _binaryTag = binaryTag;
            _extension = extension;
            _manualBinSerialize = manualBinSerialize;
            _manualXmlSerialize = manualXmlSerialize;
            _userFriendlyName = userFriendlyName;
            _preferredFormat = preferredFormat;
        }

        private string _userFriendlyName;
        private string _extension;
        private string _binaryTag;
        private bool _manualXmlSerialize = false;
        private bool _manualBinSerialize = false;
        private SerializeFormat _preferredFormat =
#if DEBUG
            SerializeFormat.XML;
#else
            SerializeFormat.Binary;
#endif
        
        public SerializeFormat PreferredFormat
        {
            get => _preferredFormat;
            set => _preferredFormat = value;
        }
        public bool ManualXmlSerialize
        {
            get => _manualXmlSerialize;
            set => _manualXmlSerialize = value;
        }
        public bool ManualBinSerialize
        {
            get => _manualBinSerialize;
            set => _manualBinSerialize = value;
        }
        public string UserFriendlyName
        {
            get => _userFriendlyName;
            set => _userFriendlyName = value;
        }
        public string Extension
        {
            get => _extension;
            set => _extension = value;
        }
        public string BinaryTag
        {
            get => _binaryTag;
            set => _binaryTag = value;
        }
    }
}