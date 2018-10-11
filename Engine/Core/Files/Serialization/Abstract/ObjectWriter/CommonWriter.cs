using System;
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
            //Collect all members that need to be written
            //List<VarInfo> members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType);

            BindingFlags retrieveFlags =
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy;

            MemberInfo[] members = TreeNode.ObjectType.GetMembersExt(retrieveFlags);
            List<MemberTreeNode> fields = members.
                Where(x => (x is FieldInfo || x is PropertyInfo) && Attribute.IsDefined(x, typeof(TSerialize))).
                Select(x => TreeNode.FormatWriter.CreateNode(x)).
                OrderBy(x => (int)x.Attrib.XmlNodeType).ToList();

            int attribCount = 0, elementCount = 0, elementStringCount = 0;
            foreach (MemberTreeNode info in fields)
            {
                switch (info.Attrib.XmlNodeType)
                {
                    case EXmlNodeType.Attribute:
                        ++attribCount;
                        break;
                    case EXmlNodeType.ChildElement:
                        ++elementCount;
                        break;
                    case EXmlNodeType.ElementString:
                        ++elementStringCount;
                        break;
                }
            }

            for (int i = 0; i < fields.Count; ++i)
            {
                MemberTreeNode info = fields[i];
                TSerialize s = info.Attrib;
                if (s.Order >= 0)
                {
                    int index = s.Order;

                    if (i < attribCount)
                        index = index.Clamp(0, attribCount - 1);
                    else
                        index = index.Clamp(0, elementCount - 1) + attribCount;

                    if (index == i)
                        continue;
                    fields.RemoveAt(i--);
                    if (index == fields.Count)
                        fields.Add(info);
                    else
                        fields.Insert(index, info);
                }
            }

            Members = members.
                Where(info => (info?.Attrib?.Condition == null) ? true : ExpressionParser.Evaluate<bool>(info.Attrib.Condition, TreeNode.Object)).
                Select(info => TreeNode.FormatWriter.CreateNode(TreeNode.Object == null ? null : info.GetValue(TreeNode.Object), info)).
                ToList();
            
            //Group children by category if set
            var categorizedChildren = Members.
                Where(x => x.MemberInfo.Category != null).
                GroupBy(x => x.MemberInfo.Category).
                ToDictionary(grp => grp.Key, grp => grp.ToArray());

            //Remove grouped members from original list
            foreach (var grouping in categorizedChildren)
                foreach (MemberTreeNode p in grouping.Value)
                    Members.Remove(p);

            //Add a member node for each category
            foreach (var cat in categorizedChildren)
            {
                MemberTreeNode node = TreeNode.FormatWriter.CreateNode(TreeNode.Object, TreeNode.MemberInfo);
                node.IsXmlAttribute = false;
                node.IsXmlElementString = false;
                node.ElementName = cat.Key;

                CommonObjectWriter objWriter = new CommonObjectWriter { TreeNode = node };
                node.ObjectWriter = objWriter;

                foreach (MemberTreeNode catChild in cat.Value)
                    objWriter.Members.Add(catChild);

                Members.Add(node);
            }

            NonAttributeCount = Members.Where(x => !x.IsXmlAttribute && x.GetValue(TreeNode.Object) != null).Count();
            foreach (MemberTreeNode member in Members)
                await TreeNode.CollectMemberInfo(member);
        }
    }
}
