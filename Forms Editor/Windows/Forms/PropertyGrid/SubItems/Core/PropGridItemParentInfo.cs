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

        public PropGridItemParentPropertyInfo(object owner, PropertyInfo property)
        {
            Owner = owner;
            Property = property;
        }

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

        public PropGridItemParentIListInfo(IList owner, int index)
        {
            Owner = owner;
            Index = index;
        }

        public override bool IsReadOnly()
        {
            return Owner == null || Owner.IsReadOnly;
        }
        public override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
        {
            dataChangeHandler?.IListObjectChanged(oldValue, newValue, Owner, Index);
        }
        public override object Value
        {
            get
            {
                if (Owner == null)
                    throw new InvalidOperationException();
                return Owner[Index];
            }
            set
            {
                if (Owner == null)
                    throw new InvalidOperationException();
                Owner[Index] = value;
            }
        }
    }
    public class PropGridItemParentIDictionaryInfo : PropGridItemParentInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public object Key { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public bool IsKey { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public IDictionary Owner { get; set; }
        
        public PropGridItemParentIDictionaryInfo(IDictionary owner, object key, bool isKey)
        {
            Owner = owner;
            Key = key;
            IsKey = isKey;
        }

        public override bool IsReadOnly()
        {
            return Owner == null || Owner.IsReadOnly;
        }
        public override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
        {
            dataChangeHandler?.IDictionaryObjectChanged(oldValue, newValue, Owner, Key, IsKey);
        }
        public override object Value
        {
            get
            {
                if (IsKey)
                    return Key;
                else
                    return Owner[Key];
            }
            set
            {
                if (!IsKey)
                    Owner[Key] = value;
                else
                {
                    object v = null;
                    if (Owner.Contains(Key))
                    {
                        Owner.Remove(Key);
                        v = Owner[Key];
                    }
                    Key = value;
                    if (Owner.Contains(Key))
                        Owner[Key] = v;
                    else
                        Owner.Add(Key, v);
                }
            }
        }
    }
}
