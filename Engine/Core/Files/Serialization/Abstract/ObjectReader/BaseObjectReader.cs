using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    public class ObjectReaderKind : Attribute
    {
        /// <summary>
        /// The type this reader will be reading members for.
        /// </summary>
        public Type ObjectType { get; }
        public ObjectReaderKind(Type objectType) => ObjectType = objectType;
    }
    /// <summary>
    /// Tool to collect all members of an object into an array of children.
    /// </summary>
    public abstract class BaseObjectReader
    {
        public IMemberTreeNode TreeNode { get; internal set; } = null;
        public List<IMemberTreeNode> Members { get; set; }
        public abstract Task CollectSerializedMembers();
    }
}
