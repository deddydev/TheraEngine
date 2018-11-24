using System;
using System.ComponentModel;
using System.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridItemRefPropertyInfo : PropGridItemRefInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => Property?.Name;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public PropertyInfo Property { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Func<object> GetOwner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => Property?.PropertyType;

        public PropGridItemRefPropertyInfo(Func<object> owner, PropertyInfo property)
        {
            GetOwner = owner;
            Property = property;
        }

        public override bool IsReadOnly()
        {
            return Property == null || !Property.CanWrite;
        }
        internal protected override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
        {
            dataChangeHandler?.PropertyObjectChanged(oldValue, newValue, GetOwner, Property);
        }
        public override object MemberValue
        {
            get
            {
                if (Property == null)
                    throw new InvalidOperationException($"{nameof(Property)} cannot be null.");

                if (!Property.CanRead)
                    return DataType.GetDefaultValue();

                object o = GetOwner();

                //If the owner is null or does not own this property,
                //return the default value for the data type.
                //This specifically fixes the owner-property mismatch exception
                //when the grid is switching target sub object
                if (o == null || !Property.DeclaringType.IsAssignableFrom(o.GetType()))
                    return DataType.GetDefaultValue();

                return Property.GetValue(o);
            }
            set
            {
                if (Property == null)
                    throw new InvalidOperationException($"{nameof(Property)} cannot be null.");

                if (!Property.CanWrite)
                    return;

                object o = GetOwner();

                if (o != null && Property.DeclaringType.IsAssignableFrom(o.GetType()))
                    Property.SetValue(o, value);
            }
        }
    }
}
