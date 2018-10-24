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
        public BinaryMemberTreeNode(BinaryMemberTreeNode parent, MemberInfo memberInfo)
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
        protected internal override async Task AddChildrenAsync(int attribCount, int elementCount, int elementStringCount, List<BinaryMemberTreeNode> members)
        {
            Children = new List<BinaryMemberTreeNode>(members.Count);
            foreach (BinaryMemberTreeNode member in members)
                await AddChildAsync(member);
        }
        protected internal override List<IMemberTreeNode> RetrieveChildren(out int elementCount, out int attributeCount, out bool elementString)
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
        public XMLMemberTreeNode(XMLMemberTreeNode parent, MemberInfo memberInfo)
            : base(parent, memberInfo) { }
        public XMLMemberTreeNode(string name, string elementString)
            : base(null)
        {
            Name = name;
            ElementString = elementString;
            NodeType = ENodeType.ChildElement;
        }
        public XMLMemberTreeNode(string name, XMLAttribute[] attributes)
            : base(null)
        {
            Name = name;
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

        protected internal override async Task AddChildrenAsync(int attribCount, int elementCount, int elementStringCount, List<XMLMemberTreeNode> members)
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
        protected internal override List<IMemberTreeNode> RetrieveChildren(out int elementCount, out int attributeCount, out bool elementString)
        {
            List<IMemberTreeNode> nodes = new List<IMemberTreeNode>();
            foreach (var attrib in Attributes)
            {
                string name = attrib.Name;
                string val = attrib.Value;
                XMLMemberTreeNode attribNode = new XMLMemberTreeNode(null)
                {
                    NodeType = ENodeType.Attribute,
                };
                nodes.Add(attribNode);
            }
            //return nodes;
        }
        public void AddChildElementString(string elementName, string value)
        {
            XMLMemberTreeNode element = new XMLMemberTreeNode(elementName, value)
            {
                NodeType = ENodeType.ChildElement,
            };
        }

        public XMLAttribute GetAttribute(string name)
        {
            return Attributes.Find(x => x.Name == name);
        }
        public XMLMemberTreeNode GetChildElement(string elementName)
        {
            return ChildElements.Find(x => x.ElementName == elementName);
        }
        public void AddAttribute(string name, string value)
        {
            Attributes.Add(new XMLAttribute(name, value));
        }
    }
    public interface IMemberTreeNode
    {
        ESerializeType SerializeType { get; }
        int Order { get; set; }
        int ProgressionCount { get; }
        string ElementName { get; set; }
        Type MemberType { get; set; }
        string Name { get; set; }
        string Category { get; set; }
        object Object { get; set; }
        Type ObjectType { get; }
        bool IsDerivedType { get; }
        ENodeType NodeType { get; set; }
        IMemberTreeNode Parent { get; set; }
        bool IsConfigMember { get; set; }
        bool IsStateMember { get; set; }
        object DefaultConstructedObject { get; }
        TBaseSerializer.IBaseAbstractReaderWriter Owner { get; }
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
        Task AddChildrenAsync(int attribCount, int elementCount, int elementStringCount, List<IMemberTreeNode> members);
        List<IMemberTreeNode> RetrieveChildren(out int elementCount, out int attributeCount, out bool elementString);

        void GenerateObjectFromTree(Type objectType);
        Task CreateTreeFromObjectAsync();
    }
    public abstract class MemberTreeNode<T> : IMemberTreeNode where T : class, IMemberTreeNode
    {
        public MemberTreeNode() { }

        public bool IsGroupingNode { get; set; } = true;
        public ESerializeType SerializeType { get; private set; }
        public int Order { get; set; }
        /// <summary>
        /// How many checkpoints need to be hit for this node and all child nodes to advance the progression handler.
        /// </summary>
        public int ProgressionCount { get; internal set; }
        /// <summary>
        /// Sets the name of this element.
        /// </summary>
        public string ElementName
        {
            get => _elementName ?? Name;
            set => _elementName = value;
        }
        private string _elementName = null;

        public TBaseSerializer.IBaseAbstractReaderWriter Owner { get; internal set; }

        /// <summary>
        /// The type that the class defines this member as.
        /// </summary>
        public Type MemberType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The category name for this member.
        /// </summary>
        public string Category { get; set; } = null;
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
        private object _object;
        /// <summary>
        /// The type of the object assigned to this member.
        /// </summary>
        public Type ObjectType => _object?.GetType() ?? MemberType;
        /// <summary>
        /// <see langword="true"/> if the object's type inherits from the member's type instead of matching it exactly.
        /// </summary>
        public bool IsDerivedType => Parent == null || (ObjectType?.IsSubclassOf(MemberType) ?? false);
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
        
        /// <summary>
        /// 
        /// </summary>
        public ENodeType NodeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public T Parent { get; internal set; }
        IMemberTreeNode IMemberTreeNode.Parent { get => Parent; set => Parent = value as T; }
        public bool IsConfigMember { get; set; }
        public bool IsStateMember { get; set; }
        public object DefaultConstructedObject { get; private set; }

        public MemberTreeNode(object obj)
        {
            Parent = null;
            MemberType = obj?.GetType();
            Name = SerializationCommon.GetTypeName(MemberType);
            Category = null;
            Object = obj;
            DefaultConstructedObject = ObjectType == null ? null : SerializationCommon.CreateObject(ObjectType);
            IsGroupingNode = obj == null;
        }
        public MemberTreeNode(T parent, MemberInfo memberInfo)
        {
            Parent = parent;

            if (memberInfo != null)
            {
                TSerialize attrib = memberInfo.GetCustomAttribute<TSerialize>();
                if (attrib != null)
                {
                    Order = attrib.Order;
                    IsConfigMember = attrib.Config;
                    IsStateMember = attrib.State;
                    NodeType = attrib.NodeType;

                    if (attrib.NameOverride != null)
                        Name = attrib.NameOverride;
                    else
                        Name = memberInfo.Name;

                    if (attrib.UseCategory)
                    {
                        if (attrib.OverrideCategory != null)
                            Category = SerializationCommon.FixElementName(attrib.OverrideCategory);
                        else
                        {
                            CategoryAttribute categoryAttrib = memberInfo.GetCustomAttribute<CategoryAttribute>();
                            if (categoryAttrib != null)
                                Category = SerializationCommon.FixElementName(categoryAttrib.Category);
                            else
                                Category = null;
                        }
                    }
                    else
                        Category = null;
                }
                else
                {
                    Name = memberInfo.Name;
                    Category = null;
                }

                Name = new string(Name.Where(x => !char.IsWhiteSpace(x)).ToArray());

                if (memberInfo.MemberType.HasFlag(MemberTypes.Field))
                {
                    FieldInfo info = (FieldInfo)memberInfo;
                    MemberType = info.FieldType;
                }
                else if (memberInfo.MemberType.HasFlag(MemberTypes.Property))
                {
                    PropertyInfo info = (PropertyInfo)memberInfo;
                    MemberType = info.PropertyType;
                }
                
                GetObject(memberInfo);
            }
        }

        public async Task AddChildAsync(T childMember)
        {
            childMember.Parent = this;

            if (childMember?.Object == null)
                return;
            if (childMember.Owner is TSerializer.IAbstractWriter writer)
            {
                if (childMember.IsStateMember && !writer.Flags.HasFlag(ESerializeFlags.SerializeState))
                    return;
                if (childMember.IsConfigMember && !writer.Flags.HasFlag(ESerializeFlags.SerializeConfig))
                    return;
            }

            var customMethods = CustomSerializeMethods.Where(
                x => string.Equals(childMember.Name, x.GetCustomAttribute<CustomSerializeMethod>().Name));

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
        public void GetObject(MemberInfo memberInfo)
        {
            DefaultConstructedObject = null;
            if (Parent.Object is null || memberInfo is null)
            {
                Object = null;
                IsGroupingNode = true;
                return;
            }

            IsGroupingNode = false;

            if (memberInfo.MemberType.HasFlag(MemberTypes.Field))
            {
                FieldInfo info = (FieldInfo)memberInfo;
                Object = info.GetValue(Parent.Object);
                if (!(Parent.DefaultConstructedObject is null))
                    DefaultConstructedObject = info.GetValue(Parent.DefaultConstructedObject);
            }
            else if (memberInfo.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfo info = (PropertyInfo)memberInfo;
                if (info.CanRead)
                {
                    Object = info.GetValue(Parent.Object);
                    if (!(Parent.DefaultConstructedObject is null))
                        DefaultConstructedObject = info.GetValue(Parent.DefaultConstructedObject);
                }
                else
                {
                    Engine.LogWarning("Can't read property '" + info.Name + "' in " + info.DeclaringType.GetFriendlyName());
                }
            }
            else
            {
                Engine.LogWarning($"Member {memberInfo.Name} is not a field or property.");
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
        public override string ToString() => Name;
        Task IMemberTreeNode.AddChildrenAsync(int attribCount, int elementCount, int elementStringCount, List<IMemberTreeNode> members)
            => AddChildrenAsync(attribCount, elementCount, elementStringCount, members.Select(x => (T)x).ToList());
        protected internal abstract Task AddChildrenAsync(int attribCount, int elementCount, int elementStringCount, List<T> members);
        List<IMemberTreeNode> IMemberTreeNode.RetrieveChildren(out int elementCount, out int attributeCount, out bool elementString)
            => RetrieveChildren(out elementCount, out attributeCount, out elementString);
        protected internal abstract List<IMemberTreeNode> RetrieveChildren(out int elementCount, out int attributeCount, out bool elementString);
        public void GenerateObjectFromTree(Type objectType)
        {

        }
    }
}
