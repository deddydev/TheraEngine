using System;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectWriterKind(typeof(string))]
    public class StringSerializer : BaseObjectSerializer
    {
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
        public override void TreeToBinary(ref VoidPtr address, TSerializer.WriterBinary binWriter)
        {

        }
        public override int OnGetTreeSize(TSerializer.WriterBinary binWriter)
        {
            throw new NotImplementedException();
        }
        public override void TreeFromBinary(ref VoidPtr address, TDeserializer.ReaderBinary binReader)
        {
            throw new NotImplementedException();
        }
    }
}
