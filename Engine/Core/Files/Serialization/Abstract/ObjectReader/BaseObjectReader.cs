using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    /// <summary>
    /// Tool to collect all members of an object into an array of children.
    /// </summary>
    public abstract class BaseObjectWriter
    {
        public IMemberTreeNode TreeNode { get; internal set; } = null;
        public List<IMemberTreeNode> Members { get; set; }
        public abstract Task CollectSerializedMembers();
    }
}
