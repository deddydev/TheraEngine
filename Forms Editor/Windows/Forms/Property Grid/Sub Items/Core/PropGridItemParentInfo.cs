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
        public abstract object Target { get; set; }
        public abstract string DisplayName { get; }
        public abstract object Owner { get; set; }
    }
    public class PropGridItemRefNullableInfo : PropGridItemRefInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => _parentInfo.DisplayName;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override object Target { get => _parentInfo.Target; set => _parentInfo.Target = value; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override bool IsReadOnly() => _parentInfo.IsReadOnly();
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override object Owner
        {
            get => _parentInfo.Owner;
            set => _parentInfo.Owner = value;
        }
        public override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
            => _parentInfo.SubmitStateChange(oldValue, newValue, dataChangeHandler);
        private readonly PropGridItemRefInfo _parentInfo;

        public PropGridItemRefNullableInfo(PropGridItemRefInfo parentInfo, Type valueType)
        {
            if (parentInfo?.DataType == null || 
                !parentInfo.DataType.IsGenericType || 
                parentInfo.DataType.GetGenericTypeDefinition() != typeof(Nullable<>))
                throw new Exception();

            _parentInfo = parentInfo;
            DataType = valueType;
        }
    }
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
        public override object Owner { get; set; }
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
        public override object Target
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
    public class PropGridItemRefMethodInfo : PropGridItemRefInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => Method?.Name;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public MethodInfo Method { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override object Owner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => Method?.ReturnType;
        
        public PropGridItemRefMethodInfo(object owner, MethodInfo method)
        {
            Owner = owner;
            Method = method;
        }

        public override bool IsReadOnly() => false;
        public override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler) { }
        public override object Target { get { return null; } set { } }
    }
    public class PropGridItemRefEventInfo : PropGridItemRefInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => Event?.Name;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public EventInfo Event { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override object Owner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => Event?.EventHandlerType;

        public PropGridItemRefEventInfo(object owner, EventInfo @event)
        {
            Owner = owner;
            Event = @event;
        }

        public override bool IsReadOnly() => false;
        public override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler) { }
        public override object Target { get { return null; } set { } }
    }
    public class PropGridItemRefIListInfo : PropGridItemRefInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => string.Format("[{0}]", Index);
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public int Index { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override object Owner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public IList OwnerIList => Owner as IList;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => Owner == null ? null : (Index >= 0 && Index < OwnerIList.Count ? OwnerIList[Index]?.GetType() ?? _dataType : _dataType);
        private readonly Type _dataType;

        public PropGridItemRefIListInfo(IList owner, int index)
        {
            Owner = owner;
            Index = index;
            _dataType = OwnerIList?.DetermineElementType();
        }

        public override bool IsReadOnly()
        {
            return Owner == null || OwnerIList.IsReadOnly;
        }
        public override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
        {
            dataChangeHandler?.IListObjectChanged(oldValue, newValue, OwnerIList, Index);
        }
        public override object Target
        {
            get
            {
                if (Owner == null)
                    throw new InvalidOperationException();
                return OwnerIList[Index];
            }
            set
            {
                if (OwnerIList == null)
                    throw new InvalidOperationException();
                OwnerIList[Index] = value;
            }
        }
    }
    public class PropGridItemRefIDictionaryInfo : PropGridItemRefInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => string.Format("[{0}]", Target?.ToString());
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => Target?.GetType();
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public object Key { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public bool IsKey { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override object Owner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public IDictionary OwnerDictionary => Owner as IDictionary;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Type ValueType => OwnerDictionary.Contains(Key) ? (OwnerDictionary[Key]?.GetType() ?? _valueType) : _valueType;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Type KeyType => Key?.GetType() ?? _keyType;

        private Type _valueType, _keyType;

        public PropGridItemRefIDictionaryInfo(IDictionary owner, object key, bool isKey)
        {
            Owner = owner;
            Key = key;
            IsKey = isKey;
            _valueType = OwnerDictionary?.DetermineValueType();
            _keyType = OwnerDictionary?.DetermineKeyType();
        }

        public override bool IsReadOnly()
        {
            return Owner == null || OwnerDictionary.IsReadOnly;
        }
        public override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
        {
            dataChangeHandler?.IDictionaryObjectChanged(oldValue, newValue, OwnerDictionary, Key, IsKey);
        }
        public override object Target
        {
            get
            {
                if (IsKey)
                    return Key;
                else
                    return OwnerDictionary[Key];
            }
            set
            {
                IDictionary dic = OwnerDictionary;
                if (!IsKey)
                    dic[Key] = value;
                else
                {
                    object v = null;
                    if (dic.Contains(Key))
                    {
                        dic.Remove(Key);
                        v = dic[Key];
                    }
                    Key = value;
                    if (dic.Contains(Key))
                        dic[Key] = v;
                    else
                        dic.Add(Key, v);
                }
            }
        }
    }
}
