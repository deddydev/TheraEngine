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
        public IMemberTreeNode[] Values { get; private set; }

        public override void GenerateObjectFromTree()
        {

        }
        public override void GenerateTreeFromBinary(ref VoidPtr address)
        {

        }
        public override async Task GenerateTreeFromObject()
        {
            List = TreeNode.Object as IList;

            Type elemType = List.DetermineElementType();
            Type objType = TreeNode.ObjectType;

            //ESerializeType elemSerType = TreeNode.GetSerializeType(elemType);

            object[] vals = new object[List.Count];
            List.CopyTo(vals, 0);
            Values = vals.Select(obj => 
            {
                IMemberTreeNode node = TreeNode.Owner.CreateNode(obj);
                node.MemberType = elemType;
                node.ElementName = SerializationCommon.GetTypeName(node.MemberType);
                node.Parent = TreeNode;
                node.NodeType = System.ComponentModel.ENodeType.ChildElement;
                return node;
            }).ToArray();

            //Values = new MemberTreeNode[List.Count];
            //for (int i = 0; i < List.Count; ++i)
            //    Values[i] = TreeNode.FormatWriter.CreateNode(List[i], new VarInfo(List[i]?.GetType() ?? listType, objType));

            await TreeNode.AddChildrenAsync(0, Members.Count, 0, Members);
        }
    }
}
