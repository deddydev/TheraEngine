using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridItemRefIDictionaryInfo : PropGridItemRefInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => string.Format("[{0}]", MemberValue?.ToString());
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => MemberValue?.GetType();
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
        public override object MemberValue
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
