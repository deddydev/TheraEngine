using KellermanSoftware.CompareNetObjects;
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
    //[ObjectSerializerFor(typeof(object), CanSerializeAsString = true)]
    public class CommonObjectSerializer : BaseObjectSerializer
    {
        #region Reading
        public override void DeserializeTreeToObject()
        {
            var members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType).ToList();

            if (members.Count == 0)
            {
                //Engine.PrintLine($"Deserializing {TreeNode.ObjectType.GetFriendlyName()} {TreeNode.Name} as {nameof(ENodeType.ElementContent)}.");

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
            
            foreach (MethodInfo m in TreeNode.PreDeserializeMethods.OrderBy(x => x.GetCustomAttribute<TPreDeserialize>().Order))
                m.Invoke(TreeNode.Object, m.GetCustomAttribute<TPreDeserialize>().Arguments);

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

            foreach (MethodInfo m in TreeNode.PostDeserializeMethods.OrderBy(x => x.GetCustomAttribute<TPostDeserialize>().Order))
                m.Invoke(TreeNode.Object, m.GetCustomAttribute<TPostDeserialize>().Arguments);
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
                x => string.Equals(member.Name, x.GetCustomAttribute<TCustomMemberDeserializeMethod>().Name));

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
                    Engine.LogWarning($"'{customMethod.GetFriendlyName()}' in class '{customMethod.DeclaringType.GetFriendlyName()}' is marked with a {nameof(TCustomMemberDeserializeMethod)} attribute, but the arguments are not correct. There must be one argument of type {typeof(T).GetFriendlyName()}.");
                }
            }

            return false;
        }
        #endregion

        #region Writing
        public bool IsObjectDefault(object obj, object defObj)
        {
            //Deep compare the current object with the default object
            CompareLogic comp = new CompareLogic(new ComparisonConfig()
            {
                CompareChildren = true,
                CompareFields = true,
                CompareProperties = true,
                ComparePrivateFields = true,
                ComparePrivateProperties = true,
                CompareStaticFields = false,
                CompareStaticProperties = false,
                CompareReadOnly = false,
                ComparePredicate = x => x.GetCustomAttribute<TSerialize>() != null
            });
            ComparisonResult result = comp.Compare(defObj, obj);
            //if (!result.AreEqual)
            //    Engine.PrintLine(result.DifferencesString);
            return result.AreEqual;
        }
        public override async void SerializeTreeFromObject()
        {
            TSerializeMemberInfo[] members = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType);

            if (members.Length == 0)
            {
                if (ShouldWriteDefaultMembers || !TreeNode.IsObjectDefault())
                    TreeNode.SetElementContent(TreeNode.Object);
                return;
            }
            
            SerializeElement parent = TreeNode;
            Dictionary<string, SerializeElement> categoryNodes = new Dictionary<string, SerializeElement>();
            foreach (TSerializeMemberInfo member in members)
            {
                if (!member.AllowSerialize(TreeNode.Object))
                    continue;
                
                if (!string.IsNullOrWhiteSpace(member.Category))
                {
                    if (!categoryNodes.ContainsKey(member.Category))
                        categoryNodes.Add(member.Category, 
                            parent = new SerializeElement(null, new TSerializeMemberInfo(null, member.Category)));
                    else
                        parent = categoryNodes[member.Category];
                }

                switch (member.NodeType)
                {
                    case ENodeType.Attribute:
                        {
                            (bool manuallySerialized, object resultMemberValue) = await TryInvokeManualParentSerializeAsync(member.Name, TreeNode);

                            if (!manuallySerialized)
                                resultMemberValue = member.GetObject(TreeNode.Object);

                            if (manuallySerialized ||
                                ShouldWriteDefaultMembers ||
                                !IsObjectDefault(resultMemberValue, TreeNode.DefaultObject == null ? null : member.GetObject(TreeNode.DefaultObject)))
                            {
                                SerializeAttribute attribute = new SerializeAttribute { Name = member.Name };
                                attribute.SetValueAsObject(resultMemberValue);
                                parent.Attributes.Add(attribute);
                            }
                        }
                        break;
                    case ENodeType.ChildElement:
                        {
                            SerializeElement element = new SerializeElement(null, member);
                            parent.ChildElements.Add(element);

                            bool manuallySerialized = await TryInvokeManualParentSerializeAsync(element.Name, TreeNode, element);
                            if (!manuallySerialized)
                            {
                                element.Object = member.GetObject(TreeNode.Object);
                                if (!ShouldWriteDefaultMembers && element.IsObjectDefault())
                                    parent.ChildElements.RemoveAt(parent.ChildElements.Count - 1);
                                else
                                    element.SerializeTreeFromObject();
                            }
                            //else if (!ShouldWriteDefaultMembers && element.ObjectIsDefault)
                            //    parent.ChildElements.RemoveAt(parent.ChildElements.Count - 1);
                        }
                        break;
                    case ENodeType.ElementContent:
                        {
                            (bool manuallySerialized, object resultMemberValue) = await TryInvokeManualParentSerializeAsync(member.Name, TreeNode);

                            if (!manuallySerialized)
                                resultMemberValue = member.GetObject(TreeNode.Object);

                            if (manuallySerialized ||
                                ShouldWriteDefaultMembers ||
                                !IsObjectDefault(resultMemberValue, TreeNode.DefaultObject == null ? null : member.GetObject(TreeNode.DefaultObject)))
                            {
                                if (!parent.SetElementContent(resultMemberValue))
                                {
                                    Engine.LogWarning("Unable to set element content.");
                                }
                            }
                        }
                        break;
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
                x => string.Equals(memberName, x.GetCustomAttribute<TCustomMemberSerializeMethod>().Name));

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
                    Engine.LogWarning($"'{customMethod.GetFriendlyName()}' in class '{customMethod.DeclaringType.GetFriendlyName()}' is marked with a {nameof(TCustomMemberSerializeMethod)} attribute, but the arguments are not correct. There must be one argument of type {typeof(SerializeElement).GetFriendlyName()}.");
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
                x => string.Equals(memberName, x.GetCustomAttribute<TCustomMemberSerializeMethod>().Name));

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
                    Engine.LogWarning($"'{customMethod.GetFriendlyName()}' in class '{customMethod.DeclaringType.GetFriendlyName()}' is marked with a {nameof(TCustomMemberSerializeMethod)} attribute, but the definition is not correct. There must be no arguments and the method should return an object.");
                }
            }
            
            return (false, null);
        }
        #endregion

        #region Binary
        public override void DeserializeTreeFromBinary(ref VoidPtr address, TDeserializer.ReaderBinary binWriter)
        {

        }
        protected override int OnGetTreeSize(TSerializer.WriterBinary binWriter)
        {
            return 0;
        }
        public override void SerializeTreeToBinary(ref VoidPtr address, TSerializer.WriterBinary binWriter)
        {

        }
        #endregion

        #region String
        public override object ObjectFromString(Type type, string value)
        {
            if (type.IsEnum)
            {
                value = value.ReplaceWhitespace("").Replace("|", ", ");
                return Enum.Parse(type, value);
            }
            switch (type.Name)
            {
                case "Boolean": return Boolean.Parse(value);
                case "SByte":   return SByte.Parse(value);
                case "Byte":    return Byte.Parse(value);
                case "Char":    return Char.Parse(value);
                case "Int16":   return Int16.Parse(value);
                case "UInt16":  return UInt16.Parse(value);
                case "Int32":   return Int32.Parse(value);
                case "UInt32":  return UInt32.Parse(value);
                case "Int64":   return Int64.Parse(value);
                case "UInt64":  return UInt64.Parse(value);
                case "Single":  return Single.Parse(value);
                case "Double":  return Double.Parse(value);
                case "Decimal": return Decimal.Parse(value);
                case "String":  return value;
            }
            if (type.IsValueType)
                return SerializationCommon.ParseStructBytesString(type, value);
            throw new InvalidOperationException();
        }
        public override bool ObjectToString(object obj, out string str)
        {
            Type type = obj.GetType();
            if (type.IsEnum)
            {
                str = obj.ToString().Replace(",", "|").ReplaceWhitespace("");
                return true;
            }
            switch (type.Name)
            {
                case "Boolean":
                case "SByte":
                case "Byte":
                case "Char":
                case "Int16":
                case "UInt16":
                case "Int32":
                case "UInt32":
                case "Int64":
                case "UInt64":
                case "Single":
                case "Double":
                case "Decimal":
                case "String":
                    str = obj.ToString();
                    return true;
            }
            if (type.IsValueType)
            {
                str = SerializationCommon.GetStructAsBytesString(obj);
                return true;
            }
            str = null;
            return false;
        }
        #endregion
    }
}
