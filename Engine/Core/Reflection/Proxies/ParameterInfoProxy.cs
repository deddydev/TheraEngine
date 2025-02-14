﻿using System.Collections.Concurrent;
using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    public class ParameterInfoProxy : ReflectionProxy
    {
        public static ConcurrentDictionary<ParameterInfo, ParameterInfoProxy> Proxies { get; }
            = new ConcurrentDictionary<ParameterInfo, ParameterInfoProxy>();
        public static ParameterInfoProxy Get(ParameterInfo info)
            => info is null ? null : Proxies.GetOrAdd(info, new ParameterInfoProxy(info));
        public static implicit operator ParameterInfoProxy(ParameterInfo info) => Get(info);
        public static implicit operator ParameterInfo(ParameterInfoProxy proxy) => proxy.Value;

        private ParameterInfo Value { get; set; }

        //public ParameterInfoProxy() { }
        private ParameterInfoProxy(ParameterInfo value) : base() => Value = value;

        public TypeProxy ParameterType => Value.ParameterType;
    }
}
