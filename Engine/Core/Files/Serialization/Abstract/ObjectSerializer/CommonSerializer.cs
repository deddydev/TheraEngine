using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Core.Files.Serialization
{
    //[ObjectWriterKind(typeof(object))]
    public class CommonSerializer : BaseObjectSerializer
    {
        public override void GenerateObjectFromTree()
        {
            foreach (MethodInfo m in TreeNode.PreDeserializeMethods.OrderBy(x => x.GetCustomAttribute<PreDeserialize>().Order))
                m.Invoke(TreeNode.Object, m.GetCustomAttribute<PreDeserialize>().Arguments);

            List<IMemberTreeNode> nodes = TreeNode.GetChildNodes();
            foreach (IMemberTreeNode node in nodes)
            {
                switch (node.NodeType)
                {
                    case ENodeType.Attribute:
                        int index = attribs.FindIndex(attrib => string.Equals(node.Name, attrib.Name, StringComparison.InvariantCultureIgnoreCase));
                        if (index >= 0)
                        {
                            var attrib = attribs[index];
                            attribs.RemoveAt(index);

                        }
                        break;
                    case ENodeType.ElementString:
                        break;
                    case ENodeType.ChildElement:
                        break;
                }
            }

            foreach (MethodInfo m in TreeNode.PostDeserializeMethods.OrderBy(x => x.GetCustomAttribute<PostDeserialize>().Order))
                m.Invoke(TreeNode.Object, m.GetCustomAttribute<PostDeserialize>().Arguments);

        }
        public override void GenerateTreeFromBinary(ref VoidPtr address)
        {

        }
        public override async Task GenerateTreeFromObject()
        {
            int attribCount = 0,
                elementCount = 0,
                elementStringCount = 0;

            List<TSerializeMemberInfo> members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType);
            Members = new List<IMemberTreeNode>(members.Count);

            foreach (TSerializeMemberInfo info in members)
            {
                TSerialize attrib = info.Attribute;
                if (attrib.AllowSerialize(TreeNode.Object))
                {
                    IMemberTreeNode child = TreeNode.Owner.CreateNode(TreeNode, info);
                    switch (attrib.NodeType)
                    {
                        case ENodeType.Attribute:
                            ++attribCount;
                            break;
                        case ENodeType.ChildElement:
                            ++elementCount;
                            break;
                        case ENodeType.ElementString:
                            ++elementStringCount;
                            break;
                    }
                    Members.Add(child);
                }
            }
                
            for (int i = 0; i < Members.Count; ++i)
            {
                IMemberTreeNode node = Members[i];
                int index = node.Order;
                if (index >= 0)
                {
                    if (i < attribCount)
                        index = index.Clamp(0, attribCount - 1);
                    else
                        index = index.Clamp(0, elementCount - 1) + attribCount;

                    if (index == i)
                        continue;

                    Members.RemoveAt(i--);

                    if (index == Members.Count)
                        Members.Add(node);
                    else
                        Members.Insert(index, node);
                }
            }
            
            //Group children by category if set
            var categorizedChildren = Members.
                Where(x => x.Category != null).
                GroupBy(x => x.Category).
                ToDictionary(grp => grp.Key, grp => grp.ToArray());

            //Remove grouped members from original list
            foreach (var grouping in categorizedChildren)
                foreach (IMemberTreeNode p in grouping.Value)
                    Members.Remove(p);
            
            //Add a member node for each category
            foreach (var cat in categorizedChildren)
            {
                //TODO: handle category tree nodes better

                IMemberTreeNode node = TreeNode.Owner.CreateNode(TreeNode.Object);
                node.Parent = TreeNode;
                node.NodeType = ENodeType.ChildElement;
                node.ElementName = cat.Key;

                CommonSerializer objWriter = new CommonSerializer { TreeNode = node };
                node.ObjectSerializer = objWriter;

                foreach (IMemberTreeNode catChild in cat.Value)
                    objWriter.Members.Add(catChild);

                Members.Add(node);
            }

            await TreeNode.AddChildNodesAsync(attribCount, elementCount, elementStringCount, Members);
        }
    }
}
