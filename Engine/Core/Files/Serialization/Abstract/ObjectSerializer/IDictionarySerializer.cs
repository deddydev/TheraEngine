using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Tools;

namespace TheraEngine.Core.Files.Serialization
{
    [ObjectWriterKind(typeof(IDictionary))]
    public class IDictionarySerializer : BaseObjectSerializer
    {
        public IDictionary Dictionary { get; private set; }
        public IMemberTreeNode[] Keys { get; private set; }
        public IMemberTreeNode[] Values { get; private set; }

        public override void GenerateObjectFromTree()
        {

        }
        public override void GenerateTreeFromBinary(ref VoidPtr address)
        {

        }
        public override async Task GenerateTreeFromObject()
        {
            Dictionary = TreeNode.Object as IDictionary;

            object[] keys = new object[Dictionary.Keys.Count];
            object[] vals = new object[Dictionary.Values.Count];

            Type keyType = Dictionary.DetermineKeyType();
            Type valType = Dictionary.DetermineValueType();
            Type objType = TreeNode.ObjectType;

            Dictionary.Keys.CopyTo(keys, 0);
            Dictionary.Values.CopyTo(vals, 0);

            //ESerializeType keySerType = TreeNode.GetSerializeType(keyType);
            //ESerializeType valSerType = TreeNode.GetSerializeType(valType);

            Keys = keys.Select(obj =>
            {
                IMemberTreeNode node = TreeNode.Owner.CreateNode(obj);
                node.MemberType = keyType;
                node.ElementName = SerializationCommon.GetTypeName(node.MemberType);
                node.NodeType = ENodeType.ChildElement;
                return node;
            }).ToArray();

            Values = vals.Select(obj =>
            {
                IMemberTreeNode node = TreeNode.Owner.CreateNode(obj);
                node.MemberType = valType;
                node.ElementName = SerializationCommon.GetTypeName(node.MemberType);
                node.NodeType = ENodeType.ChildElement;
                return node;
            }).ToArray();

            Members = new List<IMemberTreeNode>(Keys.Length);
            for (int i = 0; i < Keys.Length; ++i)
            {
                IMemberTreeNode pairNode = TreeNode.Owner.CreateNode(TreeNode, null);
                await pairNode.AddChildrenAsync(0, 2, 0, new List<IMemberTreeNode>(2) { Keys[i], Values[i] });
                Members.Add(pairNode);
            }

            await TreeNode.AddChildrenAsync(0, Members.Count, 0, Members);
        }
    }
}
