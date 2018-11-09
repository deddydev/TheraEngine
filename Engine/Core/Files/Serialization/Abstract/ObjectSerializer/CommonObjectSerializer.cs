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
                SerializeElement catNode = TreeNode.GetChildElement(catMember.Key);
                if (catNode != null)
                    foreach (var member in catMember.Value)
                        ReadMember(catNode, member, TreeNode.Object);
            }

            foreach (MethodInfo m in TreeNode.PostDeserializeMethods.OrderBy(x => x.GetCustomAttribute<PostDeserialize>().Order))
                m.Invoke(TreeNode.Object, m.GetCustomAttribute<PostDeserialize>().Arguments);
        }
        private async void ReadMember(SerializeElement parentNode, TSerializeMemberInfo member, object o)
        {
            SerializeElement node = parentNode.GetChildElement(member.Name);
            if (node != null)
            {
                node.MemberInfo = member;
                bool customInvoked = await TryInvokeManualParentDeserializeAsync(member.Name, parentNode, node);
                if (!customInvoked)
                    node.TreeToObject();
            }
            else
            {
                var attrib = parentNode.GetAttribute(member.Name);
                if (attrib != null)
                {
                    bool customInvoked = await TryInvokeManualParentDeserializeAsync(member.Name, parentNode, attrib);
                    if (!customInvoked)
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
                }
                else if (member.NodeType == ENodeType.ElementContent)
                {
                    bool customInvoked = await TryInvokeManualParentDeserializeAsync(member.Name, parentNode, parentNode._elementContent);
                    if (!customInvoked)
                    {
                        if (parentNode.GetElementContent(member.MemberType, out object value))
                        {
                            member.SetObject(o, value);
                        }
                        else
                        {
                            Engine.LogWarning("Unable to parse element content " + member.Name + " as " + member.MemberType.GetFriendlyName());
                        }
                    }
                }
                else
                {
                    //Engine.LogWarning("Unable to parse member " + member.Name + " as " + member.MemberType.GetFriendlyName());
                }
            }
        }
        public async Task<bool> TryInvokeManualParentDeserializeAsync<T>(string memberName, SerializeElement parent, T data)
        {
            if (parent?.CustomDeserializeMethods == null)
                return false;

            var customMethods = parent.CustomDeserializeMethods.Where(
                x => string.Equals(memberName, x.GetCustomAttribute<CustomDeserializeMethod>().Name));

            foreach (var customMethod in customMethods)
            {
                if (customMethod == null)
                    continue;

                var parameters = customMethod.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(typeof(T)))
                {
                    if (customMethod.ReturnType == typeof(Task))
                        await (Task)customMethod.Invoke(parent.Object, new object[] { data });
                    else
                        customMethod.Invoke(parent.Object, new object[] { data });
                    return true;
                }
                else
                {
                    Engine.LogWarning($"'{customMethod.GetFriendlyName()}' in class '{customMethod.DeclaringType.GetFriendlyName()}' is marked with a {nameof(CustomDeserializeMethod)} attribute, but the arguments are not correct. There must be one argument of type {typeof(T).GetFriendlyName()}.");
                }
            }

            return false;
        }
        public override void TreeFromObject()
        {
            TSerializeMemberInfo[] members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType);

            if (members.Length == 0)
            {
                TreeNode.SetElementContent(TreeNode.Object);
                return;
            }

            List<SerializeElement> children = members.
                Where(x => x.AllowSerialize(TreeNode.Object)).
                Select(x => new SerializeElement(TreeNode, x)).
                ToList();
            
            //Group children by category if set
            var categorizedChildren = children.
                Where(node => node.MemberInfo.Category != null).
                GroupBy(node => node.MemberInfo.Category).
                ToDictionary(grp => grp.Key, grp => grp.ToArray());

            //Remove grouped members from original list
            foreach (var grouping in categorizedChildren)
                foreach (SerializeElement p in grouping.Value)
                    children.Remove(p);
            
            //Add a member node for each category
            foreach (var cat in categorizedChildren)
            {
                SerializeElement catNode = new SerializeElement(null, new TSerializeMemberInfo(null, cat.Key));
                
                foreach (SerializeElement catChild in cat.Value)
                    catNode.ChildElements.Add(catChild);

                children.Add(catNode);
            }

            TreeNode.ChildElements = new EventList<SerializeElement>(children);
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
