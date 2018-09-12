using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public abstract class PropGridItemRefInfo
    {
        /// <summary>
        /// The type that is expected of this instance's <see cref="Target"/>.
        /// </summary>
        public abstract Type DataType { get; }
        /// <summary>
        /// False if the <see cref="Target"/> can be set.
        /// </summary>
        public abstract bool IsReadOnly();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="dataChangeHandler"></param>
        internal protected abstract void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler);
        /// <summary>
        /// 
        /// </summary>
        public abstract object Target { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public abstract string DisplayName { get; }
        /// <summary>
        /// Retrieves a boxed version of the owner's current state.
        /// </summary>
        public abstract Func<object> GetOwner { get; set; }
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
        public override Func<object> GetOwner
        {
            get => _parentInfo.GetOwner;
            set => _parentInfo.GetOwner = value;
        }
        internal protected override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
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
        public override object Target
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
        public override Func<object> GetOwner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => Method?.ReturnType;
        
        public PropGridItemRefMethodInfo(Func<object> owner, MethodInfo method)
        {
            GetOwner = owner;
            Method = method;
        }

        public override bool IsReadOnly() => false;
        internal protected override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler) { }
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
        public override Func<object> GetOwner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => Event?.EventHandlerType;

        public PropGridItemRefEventInfo(Func<object> owner, EventInfo @event)
        {
            GetOwner = owner;
            Event = @event;
        }

        public override bool IsReadOnly() => false;
        internal protected override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler) { }
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
        public override Func<object> GetOwner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public IList OwnerIList => GetOwner() as IList;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => GetOwner() == null ? null : (Index >= 0 && Index < OwnerIList.Count ? OwnerIList[Index]?.GetType() ?? _dataType : _dataType);
        private readonly Type _dataType;

        public PropGridItemRefIListInfo(Func<object> owner, int index)
        {
            GetOwner = owner;
            Index = index;
            _dataType = OwnerIList?.DetermineElementType();
        }

        public override bool IsReadOnly()
        {
            return OwnerIList == null || OwnerIList.IsReadOnly;
        }
        internal protected override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
        {
            dataChangeHandler?.IListObjectChanged(oldValue, newValue, OwnerIList, Index);
        }
        public override object Target
        {
            get
            {
                if (GetOwner == null)
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
        public override Func<object> GetOwner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public IDictionary OwnerDictionary => GetOwner() as IDictionary;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Type ValueType => OwnerDictionary.Contains(Key) ? (OwnerDictionary[Key]?.GetType() ?? _valueType) : _valueType;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Type KeyType => Key?.GetType() ?? _keyType;

        private Type _valueType, _keyType;

        public PropGridItemRefIDictionaryInfo(Func<object> owner, object key, bool isKey)
        {
            GetOwner = owner;
            Key = key;
            IsKey = isKey;
            _valueType = OwnerDictionary?.DetermineValueType();
            _keyType = OwnerDictionary?.DetermineKeyType();
        }

        public override bool IsReadOnly() => GetOwner == null || OwnerDictionary.IsReadOnly;
        internal protected override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
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
