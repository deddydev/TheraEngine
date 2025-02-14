﻿using System.Collections.Concurrent;
using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    public class MethodBodyProxy : ReflectionProxy
    {
        public static ConcurrentDictionary<MethodBody, MethodBodyProxy> Proxies { get; }
            = new ConcurrentDictionary<MethodBody, MethodBodyProxy>();
        public static MethodBodyProxy Get(MethodBody body)
            => body is null ? null : Proxies.GetOrAdd(body, new MethodBodyProxy(body));
        public static implicit operator MethodBodyProxy(MethodBody assembly) => Get(assembly);
        public static implicit operator MethodBody(MethodBodyProxy proxy) => proxy.Value;

        private MethodBody Value { get; set; }

        //public MethodBodyProxy() { }
        protected MethodBodyProxy(MethodBody value) : base() => Value = value;
    }
}
