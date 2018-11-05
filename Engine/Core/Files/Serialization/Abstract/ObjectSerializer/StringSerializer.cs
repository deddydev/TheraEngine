using System;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectWriterKind(typeof(string))]
    public class StringSerializer : BaseObjectSerializer
    {
        public override void TreeFromObject()
        {
            TreeNode.SetElementContent(TreeNode.Object as string);
        }
        public override void TreeToObject()
        {
            if (TreeNode.GetElementContentAs(out string value))
                TreeNode.Object = value;
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
