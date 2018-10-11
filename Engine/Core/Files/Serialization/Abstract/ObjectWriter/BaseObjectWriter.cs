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
        public abstract Task CollectSerializedMembers();
    }
}
