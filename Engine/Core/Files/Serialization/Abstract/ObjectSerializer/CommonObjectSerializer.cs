using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Core.Files.Serialization
{
    //[ObjectWriterKind(typeof(object))]
    public class CommonObjectSerializer : BaseObjectSerializer
    {
        public override void TreeToObject()
        {
            var members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType).ToList();

            if (members.Count == 0 && TreeNode.GetElementContent(TreeNode.ObjectType, out object obj))
            {
                TreeNode.Object = obj;
                return;
            }

            TreeNode.Object = SerializationCommon.CreateObject(TreeNode.ObjectType);

            foreach (MethodInfo m in TreeNode.PreDeserializeMethods.OrderBy(x => x.GetCustomAttribute<PreDeserialize>().Order))
                m.Invoke(TreeNode.Object, m.GetCustomAttribute<PreDeserialize>().Arguments);

            var categorizedChildren = members.
                Where(member => member.Category != null).
                GroupBy(member => member.Category).
                ToDictionary(grp => grp.Key, grp => grp.ToArray());

            foreach (var grouping in categorizedChildren)
                foreach (TSerializeMemberInfo p in grouping.Value)
                    members.Remove(p);

            foreach (var member in members)
                ReadMember(TreeNode, member, TreeNode.Object);
            foreach (var catMember in categorizedChildren)
            {
                MemberTreeNode catNode = TreeNode.GetChildElement(catMember.Key);
                if (catNode != null)
                    foreach (var member in catMember.Value)
                        ReadMember(catNode, member, TreeNode.Object);
            }

            foreach (MethodInfo m in TreeNode.PostDeserializeMethods.OrderBy(x => x.GetCustomAttribute<PostDeserialize>().Order))
                m.Invoke(TreeNode.Object, m.GetCustomAttribute<PostDeserialize>().Arguments);
        }
        private void ReadMember(MemberTreeNode parentNode, TSerializeMemberInfo member, object o)
        {
            MemberTreeNode node = TreeNode.GetChildElement(member.Name);
            if (node != null)
            {
                node.MemberInfo = member;
                node.TreeToObject();
            }
            else
            {
                var attrib = TreeNode.GetAttribute(member.Name);
                if (attrib != null)
                {
                    if (attrib.GetObject(member.MemberType, out object value))
                    {
                        member.SetObject(o, value);
                    }
                    else
                    {
                        Engine.LogWarning("Unable to parse attribute " + attrib.Name + " as " + member.MemberType.GetFriendlyName());
                    }
                }
                else
                {
                    //Engine.PrintLine("Did not parse " + member.Name);
                }
            }
        }
        public override void TreeFromObject()
        {
            TSerializeMemberInfo[] members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType);

            if (members.Length == 0)
            {
                TreeNode.SetElementContent(TreeNode.Object);
                return;
            }

            List<MemberTreeNode> children = members.
                Where(x => x.AllowSerialize(TreeNode.Object)).
                Select(x => new MemberTreeNode(TreeNode, x)).
                ToList();
            
            //Group children by category if set
            var categorizedChildren = children.
                Where(node => node.MemberInfo.Category != null).
                GroupBy(node => node.MemberInfo.Category).
                ToDictionary(grp => grp.Key, grp => grp.ToArray());

            //Remove grouped members from original list
            foreach (var grouping in categorizedChildren)
                foreach (MemberTreeNode p in grouping.Value)
                    children.Remove(p);
            
            //Add a member node for each category
            foreach (var cat in categorizedChildren)
            {
                MemberTreeNode catNode = new MemberTreeNode(null, new TSerializeMemberInfo(null, cat.Key));
                
                foreach (MemberTreeNode catChild in cat.Value)
                    catNode.ChildElementMembers.Add(catChild);

                children.Add(catNode);
            }

            TreeNode.ChildElementMembers = new EventList<MemberTreeNode>(children);
        }
        public override void TreeFromBinary(ref VoidPtr address, TDeserializer.ReaderBinary binWriter)
        {

        }
        public override void TreeToBinary(ref VoidPtr address, TSerializer.WriterBinary binWriter)
        {

        }
        public override int OnGetTreeSize(TSerializer.WriterBinary binWriter)
        {
            return 0;
        }
    }
}
