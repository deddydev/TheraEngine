using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    public class PropertyInfoProxy : MemberInfoProxy
    {
        public static ConcurrentDictionary<PropertyInfo, PropertyInfoProxy> Proxies { get; }
            = new ConcurrentDictionary<PropertyInfo, PropertyInfoProxy>();
        public static PropertyInfoProxy Get(PropertyInfo info)
            => info == null ? null : Proxies.GetOrAdd(info, new PropertyInfoProxy(info));
        public static implicit operator PropertyInfoProxy(PropertyInfo info) => Get(info);
        public static implicit operator PropertyInfo(PropertyInfoProxy proxy) => proxy.Value;

        private PropertyInfo Value { get; set; }

        //public PropertyInfoProxy() { }
        private PropertyInfoProxy(PropertyInfo value) : base(value) => Value = value;
    }
}
