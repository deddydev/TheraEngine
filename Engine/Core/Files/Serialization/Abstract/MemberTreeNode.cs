using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Core.Files.Serialization
{
    public class BinaryMemberTreeNode : MemberTreeNode<BinaryMemberTreeNode>
    {
        public BinaryMemberTreeNode(object obj)
            : base(obj) { }
        public BinaryMemberTreeNode(BinaryMemberTreeNode parent, TSerializeMemberInfo memberInfo)
            : base(parent, memberInfo) { }

        public List<BinaryMemberTreeNode> Children { get; internal set; }

        internal int CalculatedSize { get; set; }
        internal int ManuallyCalculatedSize { get; set; }
        internal byte[] ParsableBytes { get; set; } = null;
        internal string ParsableString { get; set; } = null;
        internal int ParsablePointerSize { get; set; } = 0;

        protected override async Task OnAddChildAsync(BinaryMemberTreeNode childMember)
        {
            Children.Add(childMember);
            await childMember.CreateTreeFromObjectAsync();
        }
        protected internal override async Task AddChildNodesAsync(int attribCount, int elementCount, int elementStringCount, List<BinaryMemberTreeNode> members)
        {
            Children = new List<BinaryMemberTreeNode>(members.Count);
            foreach (BinaryMemberTreeNode member in members)
                await AddChildAsync(member);
        }
        protected internal override List<IMemberTreeNode> GetChildNodes(Type objectType)
        {

        }
    }
    public class XMLAttribute
    {
        public XMLAttribute() { }
        public XMLAttribute(string name, string value) { Name = name; Value = value; }

        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class XMLMemberTreeNode : MemberTreeNode<XMLMemberTreeNode>
    {
        public XMLMemberTreeNode(object obj)
            : base(obj) { }
        public XMLMemberTreeNode(XMLMemberTreeNode parent, TSerializeMemberInfo memberInfo)
            : base(parent, memberInfo) { }
        public XMLMemberTreeNode(string name, string elementString, Type objectType)
            : base(null)
        {
            MemberInfo = new TSerializeMemberInfo(name, null, objectType, new TSerialize() { NodeType = ENodeType.ChildElement });
            ElementString = elementString;
        }
        public XMLMemberTreeNode(string name, XMLAttribute[] attributes)
            : base(null)
        {
            MemberInfo = new TSerializeMemberInfo();
            Attributes = attributes.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<XMLAttribute> Attributes { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public List<XMLMemberTreeNode> ChildElements { get; internal set; }
        /// <summary>
        /// String information for THIS element. If not null and ChildElements has no entries,
        /// this string is written between the open and close tags. If ChildElements has entries,
        /// then this string is written as a child element as well.
        /// </summary>
        public string ElementString { get; internal set; }
        public int NonAttributeCount { get; private set; }

        protected override async Task OnAddChildAsync(XMLMemberTreeNode childMember)
        {
            if (childMember != null)
            {
                if (childMember.NodeType == ENodeType.ChildElement)
                {
                    ChildElements.Add(childMember);
                    await childMember.CreateTreeFromObjectAsync();
                }
                else
                {
                    if (SerializationCommon.GetString(childMember.Object, childMember.ObjectType, out string result))
                    {
                        if (childMember.NodeType == ENodeType.ElementString && NonAttributeCount == 1)
                            ElementString = result;
                        else
                            Attributes.Add(new XMLAttribute(childMember.Name, result));
                    }
                    else
                        Engine.LogWarning(ObjectType.Name + " cannot be written as a string.");
                }
            }
        }

        protected internal override async Task AddChildNodesAsync(int attribCount, int elementCount, int elementStringCount, List<XMLMemberTreeNode> members)
        {
            NonAttributeCount = elementCount + elementStringCount;
            Attributes = new List<XMLAttribute>(attribCount);
            ChildElements = new List<XMLMemberTreeNode>(elementCount);
            ElementString = null;
            ProgressionCount = 1 + attribCount + elementCount + elementStringCount;
            if (IsDerivedType)
                ++ProgressionCount;
            foreach (XMLMemberTreeNode member in members)
            {
                await AddChildAsync(member);
                ProgressionCount += member.ProgressionCount;
            }
        }
        protected internal override List<IMemberTreeNode> GetChildNodes(Type objectType)
        {
            int elementCount = 0;
            var members = SerializationCommon.CollectSerializedMembers(objectType);
            TSerializeMemberInfo member;

            List<IMemberTreeNode> nodes = new List<IMemberTreeNode>();
            foreach (var attrib in Attributes)
            {
                string name = attrib.Name;
                string val = attrib.Value;
                member = members.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCulture));
                if (member == null)
                {
                    Engine.PrintLine($"No member in {objectType.GetFriendlyName()} matches attribute named '{name}'.");
                }
                else if (!SerializationCommon.CanParseAsString(member.MemberType))
                {
                    Engine.PrintLine($"The value ({val}) of the attribute named '{name}' cannot be parsed as {member.MemberType.GetFriendlyName()}.");
                }
                else
                {
                    object o = SerializationCommon.ParseString(val, member.MemberType);
                    XMLMemberTreeNode attribNode = new XMLMemberTreeNode(o) { NodeType = ENodeType.Attribute };
                    nodes.Add(attribNode);
                }
            }
            if (ChildElements != null)
            {
                foreach (XMLMemberTreeNode node in ChildElements)
                {
                    ++elementCount;
                    node.NodeType = ENodeType.ChildElement;
                    nodes.Add(node);
                }
            }
            if (ElementString != null)
            {
                if (elementCount > 0)
                {
                    XMLMemberTreeNode attribNode = new XMLMemberTreeNode(null)
                    {
                        NodeType = ENodeType.Attribute,
                    };
                    ++elementCount;

                }
                else
                {

                }
            }
            return nodes;
        }
        public void AddChildElementString(string elementName, string value, Type objectType)
        {
            XMLMemberTreeNode element = new XMLMemberTreeNode(elementName, value)
            {
                NodeType = ENodeType.ChildElement,
            };
        }
        public XMLAttribute GetAttribute(string attributeName)
            => Attributes.Find(x => string.Equals(x.Name, attributeName, StringComparison.InvariantCulture));
        public XMLMemberTreeNode GetChildElement(string elementName)
            => ChildElements.Find(x => string.Equals(x.MemberInfo?.Name, elementName, StringComparison.InvariantCulture));
        public void AddAttribute(string name, string value)
            => Attributes.Add(new XMLAttribute(name, value));
    }
    public interface IMemberTreeNode
    {
        int ProgressionCount { get; }
        /// <summary>
        /// The member this node references.
        /// </summary>
        TSerializeMemberInfo MemberInfo { get; set; }
        /// <summary>
        /// The default value set by the owning object in its constructor with no arguments.
        /// </summary>
        object DefaultObject { get; }
        /// <summary>
        /// The value currently attributed to this member.
        /// </summary>
        object Object { get; set; }
        /// <summary>
        /// The type of the object.
        /// </summary>
        Type ObjectType { get; }
        /// <summary>
        /// True if the object assigned to this member inherits from the member's specified type.
        /// </summary>
        bool IsDerivedType { get; }
        /// <summary>
        /// The node for the object that owns this object.
        /// </summary>
        IMemberTreeNode Parent { get; set; }
        /// <summary>
        /// The serializer that is using this node.
        /// </summary>
        TBaseSerializer.IBaseAbstractReaderWriter Owner { get; }
        /// <summary>
        /// The serializer that will be used to read and write this object.
        /// </summary>
        BaseObjectSerializer ObjectSerializer { get; set; }

        List<MethodInfo> PreSerializeMethods { get; }
        List<MethodInfo> PostSerializeMethods { get; }
        List<MethodInfo> PreDeserializeMethods { get; }
        List<MethodInfo> PostDeserializeMethods { get; }
        List<MethodInfo> CustomSerializeMethods  { get; }
        List<MethodInfo> CustomDeserializeMethods  { get; }

        /// <summary>
        /// Adds child member tree nodes. Depending on the serialization format,
        /// may take the form of elements, attributes, or child element string data.
        /// </summary>
        /// <param name="attribCount"></param>
        /// <param name="elementCount"></param>
        /// <param name="elementStringCount"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        Task AddChildNodesAsync(int attribCount, int elementCount, int elementStringCount, List<IMemberTreeNode> members);
        List<IMemberTreeNode> GetChildNodes(Type objectType);

        Task GenerateObjectFromTreeAsync(Type objectType);
        Task CreateTreeFromObjectAsync();
    }
    public abstract class MemberTreeNode<T> : IMemberTreeNode where T : class, IMemberTreeNode
    {
        public bool IsGroupingNode => MemberInfo == null;

        public T Parent { get; internal set; }
        IMemberTreeNode IMemberTreeNode.Parent { get => Parent; set => Parent = value as T; }

        /// <summary>
        /// How many checkpoints need to be hit for this node and all child nodes to advance the progression handler.
        /// </summary>
        public int ProgressionCount { get; internal set; }
        public TBaseSerializer.IBaseAbstractReaderWriter Owner { get; internal set; }
        public TSerializeMemberInfo MemberInfo { get; set; }

        public object DefaultObject { get; private set; }

        private object _object;
        /// <summary>
        /// The value assigned to this member.
        /// </summary>
        public object Object
        {
            get => _object;
            set
            {
                _object = value;

                if (ObjectType != null)
                {
                    IEnumerable<MethodInfo> methods = ObjectType?.GetMethods(
                        BindingFlags.NonPublic |
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.FlattenHierarchy);

                    PreSerializeMethods = new List<MethodInfo>();
                    PostSerializeMethods = new List<MethodInfo>();
                    PreDeserializeMethods = new List<MethodInfo>();
                    PostDeserializeMethods = new List<MethodInfo>();
                    CustomSerializeMethods = new List<MethodInfo>();
                    CustomDeserializeMethods = new List<MethodInfo>();

                    foreach (MethodInfo m in methods)
                    {
                        var attribs = m.GetCustomAttributes();
                        foreach (var attrib in attribs)
                        {
                            if (attrib is SerializationAttribute serAttrib &&
                                serAttrib.RunForFormats.HasFlag(Owner.Format))
                            {
                                switch (attrib.GetType().Name)
                                {
                                    case nameof(PreDeserialize):
                                        PreDeserializeMethods.Add(m);
                                        break;
                                    case nameof(PostDeserialize):
                                        PostDeserializeMethods.Add(m);
                                        break;
                                    case nameof(PreSerialize):
                                        PreSerializeMethods.Add(m);
                                        break;
                                    case nameof(PostSerialize):
                                        PostSerializeMethods.Add(m);
                                        break;
                                    case nameof(CustomSerializeMethod):
                                        CustomSerializeMethods.Add(m);
                                        break;
                                    case nameof(CustomDeserializeMethod):
                                        CustomDeserializeMethods.Add(m);
                                        break;
                                }
                            }
                        }
                    }
                    
                    //DefaultConstructedObject = SerializationCommon.CreateObject(ObjectType);
                    ObjectSerializer = SerializationCommon.DetermineObjectSerializer(ObjectType, this);
                }
                else
                {

                }
            }
        }
        /// <summary>
        /// The type of the object assigned to this member.
        /// </summary>
        public Type ObjectType => _object?.GetType() ?? MemberInfo?.MemberType;
        /// <summary>
        /// <see langword="true"/> if the object's type inherits from the member's type instead of matching it exactly.
        /// </summary>
        public bool IsDerivedType => Parent == null || (ObjectType?.IsSubclassOf(MemberInfo.MemberType) ?? false);
        /// <summary>
        /// The class handling how to read and write all members of any given object.
        /// Most classes will use <see cref="CommonSerializer"/>.
        /// </summary>
        public BaseObjectSerializer ObjectSerializer { get; set; }
        /// <summary>
        /// Methods for serializing data in a specific manner.
        /// </summary>
        public List<MethodInfo> CustomSerializeMethods { get; private set; }
        public List<MethodInfo> CustomDeserializeMethods { get; private set; }
        public List<MethodInfo> PreSerializeMethods { get; private set; }
        public List<MethodInfo> PostSerializeMethods { get; private set; }
        public List<MethodInfo> PreDeserializeMethods { get; private set; }
        public List<MethodInfo> PostDeserializeMethods { get; private set; }
        
        public MemberTreeNode()
        {
            Parent = null;
            MemberInfo = null;
            Object = null;
            DefaultObject = null;
        }
        public MemberTreeNode(object obj)
        {
            Object = obj;
            Parent = null;

            MemberInfo = new TSerializeMemberInfo(
                SerializationCommon.GetTypeName(ObjectType),
                null, ObjectType, new TSerialize());

            DefaultObject = ObjectType == null ? null : SerializationCommon.CreateObject(ObjectType);
        }
        public MemberTreeNode(T parent, TSerializeMemberInfo memberInfo)
        {
            Parent = parent;
            MemberInfo = memberInfo;
            GetObject();
        }

        public async Task AddChildAsync(T childMember)
        {
            childMember.Parent = this;

            if (childMember?.Object == null)
                return;
            if (childMember.Owner is TSerializer.IAbstractWriter writer)
            {
                if (childMember.MemberInfo.Attribute.State && !writer.Flags.HasFlag(ESerializeFlags.SerializeState))
                    return;
                if (childMember.MemberInfo.Attribute.Config && !writer.Flags.HasFlag(ESerializeFlags.SerializeConfig))
                    return;
            }

            var customMethods = CustomSerializeMethods.Where(
                x => string.Equals(childMember.MemberInfo.Name, x.GetCustomAttribute<CustomSerializeMethod>().Name));

            foreach (var customMethod in customMethods)
            {
                if (customMethod == null)
                    continue;

                var parameters = customMethod.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(GetType()))
                {
                    if (customMethod.ReturnType == typeof(Task))
                        await (Task)customMethod.Invoke(Object, new object[] { this });
                    else
                        customMethod.Invoke(Object, new object[] { this });
                    return;
                }
                else
                {
                    Engine.LogWarning($"Method {customMethod.GetFriendlyName()} is marked with a {nameof(CustomSerializeMethod)} attribute, but the arguments are not correct. There must be one argument of type {nameof(IMemberTreeNode)}.");
                }
            }

            await OnAddChildAsync(childMember);
        }
        protected abstract Task OnAddChildAsync(T member);
        
        //public ESerializeType GetSerializeType(Type type)
        //{
        //    if (type.GetInterface(nameof(IStringParsable)) != null)
        //        return ESerializeType.Parsable;
        //    else if (type.IsEnum)
        //        return ESerializeType.Enum;
        //    else if (type == typeof(string))
        //        return ESerializeType.String;
        //    else if (type.IsValueType)
        //        return ESerializeType.Struct;
        //    else
        //    {
        //        FileExt ext = TFileObject.GetFileExtension(type);
        //        if (ext != null)
        //        {
        //            EProprietaryFileFormat format = FormatWriter.Owner.Format;
        //            bool serializeConfig = FormatWriter.Flags.HasFlag(ESerializeFlags.SerializeConfig);
        //            bool serializeState = FormatWriter.Flags.HasFlag(ESerializeFlags.SerializeState);

        //            switch (format)
        //            {
        //                case EProprietaryFileFormat.Binary:
        //                    if (serializeConfig && ext.ManualBinConfigSerialize)
        //                        return ESerializeType.Manual;
        //                    if (serializeState && ext.ManualBinStateSerialize)
        //                        return ESerializeType.Manual;
        //                    break;
        //                case EProprietaryFileFormat.XML:
        //                    if (serializeConfig && ext.ManualXmlConfigSerialize)
        //                        return ESerializeType.Manual;
        //                    if (serializeState && ext.ManualXmlStateSerialize)
        //                        return ESerializeType.Manual;
        //                    break;
        //            }
        //        }
                
        //        return ESerializeType.Class;
        //    }
        //}
        public async Task CreateTreeFromObjectAsync()
        {
            await FileObjectCheckAsync();
            if (ObjectSerializer != null)
                await ObjectSerializer.GenerateTreeFromObject();
        }
        //private void SetObject(object value)
        //{
        //    _object = value;
        //    if (MemberInfo.MemberType.HasFlag(MemberTypes.Field))
        //        ((FieldInfo)MemberInfo).SetValue(Parent.Object, value);
        //    else if (MemberInfo.MemberType.HasFlag(MemberTypes.Property))
        //    {
        //        PropertyInfo p = (PropertyInfo)MemberInfo;
        //        if (p.CanWrite)
        //            p.SetValue(Parent.Object, value);
        //        else
        //            Engine.LogWarning("Can't set property '" + p.Name + "' in " + p.DeclaringType.GetFriendlyName());
        //    }
        //}
        public void GetObject()
        {
            DefaultObject = null;
            if (Parent?.Object is null || MemberInfo?.Member is null)
            {
                Object = null;
                return;
            }
            
            if (MemberInfo.Member.MemberType.HasFlag(MemberTypes.Field))
            {
                FieldInfo info = (FieldInfo)MemberInfo.Member;
                Object = info.GetValue(Parent.Object);
                if (!(Parent.DefaultObject is null))
                    DefaultObject = info.GetValue(Parent.DefaultObject);
            }
            else if (MemberInfo.Member.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfo info = (PropertyInfo)MemberInfo.Member;
                if (info.CanRead)
                {
                    Object = info.GetValue(Parent.Object);
                    if (!(Parent.DefaultObject is null))
                        DefaultObject = info.GetValue(Parent.DefaultObject);
                }
                else
                {
                    Engine.LogWarning("Can't read property '" + info.Name + "' in " + info.DeclaringType.GetFriendlyName());
                }
            }
            else
            {
                Engine.LogWarning($"Member {MemberInfo.Name} is not a field or property.");
            }
        }
        /// <summary>
        /// Performs special processing for classes that implement <see cref="IFileObject"/> and <see cref="IFileRef"/>.
        /// </summary>
        private async Task FileObjectCheckAsync()
        {
            //Update the object's file path
            if (Object is IFileObject fobj)
            {
                fobj.FilePath = Owner.FilePath;
                if (fobj is IFileRef fref && !fref.StoredInternally)
                {
                    //Make some last minute adjustments to external file refs
                    //First, update file relative paths using the new file location
                    if (fref.PathType == EPathType.FileRelative)
                    {
                        string root = Path.GetPathRoot(fref.ReferencePathAbsolute);
                        int colonIndex = root.IndexOf(":");
                        if (colonIndex > 0)
                            root = root.Substring(0, colonIndex);
                        else
                            root = string.Empty;

                        string root2 = Path.GetPathRoot(Owner.FileDirectory);
                        colonIndex = root2.IndexOf(":");
                        if (colonIndex > 0)
                            root2 = root2.Substring(0, colonIndex);
                        else
                            root2 = string.Empty;

                        if (!string.Equals(root, root2))
                        {
                            //Totally different drives, cannot be relative in any way
                            fref.PathType = EPathType.Absolute;
                        }
                    }
                    if (fref.IsLoaded)
                    {
                        string path = fref.ReferencePathAbsolute;
                        bool fileExists =
                            !string.IsNullOrWhiteSpace(path) &&
                            path.IsExistingDirectoryPath() == false &&
                            File.Exists(path);

                        //TODO: export even if the file exists,
                        //however only if the file has changed
                        if (!fileExists)
                        {
                            if (fref is IGlobalFileRef && !Owner.Flags.HasFlag(ESerializeFlags.ExportGlobalRefs))
                                return;
                            if (fref is ILocalFileRef && !Owner.Flags.HasFlag(ESerializeFlags.ExportLocalRefs))
                                return;

                            string absPath;
                            if (fref.PathType == EPathType.FileRelative)
                            {
                                string rel = fref.ReferencePathAbsolute.MakePathRelativeTo(Owner.FileDirectory);
                                absPath = Path.GetFullPath(Path.Combine(Owner.FileDirectory, rel));
                                //fref.ReferencePathRelative = absPath.MakePathRelativeTo(_fileDir);
                            }
                            else
                                absPath = fref.ReferencePathAbsolute;

                            string dir = absPath.Contains(".") ? Path.GetDirectoryName(absPath) : absPath;

                            IFileObject file = fref.File;
                            if (file.FileExtension != null)
                            {
                                string fileName = SerializationCommon.ResolveFileName(
                                    Owner.FileDirectory, file.Name, file.FileExtension.GetProperExtension(EProprietaryFileFormat.XML));
                                await file.ExportAsync(dir, fileName, EFileFormat.XML, null, Owner.Flags, null, CancellationToken.None);
                            }
                            else
                            {
                                var f = file.File3rdPartyExtensions;
                                if (f != null && f.ExportableExtensions != null && f.ExportableExtensions.Length > 0)
                                {
                                    string ext = f.ExportableExtensions[0];
                                    string fileName = SerializationCommon.ResolveFileName(Owner.FileDirectory, file.Name, ext);
                                    await file.ExportAsync(dir, fileName, EFileFormat.ThirdParty, ext, Owner.Flags, null, CancellationToken.None);
                                }
                                else
                                    Engine.LogWarning("Cannot export " + file.GetType().GetFriendlyName());
                            }
                        }
                    }
                }
            }
        }
        public override string ToString() => MemberInfo.Name;

        protected internal abstract Task AddChildNodesAsync(int attribCount, int elementCount, int elementStringCount, List<T> members);
        Task IMemberTreeNode.AddChildNodesAsync(int attribCount, int elementCount, int elementStringCount, List<IMemberTreeNode> members)
            => AddChildNodesAsync(attribCount, elementCount, elementStringCount, members.Select(x => (T)x).ToList());

        protected internal abstract List<IMemberTreeNode> GetChildNodes(Type objectType);
        List<IMemberTreeNode> IMemberTreeNode.GetChildNodes(Type objectType) => GetChildNodes(objectType);

        public async Task GenerateObjectFromTreeAsync(Type objectType)
        {

        }
        async Task IMemberTreeNode.GenerateObjectFromTreeAsync(Type objectType) 
            => await GenerateObjectFromTreeAsync(objectType);
    }
}
