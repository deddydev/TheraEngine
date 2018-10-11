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
        
        public override async Task CollectSerializedMembers()
        {
            List<VarInfo> members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType);

            Children = members.
                Where(info => (info?.Attrib?.Condition == null) ? true : ExpressionParser.Evaluate<bool>(info.Attrib.Condition, TreeNode.Object)).
                Select(info => TreeNode.FormatWriter.CreateNode(TreeNode.Object == null ? null : info.GetValue(TreeNode.Object), info)).
                ToList();
            
            var categorizedChildren = Children.
                Where(x => x.MemberInfo.Category != null).
                GroupBy(x => x.MemberInfo.Category).
                ToDictionary(grp => grp.Key, grp => grp.ToArray());
            foreach (var grouping in categorizedChildren)
                foreach (MemberTreeNode p in grouping.Value)
                    Children.Remove(p);
            foreach (var cat in categorizedChildren)
            {
                MemberTreeNode node = TreeNode.FormatWriter.CreateNode(TreeNode.Object, TreeNode.MemberInfo);
                Children.Add(node);
            }
            
            foreach (MemberTreeNode member in Children)
                await CollectMemberInfo(member);

            //foreach (var group in CategorizedChildren)
            //    foreach (MemberTreeNode member in group.Value)
            //        await CollectMember(member);
        }
        private async Task CollectMemberInfo(MemberTreeNode member)
        {
            if (member.Object == null)
                return;
            if (member.MemberInfo.Attrib.State && !TreeNode.FormatWriter.Flags.HasFlag(ESerializeFlags.SerializeState))
                return;
            if (member.MemberInfo.Attrib.Config && !TreeNode.FormatWriter.Flags.HasFlag(ESerializeFlags.SerializeConfig))
                return;
            if (member?.MemberInfo?.Attrib != null)
                await TreeNode.CollectMemberInfo(member);
        }
    }
}
