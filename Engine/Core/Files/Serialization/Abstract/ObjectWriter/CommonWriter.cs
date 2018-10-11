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
    public class CommonObjectWriter : BaseObjectWriter
    {
        public List<MemberTreeNode> Children { get; private set; }

        public override async Task CollectSerializedMembers()
        {
            //Collect all members that need to be written
            List<VarInfo> members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType);
            Children = members.
                Where(info => (info?.Attrib?.Condition == null) ? true : ExpressionParser.Evaluate<bool>(info.Attrib.Condition, TreeNode.Object)).
                Select(info => TreeNode.FormatWriter.CreateNode(TreeNode.Object == null ? null : info.GetValue(TreeNode.Object), info)).
                ToList();
            
            //Group children by category if set
            var categorizedChildren = Children.
                Where(x => x.MemberInfo.Category != null).
                GroupBy(x => x.MemberInfo.Category).
                ToDictionary(grp => grp.Key, grp => grp.ToArray());

            //Remove grouped members from original list
            foreach (var grouping in categorizedChildren)
                foreach (MemberTreeNode p in grouping.Value)
                    Children.Remove(p);

            //Add a member node for each category
            foreach (var cat in categorizedChildren)
            {
                MemberTreeNode node = TreeNode.FormatWriter.CreateNode(TreeNode.Object, TreeNode.MemberInfo);

                CommonObjectWriter objWriter = new CommonObjectWriter { TreeNode = node };
                node.ObjectWriter = objWriter;

                foreach (MemberTreeNode catChild in cat.Value)
                    objWriter.Children.Add(catChild);

                Children.Add(node);
            }
            
            foreach (MemberTreeNode member in Children)
                await TreeNode.CollectMemberInfo(member);
        }
    }
}
