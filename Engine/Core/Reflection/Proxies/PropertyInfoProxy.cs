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

        public TypeProxy PropertyType => Value.PropertyType;

        public bool CanRead => Value.CanRead;
        public bool CanWrite => Value.CanWrite;

        public object GetValue(object parentObject)
            => Value.GetValue(parentObject);
        public void SetValue(object parentObject, object memberObject)
            => Value.SetValue(parentObject, memberObject);
    }
}
