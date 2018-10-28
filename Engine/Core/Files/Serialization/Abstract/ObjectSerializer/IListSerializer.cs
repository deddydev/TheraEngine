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
        public IList List { get; private set; }
        
        public override void GenerateTreeFromBinary(ref VoidPtr address)
        {

        }
        public override async Task GenerateTreeFromObject()
        {
            List = TreeNode.Object as IList;

            Type elemType = List.DetermineElementType();
            
            foreach (object o in List)
                TreeNode.ChildElementMembers.Add(new MemberTreeNode(o, new TSerializeMemberInfo(elemType, null)));
        }

        public override Task ReadObjectMembersFromTreeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
