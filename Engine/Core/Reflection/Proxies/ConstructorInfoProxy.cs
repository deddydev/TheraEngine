using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    public class ConstructorInfoProxy : MethodBaseProxy
    {
        public static ConcurrentDictionary<ConstructorInfo, ConstructorInfoProxy> Proxies { get; }
            = new ConcurrentDictionary<ConstructorInfo, ConstructorInfoProxy>();
        public static ConstructorInfoProxy Get(ConstructorInfo info)
            => info == null ? null : Proxies.GetOrAdd(info, new ConstructorInfoProxy(info));
        public static implicit operator ConstructorInfoProxy(ConstructorInfo info) => Get(info);
        public static implicit operator ConstructorInfo(ConstructorInfoProxy proxy) => proxy.Value;

        private ConstructorInfo Value { get; set; }

        //public ConstructorInfoProxy() { }
        private ConstructorInfoProxy(ConstructorInfo value) : base(value) => Value = value;
    }
}
