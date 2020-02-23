using Extensions;
using System;
using System.ComponentModel;
using TheraEngine.Core.Reflection;
using TheraEngine.Editor;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridMemberInfoProperty : PropGridMemberInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string MemberAccessor => "." + Property?.Name ?? "<null>";
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => Property?.Name;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public PropertyInfoProxy Property { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override TypeProxy DataType => Property?.PropertyType;

        public PropGridMemberInfoProperty(IPropGridMemberOwner owner, PropertyInfoProxy property) : base(owner)
            => Property = property;

        public override bool IsReadOnly()
            => base.IsReadOnly() || Property.IsNull() || !Property.CanWrite;
        
        internal protected override void SubmitStateChange(object oldValue, object newValue, ValueChangeHandler dataChangeHandler)
            => dataChangeHandler?.HandleChange(new LocalValueChangeProperty(oldValue, newValue, Owner.Value, Property));
        
        public override object MemberValue
        {
            get
            {
                if (Property.IsNull())
                    throw new InvalidOperationException($"{nameof(Property)} cannot be null.");

                if (!Property.CanRead)
                    return DataType.GetDefaultValue();

                object o = Owner.Value;

                //If the owner is null or does not own this property,
                //return the default value for the data type.
                //This specifically fixes the owner-property mismatch exception
                //when the grid is switching target sub object
                if (o.IsNull() || !Property.DeclaringType.IsAssignableFrom(o.GetTypeProxy()))
                    return DataType.GetDefaultValue();

                return Property.GetValue(o);
            }
            set
            {
                if (Property.IsNull())
                    throw new InvalidOperationException($"{nameof(Property)} cannot be null.");

                if (!Property.CanWrite)
                    return;

                object o = Owner.Value;

                if (o.IsNotNull() && Property.DeclaringType.IsAssignableFrom(o.GetTypeProxy()))
                    Property.SetValue(o, value);
            }
        }
    }
}
