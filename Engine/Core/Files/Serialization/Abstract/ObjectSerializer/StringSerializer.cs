using System;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectWriterKind(typeof(string))]
    public class StringSerializer : BaseObjectSerializer
    {
        public override void TreeFromBinary(ref VoidPtr address)
        {

        }
        public override void TreeFromObject()
        {
            TreeNode.ElementContent = TreeNode.Object as string;
        }
        public override void TreeToObject()
        {
            if (TreeNode.ParseChildElementObjectMemberToObject(TreeNode.ObjectType))
            {
                TreeNode.Object = TreeNode.ElementContent;
                TreeNode.ApplyObjectToParent();
            }
            else
                TreeNode.Object = null;
        }
        public override void TreeToBinary(ref VoidPtr address)
        {

        }
        public override int OnGetTreeSize()
        {
            var binWriter = GetBinaryWriter();
        }
    }
}
