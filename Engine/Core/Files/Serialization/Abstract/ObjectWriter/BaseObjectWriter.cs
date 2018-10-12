using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    }
}
