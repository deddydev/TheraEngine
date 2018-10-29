using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectWriterKind(typeof(IList))]
    public class IListSerializer : BaseObjectSerializer
    {
        public override void GenerateTreeFromBinary(ref VoidPtr address)
        {

        }
        public override void GenerateTreeFromObject()
        {
            IList list = TreeNode.Object as IList;

            Type elemType = list.DetermineElementType();
            
            foreach (object o in list)
                TreeNode.ChildElementMembers.Add(new MemberTreeNode(o, new TSerializeMemberInfo(elemType, null)));
        }

        public override void ReadObjectMembersFromTree()
        {

        }
        public override void WriteTreeToBinary(ref VoidPtr address) => throw new NotImplementedException();
    }
}
