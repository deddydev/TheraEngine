﻿namespace System.ComponentModel
{
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
        None = 0b0000,
        Binary = 0b0001,
        XML = 0b0010,
        //JSON    = 0b0100,
        //Text    = 0b1000,
        All = 0b1111,
    }
    public enum EXmlNodeType
    {
        ChildElement,
        Attribute,
        ElementString,
    }
    /// <summary>
    /// This attribute means the field should be serialized upon saving.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Serialize : Attribute
    {
        private int _order = -1;
        private string _nameOverride = null;
        private EXmlNodeType _xmlNodeType = EXmlNodeType.ChildElement;
        private string _xmlCategoryGrouping = null;
        private string _serializeIf = null;
        private bool _useCategory = false;
        private bool _ignoreIfNull = true;
        private bool _ignoreIfDefault = true;
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
        public EXmlNodeType XmlNodeType { get => _xmlNodeType; set => _xmlNodeType = value; }
        /// <summary>
        /// Groups a set of fields together in one tag under this name.
        /// </summary>
        public string OverrideXmlCategory { get => _xmlCategoryGrouping; set => _xmlCategoryGrouping = value; }
        /// <summary>
        /// Determines if the field should be serialized using an expression using information from other fields.
        /// </summary>
        public string Condition { get => _serializeIf; set => _serializeIf = value; }
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
        public bool IgnoreIfDefault { get => _ignoreIfDefault; set => _ignoreIfDefault = value; }

        public bool IsXmlAttribute
        {
            get => XmlNodeType == EXmlNodeType.Attribute;
            set => XmlNodeType = value ? EXmlNodeType.Attribute : EXmlNodeType.ChildElement;
        }
        public bool IsXmlChildElement
        {
            get => XmlNodeType == EXmlNodeType.ChildElement;
            set => XmlNodeType = value ? EXmlNodeType.ChildElement : EXmlNodeType.Attribute;
        }
        public bool IsXmlElementString
        {
            get => XmlNodeType == EXmlNodeType.ElementString;
            set => XmlNodeType = value ? EXmlNodeType.ElementString : EXmlNodeType.ChildElement;
        }

        public Serialize() { }
        public Serialize(string nameOverride)
        {
            _nameOverride = nameOverride;
        }
        public Serialize(string nameOverride, int order)
        {
            _order = order;
            _nameOverride = nameOverride;
        }
    }
}
