using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using TheraEngine.Core.Reflection.Proxies;

namespace TheraEngine.Core.Reflection
{
    public class AssemblyProxy : MarshalByRefObject
    {
        public static ConcurrentDictionary<Assembly, AssemblyProxy> Proxies { get; }
            = new ConcurrentDictionary<Assembly, AssemblyProxy>();
        public static AssemblyProxy Get(Assembly assembly)
            => assembly == null ? null : Proxies.GetOrAdd(assembly, new AssemblyProxy(assembly));
        public AppDomain Domain => AppDomain.CurrentDomain;

        public static implicit operator AssemblyProxy(Assembly assembly) => Get(assembly);
        public static implicit operator Assembly(AssemblyProxy proxy) => proxy.Value;
        
        private Assembly Value { get; set; }

        //public AssemblyProxy() { }
        private AssemblyProxy(Assembly value) => Value = value;

        public bool IsDynamic => Value.IsDynamic;
        public ProxyList<TypeProxy> GetExportedTypes()
            => new ProxyList<TypeProxy>(Value.GetExportedTypes().Select(x => TypeProxy.Get(x)));
    }
}
