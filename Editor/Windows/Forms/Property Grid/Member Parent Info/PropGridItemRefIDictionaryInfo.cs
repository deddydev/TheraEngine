using Extensions;
using System.Collections;
using System.ComponentModel;
using TheraEngine.Core.Reflection;
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
        public override TypeProxy DataType => MemberValue?.GetTypeProxy();
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public object Key { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public bool IsKey { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public TypeProxy ValueType => Dictionary.Contains(Key) ? (Dictionary[Key]?.GetTypeProxy() ?? _valueType) : _valueType;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public TypeProxy KeyType => Key?.GetTypeProxy() ?? _keyType;

        public IDictionary Dictionary => Owner.Value as IDictionary;

        private readonly TypeProxy _valueType;
        private readonly TypeProxy _keyType;

        public PropGridItemRefIDictionaryInfo(IPropGridMemberOwner owner, object key, bool isKey) : base(owner)
        {
            Key = key;
            IsKey = isKey;
            _valueType = Dictionary?.DetermineValueTypeProxy();
            _keyType = Dictionary?.DetermineKeyTypeProxy();
        }

        public override bool IsReadOnly() => base.IsReadOnly() || (Dictionary?.IsReadOnly ?? false);
        internal protected override void SubmitStateChange(object oldValue, object newValue, ValueChangeHandler dataChangeHandler)
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
