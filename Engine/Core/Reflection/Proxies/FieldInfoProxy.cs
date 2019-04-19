using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    public class FieldInfoProxy : MemberInfoProxy
    {
        public static ConcurrentDictionary<FieldInfo, FieldInfoProxy> Proxies { get; }
            = new ConcurrentDictionary<FieldInfo, FieldInfoProxy>();
        public static FieldInfoProxy Get(FieldInfo info)
            => info == null ? null : Proxies.GetOrAdd(info, new FieldInfoProxy(info));
        public static implicit operator FieldInfoProxy(FieldInfo info) => Get(info);
        public static implicit operator FieldInfo(FieldInfoProxy proxy) => proxy.Value;

        private FieldInfo Value { get; set; }

        //public FieldInfoProxy() { }
        private FieldInfoProxy(FieldInfo value) : base(value) => Value = value;

        public TypeProxy FieldType => Value.FieldType;

        public object GetValue(object parentObject)
            => Value.GetValue(parentObject);
        public void SetValue(object parentObject, object memberObject)
            => Value.SetValue(parentObject, memberObject);
        public object GetRawConstantValue() 
            => Value.GetRawConstantValue();
    }
}
