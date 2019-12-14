using System.Runtime.Serialization;
using TheraEngine.Core.Tools;

namespace System.ComponentModel
{
    public enum ENodeType
    {
        /// <summary>
        /// This is an attribute for the parent tree node. Object must be a string value.
        /// </summary>
        Attribute,
        /// <summary>
        /// This is a child element. The Object value is irrelevant if there is no member info.
        /// </summary>
        ChildElement,
        /// <summary>
        /// This is a string value that resides between the open and close tags. Object must be a string value and there must be no child elements.
        /// </summary>
        ElementContent,
    }
    /// <summary>
    /// This attribute means the field should be serialized upon saving.
    /// Note that the class/struct owning this property or field does NOT need to be a FileObject or a class in particular.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TSerialize : Attribute, ISerializable
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
        public ENodeType NodeType { get; set; } = ENodeType.ChildElement;

        /// <summary>
        /// Groups a set of fields together in one tag under this name.
        /// </summary>
        public string OverrideCategory { get; set; } = null;

        /// <summary>
        /// Determines if the field should be serialized using a boolean expression utilizing information from other fields and properties.
        /// </summary>
        public string Condition { get; set; } = null;

        /// <summary>
        /// Determines if the element should be grouped into a category,
        /// either using the category attribute or the OverrideCategory property.
        /// </summary>
        public bool UseCategory { get; set; } = false;

        public bool DeserializeAsync { get; set; } = false;

        public bool IsStreamable { get; set; } = false;
        
        /// <summary>
        /// Determines if this field should not be written if it is null or default.
        /// </summary>
        //public bool IgnoreIfDefault { get; set; } = true;
        public bool IsAttribute
        {
            get => NodeType == ENodeType.Attribute;
            set => NodeType = value ? ENodeType.Attribute : ENodeType.ChildElement;
        }
        public bool IsChildElement
        {
            get => NodeType == ENodeType.ChildElement;
            set => NodeType = value ? ENodeType.ChildElement : ENodeType.Attribute;
        }
        public bool IsElementString
        {
            get => NodeType == ENodeType.ElementContent;
            set => NodeType = value ? ENodeType.ElementContent : ENodeType.ChildElement;
        }
        /// <summary>
        /// Determines if this is the kind of value that is read from a main file on disk.
        /// </summary>
        public bool Config { get; set; } = true;
        /// <summary>
        /// Determines if this is the kind of value that is used at runtime.
        /// </summary>
        public bool State { get; set; } = false;

        /// <summary>
        /// Creates a new TSerialize attribute definition.
        /// </summary>
        public TSerialize() { }
        /// <summary>
        /// Creates a new TSerialize attribute definition.
        /// </summary>
        /// <param name="nameOverride">The name this property or field should go by in the file. If null, empty or whitespace, uses the field or property's name.</param>
        public TSerialize(string nameOverride) => NameOverride = nameOverride;
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

        public bool AllowSerialize(object context)
            => Condition is null ? true : ExpressionParser.Evaluate<bool>(Condition, context);

        protected TSerialize(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
                throw new ArgumentNullException(nameof(info));

            Order = info.GetInt32(nameof(Order));
            NameOverride = info.GetString(nameof(NameOverride));
            NodeType = (ENodeType)info.GetByte(nameof(NodeType));
            OverrideCategory = info.GetString(nameof(OverrideCategory));
            Condition = info.GetString(nameof(Condition));
            UseCategory = info.GetBoolean(nameof(UseCategory));
            DeserializeAsync = info.GetBoolean(nameof(DeserializeAsync));
            Config = info.GetBoolean(nameof(Config));
            State = info.GetBoolean(nameof(State));
        }
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue(nameof(Order), Order);
            info.AddValue(nameof(NameOverride), NameOverride);
            info.AddValue(nameof(NodeType), (byte)NodeType);
            info.AddValue(nameof(OverrideCategory), OverrideCategory);
            info.AddValue(nameof(Condition), Condition);
            info.AddValue(nameof(UseCategory), UseCategory);
            info.AddValue(nameof(DeserializeAsync), DeserializeAsync);
            info.AddValue(nameof(Config), Config);
            info.AddValue(nameof(State), State);
        }
    }
}
