using Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Core.Reflection
{
    public class MethodInfoProxy : MethodBaseProxy
    {
        public static ConcurrentDictionary<MethodInfo, MethodInfoProxy> Proxies { get; } 
            = new ConcurrentDictionary<MethodInfo, MethodInfoProxy>();
        public static MethodInfoProxy Get(MethodInfo info)
            => info is null ? null : Proxies.GetOrAdd(info, new MethodInfoProxy(info));
        public static implicit operator MethodInfoProxy(MethodInfo info) => Get(info);
        public static explicit operator MethodInfo(MethodInfoProxy proxy) => proxy.Value;

        private MethodInfo Value { get; set; }

        //public MethodInfoProxy() { }
        private MethodInfoProxy(MethodInfo value) : base(value) => Value = value;

        public TypeProxy ReturnType => Value.ReturnType;

        public string GetFriendlyName(bool nameOnly = false, string openBracket = "<", string closeBracket = ">")
            => Value.GetFriendlyName(nameOnly, openBracket, closeBracket);

        public MethodInfoProxy MakeGenericMethod(TypeProxy[] selectedTypes)
            => Value.MakeGenericMethod(selectedTypes.Select(x => (Type)x).ToArray());
        public MethodInfoProxy GetGenericMethodDefinition()
            => Value.GetGenericMethodDefinition();

        public bool CanRunForFormat(EProprietaryFileFormatFlag format, out SerializeElement.ESerializeMethodType type)
        {
            var attribs = GetCustomAttributes<SerializationAttribute>();
            foreach (SerializationAttribute attrib in attribs)
            {
                if (attrib.RunForFormats.HasFlag(format))
                {
                    switch (attrib)
                    {
                        case TPreDeserialize _:
                            type = SerializeElement.ESerializeMethodType.PreDeserialize;
                            break;
                        case TPostDeserialize _:
                            type = SerializeElement.ESerializeMethodType.PostDeserialize;
                            break;
                        case TPreSerialize _:
                            type = SerializeElement.ESerializeMethodType.PreSerialize;
                            break;
                        case TPostSerialize _:
                            type = SerializeElement.ESerializeMethodType.PostSerialize;
                            break;
                        case CustomMemberSerializeMethod _:
                            type = SerializeElement.ESerializeMethodType.CustomSerialize;
                            break;
                        default:
                        case CustomMemberDeserializeMethod _:
                            type = SerializeElement.ESerializeMethodType.CustomDeserialize;
                            break;
                    }
                    return true;
                }
            }
            type = SerializeElement.ESerializeMethodType.CustomDeserialize;
            return false;
        }
    }
}
