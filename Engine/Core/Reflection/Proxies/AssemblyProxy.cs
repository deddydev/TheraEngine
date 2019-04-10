using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    public class AssemblyProxy : MarshalByRefObject
    {
        public static ConcurrentDictionary<Assembly, AssemblyProxy> Proxies { get; }
            = new ConcurrentDictionary<Assembly, AssemblyProxy>();
        public static AssemblyProxy Get(Assembly assembly)
            => assembly == null ? null : Proxies.GetOrAdd(assembly, new AssemblyProxy(assembly));
        public static implicit operator AssemblyProxy(Assembly assembly) => Get(assembly);
        public static implicit operator Assembly(AssemblyProxy proxy) => proxy.Value;
        
        private Assembly Value { get; set; }
        
        //public AssemblyProxy() { }
        private AssemblyProxy(Assembly value) => Value = value;
    }
}
