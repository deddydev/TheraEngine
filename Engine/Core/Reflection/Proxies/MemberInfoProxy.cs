using System;
using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    public abstract class MemberInfoProxy : MarshalByRefObject
    {
        public static MemberInfoProxy Get(MemberInfo info)
        {
            switch (info)
            {
                case FieldInfo fieldInfo:
                    return FieldInfoProxy.Get(fieldInfo);
                case PropertyInfo propertyInfo:
                    return PropertyInfoProxy.Get(propertyInfo);
                case EventInfo eventInfo:
                    return EventInfoProxy.Get(eventInfo);
                default:
                    return null;
            }
        }

        private MemberInfo Value { get; set; }

        //public MemberInfoProxy() { }
        protected MemberInfoProxy(MemberInfo value) => Value = value;
    }
}
