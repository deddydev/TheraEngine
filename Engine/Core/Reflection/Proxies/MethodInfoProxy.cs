using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    public class MethodInfoProxy : MethodBaseProxy
    {
        public static ConcurrentDictionary<MethodInfo, MethodInfoProxy> Proxies { get; } 
            = new ConcurrentDictionary<MethodInfo, MethodInfoProxy>();
        public static MethodInfoProxy Get(MethodInfo info)
            => info == null ? null : Proxies.GetOrAdd(info, new MethodInfoProxy(info));
        public static implicit operator MethodInfoProxy(MethodInfo info) => Get(info);
        public static implicit operator MethodInfo(MethodInfoProxy proxy) => proxy.Value;

        private MethodInfo Value { get; set; }

        //public MethodInfoProxy() { }
        private MethodInfoProxy(MethodInfo value) : base(value) => Value = value;
    }
}
