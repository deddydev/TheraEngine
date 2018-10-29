using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectWriterKind(typeof(string))]
    public class StringSerializer : BaseObjectSerializer
    {
        public override void GenerateTreeFromBinary(ref VoidPtr address)
        {

        }
        public override void GenerateTreeFromObject()
        {

        }
        public override void ReadObjectMembersFromTree()
        {
            if (TreeNode.ParseChildElementObjectMemberToObject(TreeNode.ObjectType))
            {
                TreeNode.Object = TreeNode.ChildElementObjectMember;
                TreeNode.ApplyObjectToParent();
            }
        }
        public override void WriteTreeToBinary(ref VoidPtr address)
        {

        }
    }
}
