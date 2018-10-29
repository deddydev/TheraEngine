﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Core.Files.Serialization
{
    //[ObjectWriterKind(typeof(object))]
    public class CommonSerializer : BaseObjectSerializer
    {
        public override void ReadObjectMembersFromTree()
        {
            foreach (MethodInfo m in TreeNode.PreDeserializeMethods.OrderBy(x => x.GetCustomAttribute<PreDeserialize>().Order))
                m.Invoke(TreeNode.Object, m.GetCustomAttribute<PreDeserialize>().Arguments);

            var members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType, TreeNode.Object);
            foreach (var member in members)
            {
                MemberTreeNode node = TreeNode.GetChildElement(member.Name);
                if (node != null)
                {
                    node.MemberInfo = member;
                    node.GenerateObjectFromTree();
                }
                else
                {
                    var attrib = TreeNode.GetAttribute(member.Name);
                    if (attrib != null)
                    {
                        if (attrib.GetObject(member.MemberType, out object value))
                        {
                            member.SetObject(TreeNode.Object, value);
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

            foreach (MethodInfo m in TreeNode.PostDeserializeMethods.OrderBy(x => x.GetCustomAttribute<PostDeserialize>().Order))
                m.Invoke(TreeNode.Object, m.GetCustomAttribute<PostDeserialize>().Arguments);

        }
        public override void GenerateTreeFromObject()
        {
            TSerializeMemberInfo[] members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType, TreeNode.Object);
            List<MemberTreeNode> memberNodes = members.Select(x => new MemberTreeNode(TreeNode, x)).ToList();
            
            //Group children by category if set
            var categorizedChildren = memberNodes.
                Where(node => node.MemberInfo.Category != null).
                GroupBy(node => node.MemberInfo.Category).
                ToDictionary(grp => grp.Key, grp => grp.ToArray());

            //Remove grouped members from original list
            foreach (var grouping in categorizedChildren)
                foreach (MemberTreeNode p in grouping.Value)
                    memberNodes.Remove(p);
            
            //Add a member node for each category
            foreach (var cat in categorizedChildren)
            {
                MemberTreeNode catNode = new MemberTreeNode(null, new TSerializeMemberInfo(null, cat.Key));
                
                foreach (MemberTreeNode catChild in cat.Value)
                    catNode.ChildElementMembers.Add(catChild);

                memberNodes.Add(catNode);
            }

            TreeNode.ChildElementMembers = new EventList<MemberTreeNode>(memberNodes);
        }
        public override void GenerateTreeFromBinary(ref VoidPtr address)
        {

        }
        public override void WriteTreeToBinary(ref VoidPtr address)
        {

        }
    }
}
