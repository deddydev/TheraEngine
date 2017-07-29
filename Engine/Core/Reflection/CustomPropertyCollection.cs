using System.Collections;
using System.ComponentModel;

namespace System
{
    public class CustomProperty
    {
        public string _name;
        public object _value;
        public bool _readOnly;
        public bool _visible;
        public string _category;
        public string _description;
        public CustomProperty(
            string name,
            string category,
            object value, 
            bool readOnly, 
            bool visible,
            string description = "")
        {
            _name = name;
            _value = value;
            _readOnly = readOnly;
            _visible = visible;
            _description = description;
            _category = category;
        }
    }
    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        CustomProperty _property;

        public CustomPropertyDescriptor(CustomProperty property, Attribute[] attrs) 
            : base(property._name, attrs) { _property = property; }

        public override Type ComponentType { get { return null; } }
        public override bool IsReadOnly { get { return _property._readOnly; } }
        public override Type PropertyType { get { return _property._value.GetType(); } }

        public override object GetValue(object component)
        {
            return _property._value;
        }
        public override void SetValue(object component, object value)
        {
            _property._value = value;
        }
        public override bool CanResetValue(object component) { return false; }
        public override void ResetValue(object component) { }
        public override bool ShouldSerializeValue(object component) { return false; }
        public override string Description { get { return _property._description; } }
        public override string DisplayName { get { return _property._name; } }
        public override string Category { get { return _property._category; } }
        public override bool IsBrowsable { get { return _property._visible; } }
    }
    public class CustomPropertyCollection : CollectionBase, ICustomTypeDescriptor
    {
        public void Add(CustomProperty value) { List.Add(value); }
        public void Remove(string name)
        {
            foreach (CustomProperty prop in List)
                if (prop._name == name)
                {
                    List.Remove(prop);
                    return;
                }
        }
        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }
        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }
        public string GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }
        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }
        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
        public PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }
        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
        public PropertyDescriptorCollection GetProperties() { return TypeDescriptor.GetProperties(this, true); }
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptor[] newProps = new PropertyDescriptor[Count];
            for (int i = 0; i < Count; i++)
                newProps[i] = new CustomPropertyDescriptor(this[i], attributes);
            return new PropertyDescriptorCollection(newProps);
        }
        public object GetPropertyOwner(PropertyDescriptor pd) { return this; }
        public CustomProperty this[int index]
        {
            get { return (CustomProperty)List[index]; }
            set { List[index] = value; }
        }
    }
}