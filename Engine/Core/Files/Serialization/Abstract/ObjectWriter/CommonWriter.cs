using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Tools;
using static TheraEngine.Core.Files.Serialization.TSerializer.WriterBinary;

namespace TheraEngine.Core.Files.Serialization
{
    public class CommonWriter : BaseObjectWriter
    {
        public List<MemberTreeNode> Children { get; private set; }
        //public Dictionary<string, MemberTreeNode[]> CategorizedChildren { get; private set; }
        private int NonAttributeCount { get; set; }
        
        public override async Task GenerateTree()
        {
            List<VarInfo> members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType);

            Children = members.
                Where(x => (x.Attrib == null || x.Attrib.Condition == null) ? true : ExpressionParser.Evaluate<bool>(x.Attrib.Condition, TreeNode.Object)).
                Select(x => new MemberTreeNode(TreeNode.Object == null ? null : x.GetValue(TreeNode.Object), x, TreeNode.FormatWriter)).
                ToList();
            
            var categorizedChildren = Children.
                Where(x => x.MemberInfo.Category != null).
                GroupBy(x => x.MemberInfo.Category).
                ToDictionary(grp => grp.Key, grp => grp.ToArray());
            foreach (var grouping in categorizedChildren)
                foreach (MemberTreeNode p in grouping.Value)
                    Children.Remove(p);
            foreach (var cat in categorizedChildren)
                Children.Add(new MemberTreeNode(TreeNode.Object, TreeNode.MemberInfo, TreeNode.FormatWriter));
            
            int childElementCount = Children.Where(x => !x.MemberInfo.Attrib.IsXmlAttribute && x.Object != null).Count();
            NonAttributeCount = childElementCount + categorizedChildren.Count;
            Attributes = new List<(string, object)>();
            ChildElements = new List<MemberTreeNode>();
            
            foreach (MemberTreeNode member in Children)
                await CollectMember(member);

            //foreach (var group in CategorizedChildren)
            //    foreach (MemberTreeNode member in group.Value)
            //        await CollectMember(member);
        }
        private async Task CollectMember(MemberTreeNode member)
        {
            if (member.Object == null)
                return;
            if (member.MemberInfo.Attrib.State && !TreeNode.FormatWriter.Flags.HasFlag(ESerializeFlags.SerializeState))
                return;
            if (member.MemberInfo.Attrib.Config && !TreeNode.FormatWriter.Flags.HasFlag(ESerializeFlags.SerializeConfig))
                return;

            Type valueType = member.ObjectType;
            if (member?.MemberInfo?.Attrib != null)
            {
                MethodInfo customMethod = TreeNode.CustomMethods.FirstOrDefault(
                    x => string.Equals(member.MemberInfo.Name, x.GetCustomAttribute<CustomSerializeMethod>().Name));

                if (customMethod != null)
                {
                    await (Task)customMethod.Invoke(TreeNode.Object, new object[] { _writer, _flags });
                    return;
                }

                if (member.MemberInfo.Attrib.IsXmlElementString)
                {
                    if (TreeNode.FormatWriter.ParseElementObject(member, out object result))
                    {
                        if (NonAttributeCount == 1)
                            SingleSerializableChildData = result;
                        else
                            Attributes.Add((member.MemberInfo.Name, result));
                        return;
                    }
                }
                else if (member.MemberInfo.Attrib.IsXmlAttribute)
                {
                    if (SerializationCommon.GetString(member.Object, member.MemberInfo.VariableType, out string result))
                    {
                        Attributes.Add((member.MemberInfo.Name, result));
                        return;
                    }
                }
            }

            ChildElements.Add(member);
            await member.GenerateTree();
        }
    }
}
