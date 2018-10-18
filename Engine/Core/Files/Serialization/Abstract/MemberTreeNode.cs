using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Files.Serialization;

namespace TheraEngine.Core.Files.Serialization
{
    public class BinaryMemberTreeNode : MemberTreeNode<BinaryMemberTreeNode>
    {
        public BinaryMemberTreeNode(object obj)
            : base(obj) { }
        public BinaryMemberTreeNode(BinaryMemberTreeNode parent, MemberInfo memberInfo)
            : base(parent, memberInfo) { }

        public int CalculatedSize { get; internal set; }
        public List<BinaryMemberTreeNode> Children { get; internal set; }
        public byte[] ParsableBytes { get; internal set; } = null;
        public string ParsableString { get; internal set; } = null;
        
        protected override async Task OnAddChild(BinaryMemberTreeNode childMember)
        {
            Children.Add(childMember);
            await childMember.CollectSerializedMembers();
        }
        protected internal override async Task AddChildren(int attribCount, int elementCount, int elementStringCount, List<BinaryMemberTreeNode> members)
        {
            Children = new List<BinaryMemberTreeNode>(members.Count);
            foreach (BinaryMemberTreeNode member in members)
                await AddChild(member);
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

        protected override async Task OnAddChild(XMLMemberTreeNode childMember)
        {
            if (childMember != null)
            {
                if (childMember.NodeType == ENodeType.ChildElement)
                {
                    ChildElements.Add(childMember);
                    await childMember.CollectSerializedMembers();
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

        protected internal override async Task AddChildren(int attribCount, int elementCount, int elementStringCount, List<XMLMemberTreeNode> members)
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
                await AddChild(member);
                ProgressionCount += member.ProgressionCount;
            }
        }
        public void AddChildElementString(string elementName, string value)
        {
            XMLMemberTreeNode element = new XMLMemberTreeNode(null)
            {
                ElementName = elementName,
                NodeType = ENodeType.ChildElement,
                ElementString = value
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
        IEnumerable<MethodInfo> CustomMethods { get; }
        ENodeType NodeType { get; set; }
        IMemberTreeNode Parent { get; set; }
        bool ConfigMember { get; set; }
        bool StateMember { get; set; }
        object DefaultConstructedObject { get; }
        TBaseSerializer.IBaseAbstractReaderWriter Owner { get; }
        BaseObjectWriter ObjectWriter { get; set; }
        BaseObjectReader ObjectReader { get; set; }

        Task AddChildren(int attribCount, int elementCount, int elementStringCount, List<IMemberTreeNode> members);
    }
    public abstract class MemberTreeNode<T> : IMemberTreeNode where T : class, IMemberTreeNode
    {
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
                CustomMethods = ObjectType?.GetMethods(
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.FlattenHierarchy).
                    Where(x => x.GetCustomAttribute<CustomSerializeMethod>() != null);
                DetermineObjectWriter();
                //DefaultConstructedObject = SerializationCommon.CreateObject(ObjectType);
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
        public bool IsDerivedType => ObjectType?.IsSubclassOf(MemberType) ?? false;
        /// <summary>
        /// The class handling how to collect all members of any given object.
        /// Most classes will use <see cref="CommonWriter"/>.
        /// </summary>
        public BaseObjectWriter ObjectWriter { get; set; }
        /// <summary>
        /// The class handling how to collect all members of any given object.
        /// Most classes will use <see cref="CommonReader"/>.
        /// </summary>
        public BaseObjectReader ObjectReader { get; set; }
        /// <summary>
        /// Methods for serializing data in a specific manner.
        /// </summary>
        public IEnumerable<MethodInfo> CustomMethods { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public ENodeType NodeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public T Parent { get; internal set; }
        IMemberTreeNode IMemberTreeNode.Parent { get => Parent; set => Parent = value as T; }
        public bool ConfigMember { get; set; }
        public bool StateMember { get; set; }
        public object DefaultConstructedObject { get; private set; }

        public MemberTreeNode(object root)
        {
            Parent = null;
            MemberType = root?.GetType();
            Name = SerializationCommon.GetTypeName(MemberType);
            Category = null;
            Object = root;
            DefaultConstructedObject = SerializationCommon.CreateObject(ObjectType);
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
                    ConfigMember = attrib.Config;
                    StateMember = attrib.State;
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
        public async Task AddChild(T childMember)
        {
            childMember.Parent = this;

            if (childMember?.Object == null)
                return;
            if (childMember.Owner is TSerializer.IAbstractWriter writer)
            {
                if (childMember.StateMember && !writer.Flags.HasFlag(ESerializeFlags.SerializeState))
                    return;
                if (childMember.ConfigMember && !writer.Flags.HasFlag(ESerializeFlags.SerializeConfig))
                    return;
            }

            var customMethods = CustomMethods.Where(
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
                    Engine.LogWarning($"Method {customMethod.GetFriendlyName()} is marked with a CustomSerializeMethod attribute, but the arguments are not correct. There must be one argument of type {nameof(IMemberTreeNode)} or {nameof(XMLMemberTreeNode)}.");
                }
            }

            await OnAddChild(childMember);
        }
        protected abstract Task OnAddChild(T member);
        private void DetermineObjectWriter()
        {
            Type objType = ObjectType;
            BaseObjectWriter writer = null;

            Type baseObjWriterType = typeof(BaseObjectWriter);
            var types = Engine.FindTypes(type =>
                baseObjWriterType.IsAssignableFrom(type) &&
                (type.GetCustomAttributeExt<ObjectWriterKind>()?.ObjectType?.IsAssignableFrom(objType) ?? false),
            true, null).ToArray();

            if (types.Length > 0)
                writer = (BaseObjectWriter)Activator.CreateInstance(types[0]);
            else
                writer = new CommonWriter();

            writer.TreeNode = this;
            ObjectWriter = writer;
        }
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
        public async Task CollectSerializedMembers()
        {
            await FileObjectCheck();
            if (ObjectWriter != null)
                await ObjectWriter.CollectSerializedMembers();
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
                return;
            }

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
                    Engine.LogWarning("Can't read property '" + info.Name + "' in " + info.DeclaringType.GetFriendlyName());
            }
            else
            {
                Engine.LogWarning($"Member {memberInfo.Name} is not a field or property.");
            }
        }
        /// <summary>
        /// Performs special processing for classes that implement <see cref="IFileObject"/> and <see cref="IFileRef"/>.
        /// </summary>
        private async Task FileObjectCheck()
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
                            if (fref is IGlobalFileRef && !FormatWriter.Flags.HasFlag(ESerializeFlags.ExportGlobalRefs))
                                return;
                            if (fref is ILocalFileRef && !FormatWriter.Flags.HasFlag(ESerializeFlags.ExportLocalRefs))
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
        Task IMemberTreeNode.AddChildren(int attribCount, int elementCount, int elementStringCount, List<IMemberTreeNode> members)
            => AddChildren(attribCount, elementCount, elementStringCount, members.Select(x => (T)x).ToList());
        protected internal abstract Task AddChildren(int attribCount, int elementCount, int elementStringCount, List<T> members);
    }
}
