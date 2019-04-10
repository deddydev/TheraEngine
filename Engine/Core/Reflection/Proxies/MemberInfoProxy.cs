using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    public abstract class MemberInfoProxy : MarshalByRefObject
    {
        private MemberInfo Value { get; set; }

        //public MemberInfoProxy() { }
        protected MemberInfoProxy(MemberInfo value) => Value = value;
    }
}
