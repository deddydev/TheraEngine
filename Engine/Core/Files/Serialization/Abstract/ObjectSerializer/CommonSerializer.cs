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
        public override async Task ReadObjectMembersFromTreeAsync()
        {
            bool custom = await TreeNode.TryInvokeCustomDeserializeAsync();
            if (custom)
                return;

            foreach (MethodInfo m in TreeNode.PreDeserializeMethods.OrderBy(x => x.GetCustomAttribute<PreDeserialize>().Order))
                m.Invoke(TreeNode.Object, m.GetCustomAttribute<PreDeserialize>().Arguments);

            var members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType, TreeNode.Object);
            foreach (var member in members)
            {
                MemberTreeNode node = TreeNode.GetChildElement(member.Name);
                if (node != null)
                {

                }
                else
                {
                    var attrib = TreeNode.GetAttribute(member.Name);
                    if (attrib != null)
                    {
                        if (attrib.IsUnparsedString && !attrib.ParseStringToObject(member.MemberType))
                        {
                            Engine.LogWarning("Unable to parse attribute " + attrib.Name + " as " + member.MemberType.GetFriendlyName());
                        }
                    }
                    else
                    {

                    }
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
            TSerializeMemberInfo[] members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType, TreeNode.Object);
            Members = members.Select(x => new MemberTreeNode(TreeNode, x)).ToList();
            
            //Group children by category if set
            var categorizedChildren = Members.
                Where(node => node.MemberInfo.Category != null).
                GroupBy(node => node.MemberInfo.Category).
                ToDictionary(grp => grp.Key, grp => grp.ToArray());

            //Remove grouped members from original list
            foreach (var grouping in categorizedChildren)
                foreach (MemberTreeNode p in grouping.Value)
                    Members.Remove(p);
            
            //Add a member node for each category
            foreach (var cat in categorizedChildren)
            {
                //TODO: handle category tree nodes better

                MemberTreeNode node = new MemberTreeNode(TreeNode, new TSerializeMemberInfo(null, cat.Key));

                CommonSerializer objWriter = new CommonSerializer { TreeNode = node };
                node.ObjectSerializer = objWriter;

                foreach (MemberTreeNode catChild in cat.Value)
                    objWriter.Members.Add(catChild);

                Members.Add(node);
            }
            TreeNode.ChildElements = Members;
        }
    }
}
