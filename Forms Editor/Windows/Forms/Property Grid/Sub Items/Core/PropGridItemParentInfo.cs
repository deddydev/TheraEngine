using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public abstract class PropGridItemRefInfo
    {
        public abstract Type DataType { get; }
        public abstract bool IsReadOnly();
        public abstract void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler);
        public abstract object Value { get; set; }
    }
    public class PropGridItemRefDirectInfo : PropGridItemRefInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => Value?.GetType() ?? _type;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override object Value { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override bool IsReadOnly() => false;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
        {

        }

        private readonly Type _type;

        public PropGridItemRefDirectInfo(object value, Type type)
        {
            Value = value;
            _type = type;
        }
    }
    public class PropGridItemRefPropertyInfo : PropGridItemRefInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public PropertyInfo Property { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public object Owner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => Property?.PropertyType;

        public PropGridItemRefPropertyInfo(object owner, PropertyInfo property)
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
    public class PropGridItemRefIListInfo : PropGridItemRefInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public int Index { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public IList Owner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => Owner == null ? null : (Index >= 0 && Index < Owner.Count ? Owner[Index]?.GetType() ?? _dataType : _dataType);
        private readonly Type _dataType;

        public PropGridItemRefIListInfo(IList owner, int index)
        {
            Owner = owner;
            Index = index;
            _dataType = Owner?.DetermineElementType();
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
    public class PropGridItemRefIDictionaryInfo : PropGridItemRefInfo
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Type ValueType => Owner.Contains(Key) ? (Owner[Key]?.GetType() ?? _valueType) : _valueType;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Type KeyType => Key?.GetType() ?? _keyType;

        private Type _valueType, _keyType;

        public PropGridItemRefIDictionaryInfo(IDictionary owner, object key, bool isKey)
        {
            Owner = owner;
            Key = key;
            IsKey = isKey;
            _valueType = Owner?.DetermineValueType();
            _keyType = Owner?.DetermineKeyType();
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

        public override Type DataType => throw new NotImplementedException();
    }
}
