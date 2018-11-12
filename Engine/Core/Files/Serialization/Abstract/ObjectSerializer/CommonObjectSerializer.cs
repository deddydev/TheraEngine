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
        #region Reading
        public override void DeserializeTreeToObject()
        {
            var members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType).ToList();

            if (members.Count == 0)
            {
                Engine.PrintLine($"Deserializing {TreeNode.ObjectType.GetFriendlyName()} {TreeNode.Name} as {nameof(ENodeType.ElementContent)}.");

                bool success = TreeNode.GetElementContent(TreeNode.ObjectType, out object obj);
                TreeNode.Object = success ? obj : null;
                return;
            }

            TreeNode.Object = SerializationCommon.CreateObject(TreeNode.ObjectType);
            
            if (TreeNode.Object is TFileObject fobj)
            {
                fobj.ConstructedProgrammatically = false;
                fobj.RootFile = TreeNode.Owner.RootFileObject as TFileObject;
                if (TreeNode.IsRoot)
                    fobj.FilePath = TreeNode.Owner.FilePath;
            }
            
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
                bool customInvoked = await TryInvokeManualParentDeserializeAsync(member, parentNode, node);
                if (!customInvoked)
                    node.DeserializeTreeToObject();
            }
            else
            {
                var attrib = parentNode.GetAttribute(member.Name);
                if (attrib != null)
                {
                    bool customInvoked = await TryInvokeManualParentDeserializeAsync(member, parentNode, attrib);
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
                    bool customInvoked = await TryInvokeManualParentDeserializeAsync(member, parentNode, parentNode._elementContent);
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
        public async Task<bool> TryInvokeManualParentDeserializeAsync<T>(TSerializeMemberInfo member, SerializeElement parent, T data)
        {
            if (parent?.CustomDeserializeMethods == null)
                return false;

            var customMethods = parent.CustomDeserializeMethods.Where(
                x => string.Equals(member.Name, x.GetCustomAttribute<CustomDeserializeMethod>().Name));

            foreach (var customMethod in customMethods)
            {
                if (customMethod == null)
                    continue;

                var parameters = customMethod.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(typeof(T)))
                {
                    Engine.PrintLine($"Deserializing {member.MemberType.GetFriendlyName()} {member.Name} manually as {member.NodeType.ToString()} via parent.");

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
        #endregion

        #region Writing
        public override async void SerializeTreeFromObject()
        {
            TSerializeMemberInfo[] members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType);

            if (members.Length == 0)
            {
                TreeNode.SetElementContent(TreeNode.Object);
                return;
            }
            
            Dictionary<string, List<SerializeElement>> categories = new Dictionary<string, List<SerializeElement>>();
            foreach (TSerializeMemberInfo member in members)
            {
                if (/*TreeNode.Object == TreeNode.DefaultObject || */TreeNode.Object == null || !member.AllowSerialize(TreeNode.Object))
                    continue;

                switch (member.NodeType)
                {
                    case ENodeType.Attribute:
                        {
                            (bool success, object value) = await TryInvokeManualParentSerializeAsync(member.Name, TreeNode);

                            object obj;
                            if (success)
                                obj = value;
                            else
                                obj = member.GetObject(TreeNode.Object);

                            if (obj != null)
                            {
                                SerializeAttribute attribute = new SerializeAttribute { Name = member.Name };
                                attribute.SetValueAsObject(obj);
                                TreeNode.Attributes.Add(attribute);
                            }
                        }
                        break;
                    case ENodeType.ChildElement:
                        {
                            SerializeElement element = new SerializeElement(null, member);
                            if (!string.IsNullOrWhiteSpace(member.Category))
                            {
                                if (!categories.ContainsKey(member.Category))
                                    categories.Add(member.Category, new List<SerializeElement>() { element });
                                else
                                    categories[member.Category].Add(element);
                            }
                            else
                            {
                                TreeNode.ChildElements.Add(element);
                            }
                        }
                        break;
                    case ENodeType.ElementContent:
                        {
                            object obj;
                            (bool success, object value) = await TryInvokeManualParentSerializeAsync(member.Name, TreeNode);
                            if (success)
                                obj = value;
                            else
                                obj = member.GetObject(TreeNode.Object);
                            if (obj != null)
                                TreeNode.SetElementContent(obj);
                        }
                        break;
                }
            }

            foreach (var node in TreeNode.ChildElements)
            {
                bool manualSuccess = await TryInvokeManualParentSerializeAsync(node.Name, TreeNode, node);
                if (!manualSuccess)
                {
                    node.RetrieveObjectFromParent();
                    node.SerializeTreeFromObject();
                }
            }
            foreach (var cat in categories)
            {
                SerializeElement catNode = new SerializeElement(null, new TSerializeMemberInfo(null, cat.Key));
                TreeNode.ChildElements.Add(catNode);

                foreach (SerializeElement catChild in cat.Value)
                {
                    catNode.ChildElements.Add(catChild);

                    bool manualSuccess = await TryInvokeManualParentSerializeAsync(catChild.Name, TreeNode, catChild);
                    if (!manualSuccess)
                    {
                        catNode.Object = catNode.MemberInfo.GetObject(TreeNode.Object);
                        catNode.SerializeTreeFromObject();
                    }
                }
            }
        }
        /// <summary>
        /// Parent serializes this node
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TryInvokeManualParentSerializeAsync(string memberName, SerializeElement parent, SerializeElement element)
        {
            if (parent?.CustomSerializeMethods == null)
                return false;

            var customMethods = parent.CustomSerializeMethods.Where(
                x => string.Equals(memberName, x.GetCustomAttribute<CustomSerializeMethod>().Name));

            foreach (var customMethod in customMethods)
            {
                if (customMethod == null)
                    continue;

                var parameters = customMethod.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(typeof(SerializeElement)))
                {
                    if (customMethod.ReturnType == typeof(Task))
                        await (Task)customMethod.Invoke(parent.Object, new object[] { element });
                    else
                        customMethod.Invoke(parent.Object, new object[] { element });
                    return true;
                }
                else
                {
                    Engine.LogWarning($"'{customMethod.GetFriendlyName()}' in class '{customMethod.DeclaringType.GetFriendlyName()}' is marked with a {nameof(CustomSerializeMethod)} attribute, but the arguments are not correct. There must be one argument of type {typeof(SerializeElement).GetFriendlyName()}.");
                }
            }

            return false;
        }
        /// <summary>
        /// Parent serializes this node
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, object)> TryInvokeManualParentSerializeAsync(string memberName, SerializeElement parent)
        {
            if (parent?.CustomSerializeMethods == null)
                return (false, null);

            var customMethods = parent.CustomSerializeMethods.Where(
                x => string.Equals(memberName, x.GetCustomAttribute<CustomSerializeMethod>().Name));

            foreach (var customMethod in customMethods)
            {
                if (customMethod == null)
                    continue;

                var parameters = customMethod.GetParameters();
                if (parameters.Length == 0)
                {
                    object o;
                    if (customMethod.ReturnType == typeof(Task))
                        o = await (Task<object>)customMethod.Invoke(parent.Object, null);
                    else
                        o = customMethod.Invoke(parent.Object, null);
                    return (true, o);
                }
                else
                {
                    Engine.LogWarning($"'{customMethod.GetFriendlyName()}' in class '{customMethod.DeclaringType.GetFriendlyName()}' is marked with a {nameof(CustomSerializeMethod)} attribute, but the definition is not correct. There must be no arguments and the method should return an object.");
                }
            }
            
            return (false, null);
        }
        #endregion

        #region Binary
        public override void TreeFromBinary(ref VoidPtr address, TDeserializer.ReaderBinary binWriter)
        {

        }
        public override int OnGetTreeSize(TSerializer.WriterBinary binWriter)
        {
            return 0;
        }
        public override void TreeToBinary(ref VoidPtr address, TSerializer.WriterBinary binWriter)
        {

        }
        #endregion
    }
}
