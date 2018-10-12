using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    public class ObjectWriterKind : Attribute
    {
        public Type ObjectType { get; }
        public ObjectWriterKind(Type objectType) => ObjectType = objectType;
    }
    public abstract class BaseObjectWriter
    {
        public MemberTreeNode TreeNode { get; internal set; } = null;
        public List<MemberTreeNode> Members { get; set; }
        public abstract Task CollectSerializedMembers();

        public static SerializationCommon.ESerializeType GetSerializeType(Type t)
        {
            if (t.IsSubclassOf(typeof(TFileObject)) && (TFileObject.GetFileExtension(t)?.ManualXmlConfigSerialize == true))
            {
                return SerializeType.Manual;
            }
            else if (t.GetInterface(nameof(IParsable)) != null)
            {
                return SerializeType.Parsable;
            }
            else if (t.IsEnum)
            {
                return SerializeType.Enum;
            }
            else if (t == typeof(string))
            {
                return SerializeType.String;
            }
            else if (t.IsValueType)
            {
                return SerializeType.Struct;
            }
            else
            {
                return SerializeType.Pointer;
            }
        }
    }
}
