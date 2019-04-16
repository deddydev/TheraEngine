using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Security;
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

        //
        // Summary:
        //     Indicates whether two System.Type objects are equal.
        //
        // Parameters:
        //   left:
        //     The first object to compare.
        //
        //   right:
        //     The second object to compare.
        //
        // Returns:
        //     true if left is equal to right; otherwise, false.
        [SecuritySafeCritical]
        public static bool operator ==(AssemblyProxy left, AssemblyProxy right)
            => left is null ? right is null : left.EqualTo(right);
        //
        // Summary:
        //     Indicates whether two System.Type objects are not equal.
        //
        // Parameters:
        //   left:
        //     The first object to compare.
        //
        //   right:
        //     The second object to compare.
        //
        // Returns:
        //     true if left is not equal to right; otherwise, false.
        [SecuritySafeCritical]
        public static bool operator !=(AssemblyProxy left, AssemblyProxy right)
            => left is null ? !(right is null) : !left.EqualTo(right);

        public bool EqualTo(AssemblyProxy other)
        {
            if (other is null)
                return false;

            return Value == other.Value;
        }
        public bool EqualTo(Assembly other)
        {
            if (other is null)
                return false;

            return Value == other;
        }

        public override bool Equals(object obj) 
            => obj is AssemblyProxy proxy && Value.Equals(proxy.Value);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.FullName + " [" + Domain.FriendlyName + "]";
    }
}
