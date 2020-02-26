using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using Extensions;
using TheraEngine.Core.Reflection;
using TheraEngine.ComponentModel;

namespace TheraEngine.Core.Files.Serialization
{
    //[ObjectSerializerFor(typeof(object), CanSerializeAsString = true)]
    public class CommonObjectSerializer : BaseObjectSerializer
    {
        #region Reading

        public event Action DoneReadingChildMembers;

        public override async Task DeserializeTreeToObjectAsync()
        {
            bool async = TreeNode.MemberInfo?.DeserializeAsync ?? false;

            var (Count, Values) = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType);
            var values = Values.ToList();
            if (values.Count == 0)
            {
                //Engine.PrintLine($"Deserializing {TreeNode.ObjectType.GetFriendlyName()} {TreeNode.Name} as {nameof(ENodeType.ElementContent)}.");

                if (async)
                    TreeNode.Manager.PendingAsyncTasks.Add(Task.Run(ReadContent));
                else
                    ReadContent();
                
                return;
            }

            if (TreeNode.ObjectType.IsAbstract || TreeNode.ObjectType.IsInterface)
                TreeNode.Object = null;
            else
                TreeNode.Object = TreeNode.ObjectType.CreateInstance();
            
            if (TreeNode.Object is IFileObject fobj && fobj.RootFile != TreeNode.Manager.RootFileObject)
            {
                fobj.RootFile = TreeNode.Manager.RootFileObject as IFileObject;
                if (TreeNode.IsRoot)
                    fobj.FilePath = TreeNode.Manager.FilePath;
            }

            if (async)
            {
                TreeNode.Manager.PendingAsyncTasks.Add(Task.Run(() => ReadChildren(values)).ContinueWith(t => DoneReadingChildMembers?.Invoke()));
            }
            else
            {
                await ReadChildren(values);
                DoneReadingChildMembers?.Invoke();
            }
        }

        private void ReadContent() 
            => TreeNode.Object = TreeNode.Content.GetObject(TreeNode.ObjectType, out object obj) ? obj : null;

        private async Task ReadChildren(List<TSerializeMemberInfo> values)
        {
            foreach (MethodInfo m in TreeNode.PreDeserializeMethods.OrderBy(x => x.GetCustomAttribute<TPreDeserialize>().Order))
                m.Invoke(TreeNode.Object, m.GetCustomAttribute<TPreDeserialize>().Arguments);

            var categorizedChildren = values.
                Where(member => member.Category != null).
                GroupBy(member => member.Category).
                ToDictionary(grp => grp.Key, grp => grp.ToArray());

            foreach (var grouping in categorizedChildren)
                foreach (TSerializeMemberInfo p in grouping.Value)
                    values.Remove(p);

            foreach (var member in values)
                await ReadMember(TreeNode, member, TreeNode.Object);

            foreach (var catMember in categorizedChildren)
            {
                SerializeElement catNode = TreeNode.GetChildElement(catMember.Key);
                if (catNode != null)
                    foreach (var member in catMember.Value)
                        await ReadMember(catNode, member, TreeNode.Object);
            }

            foreach (MethodInfo m in TreeNode.PostDeserializeMethods.OrderBy(x => x.GetCustomAttribute<TPostDeserialize>().Order))
                m.Invoke(TreeNode.Object, m.GetCustomAttribute<TPostDeserialize>().Arguments);
        }
        private async Task ReadMember(SerializeElement parentNode, TSerializeMemberInfo member, object o)
        {
            SerializeElement node = parentNode.GetChildElement(member.Name);
            if (node != null)
            {
                node.MemberInfo = member;
                bool customInvoked = await TryInvokeManualParentDeserializeAsync(member, parentNode, node);
                if (!customInvoked)
                    await node.DeserializeTreeToObjectAsync();
            }
            else
            {
                SerializeAttribute attrib = parentNode.GetAttribute(member.Name);
                if (attrib != null)
                {
                    bool customInvoked = await TryInvokeManualParentDeserializeAsync(member, parentNode, attrib);
                    if (!customInvoked)
                    {
                        if (member.DeserializeAsync)
                        {
                            parentNode.Manager.PendingAsyncTasks.Add(Task.Run(() => 
                            {
                                if (attrib.GetObject(member.MemberType, out object value))
                                    member.SetObject(o, value);
                            }));
                        }
                        else if (attrib.GetObject(member.MemberType, out object value))
                            member.SetObject(o, value);
                    }
                }
                else if (member.NodeType == ENodeType.ElementContent)
                {
                    bool customInvoked = await TryInvokeManualParentDeserializeAsync(member, parentNode, parentNode.Content);
                    if (!customInvoked && parentNode.Content.GetObject(member.MemberType, out object value))
                        member.SetObject(o, value);
                }
            }
            //Otherwise, this member must be default
        }
        /// <summary>
        /// If the parent member overrides deserialization, use that.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <param name="parent"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> TryInvokeManualParentDeserializeAsync<T>(
            TSerializeMemberInfo member, SerializeElement parent, T data)
        {
            if (parent?.CustomDeserializeMethods is null)
                return false;

            IEnumerable<MethodInfoProxy> customMethods = parent.CustomDeserializeMethods.Where(
                x => string.Equals(member.Name, x.GetCustomAttribute<CustomMemberDeserializeMethod>().Name));

            foreach (MethodInfoProxy customMethod in customMethods)
            {
                ParameterInfoProxy[] parameters = customMethod.GetParameters();
                if (parameters.Length != 1 || !parameters[0].ParameterType.IsAssignableFrom(typeof(T)))
                {
                    Engine.LogWarning($"'{customMethod.GetFriendlyName()}' in class '{customMethod.DeclaringType.GetFriendlyName()}' is marked with a {nameof(CustomMemberDeserializeMethod)} attribute, but the arguments are not correct. There must be one argument of type {typeof(T).GetFriendlyName()}.");
                    continue;
                }

                //Engine.PrintLine($"Deserializing {member.MemberType.GetFriendlyName()} {member.Name} manually as {member.NodeType.ToString()} via parent.");

                if (customMethod.ReturnType == typeof(Task))
                    await (Task)customMethod.Invoke(parent.Object, new object[] { data });
                else
                    customMethod.Invoke(parent.Object, new object[] { data });

                return true;
            }

            return false;
        }
        #endregion

        #region Writing
        public static bool IsObjectDefault(object obj, object defObj)
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
                SkipInvalidIndexers = true,
                ComparePredicate = IsComparable
            });
            ComparisonResult result = comp.Compare(defObj, obj);
            //if (!result.AreEqual)
            //    Engine.PrintLine(result.DifferencesString);
            return result.AreEqual;
        }

        private static bool IsComparable(MemberInfo x)
        {
            bool isSerializable = x.GetCustomAttribute<TSerialize>() != null;
            switch (x)
            {
                case FieldInfo fieldInfo:
                    isSerializable = isSerializable || fieldInfo.FieldType.IsValueType;
                    break;
                case PropertyInfo propInfo:
                    isSerializable = isSerializable || propInfo.PropertyType.IsValueType;
                    break;
            }
            return isSerializable;
        }

        public override async Task SerializeTreeFromObjectAsync()
        {
            var (Count, Values) = SerializationCommon.CollectSerializedMembers(TreeNode.ObjectType);
            if (Count == 0)
            {
                if (ShouldWriteDefaultMembers || !TreeNode.IsObjectDefault())
                    TreeNode.Content.SetValueAsObject(TreeNode.Object);
                return;
            }
            
            SerializeElement parent = TreeNode;
            Dictionary<string, SerializeElement> categoryNodes = new Dictionary<string, SerializeElement>();
            foreach (TSerializeMemberInfo member in Values)
            {
                if (!member.AllowSerialize(TreeNode.Object))
                    continue;
                
                if (!string.IsNullOrWhiteSpace(member.Category))
                {
                    if (!categoryNodes.ContainsKey(member.Category))
                        categoryNodes.Add(member.Category, parent = new SerializeElement(null, new TSerializeMemberInfo(null, member.Category)));
                    else
                        parent = categoryNodes[member.Category];

                    parent.Manager = TreeNode.Manager;
                    parent.Parent = TreeNode.Parent;
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
                                !IsObjectDefault(resultMemberValue, TreeNode.DefaultObject is null ? null : member.GetObject(TreeNode.DefaultObject)))
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
                            parent.Children.Add(element);

                            bool manuallySerialized = await TryInvokeManualParentSerializeAsync(element.Name, TreeNode, element);
                            if (!manuallySerialized)
                            {
                                element.Object = member.GetObject(TreeNode.Object);
                                if (!ShouldWriteDefaultMembers && element.IsObjectDefault())
                                    parent.Children.RemoveAt(parent.Children.Count - 1);
                                else
                                    element.SerializeTreeFromObjectAsync();
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
                                !IsObjectDefault(resultMemberValue, TreeNode.DefaultObject is null ? null : member.GetObject(TreeNode.DefaultObject)))
                            {
                                if (!parent.Content.SetValueAsObject(resultMemberValue))
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
            if (parent?.CustomSerializeMethods is null)
                return false;

            IEnumerable<MethodInfoProxy> customMethods = parent.CustomSerializeMethods.Where(
                x => string.Equals(memberName, x.GetCustomAttribute<CustomMemberSerializeMethod>().Name));

            foreach (MethodInfoProxy customMethod in customMethods)
            {
                ParameterInfoProxy[] parameters = customMethod.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(typeof(SerializeElement)))
                {
                    if (customMethod.ReturnType == typeof(Task))
                        await (Task)customMethod.Invoke(parent.Object, new object[] { element });
                    else
                        customMethod.Invoke(parent.Object, new object[] { element });
                    return true;
                }

                Engine.LogWarning($"'{customMethod.GetFriendlyName()}' in class '{customMethod.DeclaringType.GetFriendlyName()}' is marked with a {nameof(CustomMemberSerializeMethod)} attribute, but the arguments are not correct. There must be one argument of type {typeof(SerializeElement).GetFriendlyName()}.");
            }

            return false;
        }
        /// <summary>
        /// Parent serializes this node
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, object)> TryInvokeManualParentSerializeAsync(string memberName, SerializeElement parent)
        {
            if (parent?.CustomSerializeMethods is null)
                return (false, null);

            IEnumerable<MethodInfoProxy> customMethods = parent.CustomSerializeMethods.Where(
                x => string.Equals(memberName, x.GetCustomAttribute<CustomMemberSerializeMethod>().Name));

            foreach (MethodInfoProxy customMethod in customMethods)
            {
                ParameterInfoProxy[] parameters = customMethod.GetParameters();
                if (parameters.Length == 0)
                {
                    object o;
                    if (customMethod.ReturnType == typeof(Task))
                        o = await (Task<object>)customMethod.Invoke(parent.Object, null);
                    else
                        o = customMethod.Invoke(parent.Object, null);
                    return (true, o);
                }

                Engine.LogWarning($"'{customMethod.GetFriendlyName()}' in class '{customMethod.DeclaringType.GetFriendlyName()}' is marked with a {nameof(CustomMemberSerializeMethod)} attribute, but the definition is not correct. There must be no arguments and the method should return an object that can be serialized as a string.");
            }
            
            return (false, null);
        }
        #endregion

        #region Binary
        public override void DeserializeTreeFromBinary(ref VoidPtr address, Deserializer.ReaderBinary binWriter)
        {

        }
        protected override int OnGetTreeSize(Serializer.WriterBinary binWriter)
        {
            return 0;
        }
        public override void SerializeTreeToBinary(ref VoidPtr address, Serializer.WriterBinary binWriter)
        {

        }
        #endregion

        #region String
        public override bool ObjectFromString(TypeProxy type, string value, out object result)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    result = null;
                    return true;
                }
                else
                {
                    type = type.GetGenericArguments()[0];
                    var ser = GetSerializerFor(type, true);
                    if (ser is null)
                    {
                        result = null;
                        return false;
                    }
                    return ser.ObjectFromString(type, value, out result);
                }
            }

            if (type.IsEnum)
            {
                value = value.ReplaceWhitespace("").Replace("/", ", ");
                result = type.ParseEnum(value);
                return true;
            }

            switch (type.Name)
            {
                case "Boolean": result = Boolean.Parse(value); return true;
                case "SByte": result = result = SByte.Parse(value); return true;
                case "Byte": result = Byte.Parse(value); return true;
                case "Char":    result =  Char.Parse(value); return true;
                case "Int16": result = Int16.Parse(value); return true;
                case "UInt16": result = UInt16.Parse(value); return true;
                case "Int32": result = Int32.Parse(value); return true;
                case "UInt32": result = UInt32.Parse(value); return true;
                case "Int64": result = Int64.Parse(value); return true;
                case "UInt64": result = UInt64.Parse(value); return true;
                case "Single": result = Single.Parse(value); return true;
                case "Double": result = Double.Parse(value); return true;
                case "Decimal": result = Decimal.Parse(value); return true;
                case "String": result = value; return true;
            }

            if (type.GetInterface(nameof(ISerializableString)) != null)
            {
                ISerializableString o = (ISerializableString)Activator.CreateInstance((Type)type);
                o.ReadFromString(value);
                result = o;
                return true;
            }

            if (type.IsValueType)
            {
                result = SerializationCommon.ParseStructBytesString(type, value);
                return true;
            }

            result = null;
            return false;
        }

        public override bool CanWriteAsString(TypeProxy type)
        {
            if (type.IsEnum)
                return true;
            
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
                    return true;
            }

            if (type.GetInterface(nameof(ISerializableString)) != null)
                return true;

            if (type.IsValueType)
                return true;

            return false;
        }
        public override bool ObjectToString(object obj, out string str)
        {
            if (obj is null)
            {
                str = string.Empty;
                return true;
            }

            Type type = obj.GetType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = type.GetGenericArguments()[0];
            
            if (type.IsEnum)
            {
                str = obj.ToString().Replace(",", "/").ReplaceWhitespace("");
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

            if (type.GetInterface(nameof(ISerializableString)) != null)
            {
                str = (obj as ISerializableString)?.WriteToString();
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
