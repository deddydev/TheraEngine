using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Editor;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridItemRefIDictionaryInfo : PropGridMemberInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string MemberAccessor => IsKey ? $".Keys[{MemberValue}]" : $"[{MemberValue}]";
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => string.Format("[{0}]", MemberValue?.ToString() ?? "<null>");
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
        public Type ValueType => Dictionary.Contains(Key) ? (Dictionary[Key]?.GetType() ?? _valueType) : _valueType;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Type KeyType => Key?.GetType() ?? _keyType;

        public IDictionary Dictionary => Owner.Value as IDictionary;

        private readonly Type _valueType;
        private readonly Type _keyType;

        public PropGridItemRefIDictionaryInfo(IPropGridMemberOwner owner, object key, bool isKey) : base(owner)
        {
            Key = key;
            IsKey = isKey;
            _valueType = Dictionary?.DetermineValueType();
            _keyType = Dictionary?.DetermineKeyType();
        }

        public override bool IsReadOnly() => base.IsReadOnly() || (Dictionary?.IsReadOnly ?? false);
        internal protected override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
            => dataChangeHandler?.HandleChange(new LocalValueChangeIDictionary(oldValue, newValue, Dictionary, Key, IsKey));
        
        public override object MemberValue
        {
            get
            {
                if (IsKey)
                    return Key;
                else
                {
                    IDictionary dic = Dictionary;
                    if (dic == null)
                        return null;
                    return dic[Key];
                }
            }
            set
            {
                IDictionary dic = Dictionary;
                if (dic == null)
                    return;

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
                    if (v != null)
                    {
                        if (dic.Contains(Key))
                            dic[Key] = v;
                        else
                            dic.Add(Key, v);
                    }
                }
            }
        }
    }
}
