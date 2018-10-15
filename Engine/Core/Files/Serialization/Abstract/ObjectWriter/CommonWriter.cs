using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    //[ObjectWriterKind(typeof(object))]
    public class CommonWriter : BaseObjectWriter
    {
        public override async Task CollectSerializedMembers()
        {
            int attribCount = 0,
                elementCount = 0,
                elementStringCount = 0;

            BindingFlags retrieveFlags =
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy;
            
            MemberInfo[] members = TreeNode.ObjectType?.GetMembersExt(retrieveFlags) ?? new MemberInfo[0];
            Members = new List<IMemberTreeNode>(members.Length);

            foreach (MemberInfo info in members)
                if ((info is FieldInfo || info is PropertyInfo) && Attribute.IsDefined(info, typeof(TSerialize)))
                {
                    TSerialize attrib = info.GetCustomAttribute<TSerialize>();
                    if (attrib.AllowSerialize(TreeNode.Object))
                    {
                        IMemberTreeNode child = TreeNode.FormatWriter.CreateNode(TreeNode, info);
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
                IMemberTreeNode node = TreeNode.FormatWriter.CreateNode(TreeNode.Object);
                node.Parent = TreeNode;
                node.NodeType = ENodeType.ChildElement;
                node.ElementName = cat.Key;

                CommonWriter objWriter = new CommonWriter { TreeNode = node };
                node.ObjectWriter = objWriter;

                foreach (IMemberTreeNode catChild in cat.Value)
                    objWriter.Members.Add(catChild);

                Members.Add(node);
            }

            await TreeNode.AddChildren(attribCount, elementCount, elementStringCount, Members);
        }
    }
}
