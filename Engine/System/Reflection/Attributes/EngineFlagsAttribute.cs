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
    //public delegate void Write(VoidPtr address, StringTable table);
    /// <summary>
    /// This attribute means the field should be serialized upon saving.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class Serialize : Attribute
    {
        private int _order = -1;
        private string _nameOverride = null;
        private bool _isXmlAttribute = false;
        private object _defaultValue = null;
        private string _xmlCategoryGrouping = null;
        private string _serializeIf = null;

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
        public string XmlCategoryGrouping { get => _xmlCategoryGrouping; set => _xmlCategoryGrouping = value; }
        /// <summary>
        /// Determines if the field should be serialized using an expression using information from other fields.
        /// </summary>
        public string SerializeIf { get => _serializeIf; set => _serializeIf = value; }

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
    public class FileHeader : Attribute
    {
        private bool _manualXmlSerialize = false;
        private bool _manualBinSerialize = false;
        
        public bool ManualXmlSerialize { get => _manualXmlSerialize; set => _manualXmlSerialize = value; }
        public bool ManualBinSerialize { get => _manualBinSerialize; set => _manualBinSerialize = value; }
    }
}