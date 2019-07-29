using Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;

namespace TheraEngine.Core.Reflection
{
    public class AssemblyProxy : ReflectionProxy
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
        private AssemblyProxy(Assembly value) : base() => Value = value;

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

            if (other.Domain.IsGameDomain() && !Domain.IsGameDomain())
                return other.EqualTo(this);

            return Value == other.Value;
        }
        public bool EqualTo(Assembly other)
        {
            if (other is null)
                return false;

            return Value == other;
        }

        public override bool Equals(object obj) 
            => obj is AssemblyProxy proxy && EqualTo(proxy);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => "[" + Domain.FriendlyName + "] " + Value.FullName;

        public class EqualityComparer : IEqualityComparer<AssemblyProxy>
        {
            public bool Equals(AssemblyProxy x, AssemblyProxy y)
            {
                return x == y;
            }
            public int GetHashCode(AssemblyProxy x)
            {
                return x.GetHashCode();
            }
        }
    }
}
