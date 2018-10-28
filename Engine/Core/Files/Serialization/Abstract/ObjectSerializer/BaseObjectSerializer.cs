using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;

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
    public abstract class BaseObjectSerializer
    {
        public MemberTreeNode TreeNode { get; internal protected set; } = null;
        public List<MemberTreeNode> Members { get; set; }

        public abstract void GenerateTreeFromBinary(ref VoidPtr address);
        public abstract Task ReadObjectMembersFromTreeAsync();
        public abstract Task GenerateTreeFromObject();
    }
}
