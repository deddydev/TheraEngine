using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectWriterKind(typeof(IList))]
    public class IListWriter : BaseObjectWriter
    {
        public IList List { get; private set; }
        public MemberTreeNode[] Values { get; private set; }
        
        public override async Task CollectSerializedMembers()
        {
            List = TreeNode.Object as IList;

            Type elemType = List.DetermineElementType();
            Type objType = TreeNode.ObjectType;

            //ESerializeType elemSerType = TreeNode.GetSerializeType(elemType);

            object[] vals = new object[List.Count];
            List.CopyTo(vals, 0);
            Values = vals.Select(obj => 
            {
                MemberTreeNode node = TreeNode.FormatWriter.CreateNode(obj);
                node.MemberType = elemType;
                node.ElementName = SerializationCommon.GetTypeName(node.MemberType);
                node.Parent = TreeNode;
                node.NodeType = System.ComponentModel.ENodeType.ChildElement;
                return node;
            }).ToArray();

            //Values = new MemberTreeNode[List.Count];
            //for (int i = 0; i < List.Count; ++i)
            //    Values[i] = TreeNode.FormatWriter.CreateNode(List[i], new VarInfo(List[i]?.GetType() ?? listType, objType));

            await TreeNode.AddChildren(0, Members.Count, 0, Members);
        }
    }
}
