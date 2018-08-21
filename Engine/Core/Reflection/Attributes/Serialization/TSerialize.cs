namespace System.ComponentModel
{
    public enum ESerializeFormat
    {
        Binary,
        XML,
        //JSON,
        //Text
    }
    [Flags]
    public enum ESerializeFormatFlag
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
        Attribute,
        ChildElement,
        ElementString,
    }
    /// <summary>
    /// This attribute means the field should be serialized upon saving.
    /// Note that the class/struct owning this property or field does NOT need to be a FileObject or a class in particular.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TSerialize : Attribute
    {
        /// <summary>
        /// The order this field should be serialized in.
        /// Defaults to the order this field appears in in the class.
        /// </summary>
        public int Order { get; set; } = -1;

        /// <summary>
        /// The name this field should be serialized with.
        /// Defaults to the name of the field.
        /// </summary>
        public string NameOverride { get; set; } = null;

        /// <summary>
        /// Determines if this object should be serialized as an attribute or element.
        /// </summary>
        public EXmlNodeType XmlNodeType { get; set; } = EXmlNodeType.ChildElement;

        /// <summary>
        /// Groups a set of fields together in one tag under this name.
        /// </summary>
        public string OverrideXmlCategory { get; set; } = null;

        /// <summary>
        /// Determines if the field should be serialized using a boolean expression utilizing information from other fields and properties.
        /// </summary>
        public string Condition { get; set; } = null;

        /// <summary>
        /// Determines if the element should be grouped into a category,
        /// either using the category attribute or the OverrideXmlCategory property.
        /// </summary>
        public bool UseCategory { get; set; } = false;

        /// <summary>
        /// Determines if this field should not be written if it is null.
        /// </summary>
        public bool IgnoreIfNull { get; set; } = true;
        public bool External { get; set; } = false;
        public bool IgnoreIfDefault { get; set; } = true;
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
        public bool Config { get; set; } = true;
        public bool State { get; set; } = false;

        /// <summary>
        /// Creates a new TSerialize attribute definition.
        /// </summary>
        public TSerialize() { }
        /// <summary>
        /// Creates a new TSerialize attribute definition.
        /// </summary>
        /// <param name="nameOverride">The name this property or field should go by in the file. If null, empty or whitespace, uses the field or property's name.</param>
        public TSerialize(string nameOverride)
        {
            NameOverride = nameOverride;
        }
        /// <summary>
        ///  Creates a new TSerialize attribute definition.
        /// </summary>
        /// <param name="nameOverride">The name this property or field should go by in the file. If null, empty or whitespace, uses the field or property's name.</param>
        /// <param name="order">Defines the order that this field or property should be written in in relation to the others, where 0 is first.</param>
        public TSerialize(string nameOverride, int order)
        {
            Order = order;
            NameOverride = nameOverride;
        }
    }
}
