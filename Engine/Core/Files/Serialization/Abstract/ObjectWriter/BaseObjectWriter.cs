using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    public class ObjectWriterKind : Attribute
    {
        /// <summary>
        /// The type this writer will be collecting members for.
        /// </summary>
        public Type ObjectType { get; }
        public ObjectWriterKind(Type objectType) => ObjectType = objectType;
    }
    /// <summary>
    /// Tool to collect all members of an object into an array of children.
    /// </summary>
    public abstract class BaseObjectWriter
    {
        public MemberTreeNode TreeNode { get; internal set; } = null;
        public List<MemberTreeNode> Members { get; set; }
        public abstract Task CollectSerializedMembers();
    }
}
