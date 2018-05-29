using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public abstract class PropGridItemParentInfo
    {
        public abstract bool IsReadOnly();
        public abstract void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler);
        public abstract object Value { get; set; }
    }
    public class PropGridItemParentPropertyInfo : PropGridItemParentInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public PropertyInfo Property { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public object Owner { get; set; }

        public override bool IsReadOnly()
        {
            return Property == null || !Property.CanWrite;
        }
        public override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
        {
            dataChangeHandler?.PropertyObjectChanged(oldValue, newValue, Owner, Property);
        }
        public override object Value
        {
            get
            {
                if (Property == null)
                    throw new InvalidOperationException();

                if (!Property.CanRead)
                    return null;

                return Property.GetValue(Owner);
            }
            set
            {
                if (Property == null)
                    throw new InvalidOperationException();

                if (!Property.CanWrite)
                    return;

                Property.SetValue(Owner, value);
            }
        }
    }
    public class PropGridItemParentIListInfo : PropGridItemParentInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public int Index { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public IList Owner { get; set; }

        public override bool IsReadOnly()
        {
            return Owner == null || Owner.IsReadOnly;
        }
        public override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
        {
            dataChangeHandler?.IListObjectChanged(oldValue, newValue, Owner, Index);
        }
        public override object GetValue()
        {
            if (Owner == null)
                throw new InvalidOperationException();
            return Owner[Index];
        }
    }
    public class PropGridItemParentIDictionaryInfo : PropGridItemParentInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public object Key { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public IDictionary Owner { get; set; }

        public bool IsKey => Key == null;
        public bool IsValue => Key != null;

        public override bool IsReadOnly()
        {
            return Owner == null || Owner.IsReadOnly;
        }
        public override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
        {
            dataChangeHandler?.IDictionaryObjectChanged(oldValue, newValue, Owner, Key);
        }
        public override object GetValue()
        {
            if (IsValue)
                return Owner[Key];
            else
                return Key;
        }
    }
}
