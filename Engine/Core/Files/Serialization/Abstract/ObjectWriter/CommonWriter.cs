using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TheraEngine.Core.Tools;

namespace TheraEngine.Core.Files.Serialization
{
    public class CommonObjectWriter : BaseObjectWriter
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

            MemberInfo[] members = TreeNode.ObjectType.GetMembersExt(retrieveFlags);
            Members = new List<MemberTreeNode>(members.Length);

            foreach (MemberInfo info in members)
            {
                if (info is FieldInfo || info is PropertyInfo)
                {
                    if (Attribute.IsDefined(info, typeof(TSerialize)))
                    {
                        TSerialize attrib = info.GetCustomAttribute<TSerialize>();
                        if (attrib.AllowSerialize(TreeNode.Object))
                        {
                            MemberTreeNode child = TreeNode.FormatWriter.CreateNode(TreeNode, info);
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
                }
            }
            
            for (int i = 0; i < Members.Count; ++i)
            {
                MemberTreeNode node = Members[i];
                TSerialize s = node.Attrib;
                if (s.Order >= 0)
                {
                    int index = s.Order;

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
                foreach (MemberTreeNode p in grouping.Value)
                    Members.Remove(p);
            
            //Add a member node for each category
            foreach (var cat in categorizedChildren)
            {
                MemberTreeNode node = TreeNode.FormatWriter.CreateNode(TreeNode.Object);
                node.Parent = TreeNode;
                node.NodeType = ENodeType.ChildElement;
                node.ElementName = cat.Key;

                CommonObjectWriter objWriter = new CommonObjectWriter { TreeNode = node };
                node.ObjectWriter = objWriter;

                foreach (MemberTreeNode catChild in cat.Value)
                    objWriter.Members.Add(catChild);

                Members.Add(node);
            }

            await TreeNode.AddChildren(attribCount, elementCount, elementStringCount, Members);
        }
    }
}
