using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using static TheraEngine.Core.Files.Serialization.TSerializer.WriterBinary;

namespace TheraEngine.Core.Files.Serialization
{
    public class BinaryMemberTreeNode : MemberTreeNode
    {
        public BinaryMemberTreeNode(object root, TSerializer.AbstractWriter<BinaryMemberTreeNode> writer)
            : base(root, writer) { }
        public BinaryMemberTreeNode(BinaryMemberTreeNode parent, MemberInfo memberInfo, TSerializer.AbstractWriter<BinaryMemberTreeNode> writer)
            : base(parent, memberInfo, writer) { }

        public int CalculatedSize { get; internal set; }
        public int FlagSize { get; internal set; }
        public List<MemberTreeNode> ChildMembers { get; internal set; }

        public int GetSize(BinaryStringTable table)
        {
            if (Object == null)
                return 0;

            int size = 0;
            
            if (ObjectType.IsValueType)
                size += Marshal.SizeOf(ObjectType);
            else
            {
                MethodInfo[] customSizeMethods = ObjectType.GetMethods(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                    Where(x => x.GetCustomAttribute<CustomBinarySerializeSizeMethod>() != null).ToArray();

                int flagCount = 0;
                //size += ObjectWriter.GetSize(customSizeMethods, ref flagCount, table);
                size += (FlagSize = flagCount.Align(8) / 8); //Align to nearest byte
            }

            return CalculatedSize = size.Align(4);
        }

        protected internal override async Task AddChild(MemberTreeNode childMember)
        {
            childMember.Parent = this;

            if (childMember?.Object == null)
                return;

            TSerialize attrib = childMember.Attrib;
            if (attrib != null)
            {
                if (attrib.State && !childMember.FormatWriter.Flags.HasFlag(ESerializeFlags.SerializeState))
                    return;
                if (attrib.Config && !childMember.FormatWriter.Flags.HasFlag(ESerializeFlags.SerializeConfig))
                    return;
            }

            MethodInfo customMethod = CustomMethods.FirstOrDefault(
                x => string.Equals(childMember.Name, x.GetCustomAttribute<CustomSerializeMethod>().Name));
            
            if (customMethod != null)
            {
                var parameters = customMethod.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(GetType()))
                {
                    if (customMethod.ReturnType == typeof(Task))
                        await (Task)customMethod.Invoke(Object, new object[] { this });
                    else
                        customMethod.Invoke(Object, new object[] { this });
                    return;
                }
            }

            ChildMembers.Add(childMember);
            await childMember.CollectSerializedMembers();
        }
        protected internal override async Task AddChildren(int attribCount, int elementCount, int elementStringCount, List<MemberTreeNode> members)
        {
            ChildMembers = new List<MemberTreeNode>(members.Count);
            foreach (MemberTreeNode member in members)
                await AddChild(member);
        }
    }
    public class XMLMemberTreeNode : MemberTreeNode
    {
        public XMLMemberTreeNode(object root, TSerializer.AbstractWriter<XMLMemberTreeNode> writer)
            : base(root, writer) { }
        public XMLMemberTreeNode(XMLMemberTreeNode parent, MemberInfo memberInfo, TSerializer.AbstractWriter<XMLMemberTreeNode> writer)
            : base(parent, memberInfo, writer) { }

        public List<(string Name, string Value)> Attributes { get; internal set; }
        public List<MemberTreeNode> ChildElements { get; internal set; }
        public string ChildStringData { get; internal set; }
        public int NonAttributeCount { get; private set; }

        protected internal override async Task AddChild(MemberTreeNode childMember)
        {
            childMember.Parent = this;

            if (childMember?.Object == null)
                return;
            
            TSerialize attrib = childMember.Attrib;
            if (attrib != null)
            {
                if (attrib.State && !childMember.FormatWriter.Flags.HasFlag(ESerializeFlags.SerializeState))
                    return;
                if (attrib.Config && !childMember.FormatWriter.Flags.HasFlag(ESerializeFlags.SerializeConfig))
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
                    Engine.LogWarning($"Method {customMethod.GetFriendlyName()} is marked with a CustomSerializeMethod attribute, but the arguments are not correct. There must be one argument of type {nameof(MemberTreeNode)} or {nameof(XMLMemberTreeNode)}.");
                }
            }
            
            if (childMember != null)
            {
                if (childMember.NodeType == ENodeType.ChildElement)
                {
                    ChildElements.Add(childMember);
                    await childMember.CollectSerializedMembers();
                }
                else
                {
                    if (SerializationCommon.GetString(childMember.Object, childMember.MemberType, out string result))
                    {
                        if (childMember.NodeType == ENodeType.ElementString && NonAttributeCount == 1)
                            ChildStringData = result;
                        else
                            Attributes.Add((childMember.Name, result));
                    }
                    else
                        Engine.LogWarning(ObjectType.Name + " cannot be written as a string.");
                }
            }
        }

        protected internal override async Task AddChildren(int attribCount, int elementCount, int elementStringCount, List<MemberTreeNode> members)
        {
            NonAttributeCount = elementCount + elementStringCount;
            Attributes = new List<(string Name, string Value)>(attribCount);
            ChildElements = new List<MemberTreeNode>(elementCount);
            ChildStringData = null;
            ProgressionCount = 1 + attribCount + elementCount + elementStringCount;
            if (IsDerivedType)
                ++ProgressionCount;
            foreach (MemberTreeNode member in members)
            {
                await AddChild(member);
                ProgressionCount += member.ProgressionCount;
            }
        }
    }
    public abstract class MemberTreeNode
    {
        public int ProgressionCount { get; set; }
        public string ElementName { get; set; }
        /// <summary>
        /// The writer that handles what format this information is being written in,
        /// such as binary, XML, or JSON.
        /// </summary>
        public TSerializer.BaseAbstractWriter FormatWriter { get; }
        public Type MemberType { get; set; }
        public string Name { get; set; }
        public string Category { get; set; } = null;
        public TSerialize Attrib { get; set; }
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
            }
        }
        private object _object;
        /// <summary>
        /// The type of the object assigned to this member.
        /// For the member's type, see MemberInfo.VariableType.
        /// </summary>
        public Type ObjectType => _object?.GetType() ?? MemberType;
        /// <summary>
        /// <see langword="true"/> if the object's type inherits from the member's type instead of matching it exactly.
        /// </summary>
        public bool IsDerivedType => ObjectType?.IsSubclassOf(MemberType) ?? false;
        /// <summary>
        /// The class handling how to collect all members of any given object.
        /// Most classes will use <see cref="CommonObjectWriter"/>.
        /// </summary>
        public BaseObjectWriter ObjectWriter { get; internal set; }
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
        public MemberTreeNode Parent { get; internal set; }

        public MemberTreeNode(object root, TSerializer.BaseAbstractWriter writer)
        {
            Parent = null;
            FormatWriter = writer;
            Attrib = null;
            Name = null;
            Category = null;
            MemberType = root?.GetType();
            ElementName = SerializationCommon.GetTypeName(MemberType);
            Object = root;
        }
        public MemberTreeNode(MemberTreeNode parent, MemberInfo memberInfo, TSerializer.BaseAbstractWriter writer)
        {
            Parent = parent;
            FormatWriter = writer;

            if (memberInfo != null)
            {
                Attrib = memberInfo.GetCustomAttribute<TSerialize>();
                if (Attrib != null)
                {
                    if (Attrib.NameOverride != null)
                        Name = Attrib.NameOverride;
                    else
                        Name = memberInfo.Name;

                    if (Attrib.UseCategory)
                    {
                        if (Attrib.OverrideCategory != null)
                            Category = SerializationCommon.FixElementName(Attrib.OverrideCategory);
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

                ElementName = SerializationCommon.GetTypeName(MemberType);

                GetObject(memberInfo);
            }
        }

        internal protected abstract Task AddChild(MemberTreeNode member);
        
        private void DetermineObjectWriter()
        {
            Type baseObjWriterType = typeof(BaseObjectWriter);
            var types = Engine.FindTypes(type => 
                baseObjWriterType.IsAssignableFrom(type) && 
                (type.GetCustomAttributeExt<ObjectWriterKind>()?.ObjectType?.IsAssignableFrom(ObjectType) ?? false),
            true, null).ToArray();

            BaseObjectWriter writer;
            if (types.Length > 0)
                writer = (BaseObjectWriter)Activator.CreateInstance(types[0]);
            else
                writer = new CommonObjectWriter();
            
            writer.TreeNode = this;
            ObjectWriter = writer;
        }
        public async Task CollectSerializedMembers()
        {
            await FileObjectCheck();
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
        private void GetObject(MemberInfo memberInfo)
        {
            if (Parent.Object is null)
                return;

            if (memberInfo.MemberType.HasFlag(MemberTypes.Field))
                Object = ((FieldInfo)memberInfo).GetValue(Parent.Object);
            else if (memberInfo.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfo p = (PropertyInfo)memberInfo;
                if (p.CanRead)
                    Object = p.GetValue(Parent.Object);
                else
                    Engine.LogWarning("Can't read property '" + p.Name + "' in " + p.DeclaringType.GetFriendlyName());
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
                fobj.FilePath = FormatWriter.FilePath;
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

                        string root2 = Path.GetPathRoot(FormatWriter.FileDirectory);
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
                                string rel = fref.ReferencePathAbsolute.MakePathRelativeTo(FormatWriter.FileDirectory);
                                absPath = Path.GetFullPath(Path.Combine(FormatWriter.FileDirectory, rel));
                                //fref.ReferencePathRelative = absPath.MakePathRelativeTo(_fileDir);
                            }
                            else
                                absPath = fref.ReferencePathAbsolute;

                            string dir = absPath.Contains(".") ? Path.GetDirectoryName(absPath) : absPath;

                            IFileObject file = fref.File;
                            if (file.FileExtension != null)
                            {
                                string fileName = SerializationCommon.ResolveFileName(
                                    FormatWriter.FileDirectory, file.Name, file.FileExtension.GetProperExtension(EProprietaryFileFormat.XML));
                                await file.ExportAsync(dir, fileName, EFileFormat.XML, null, FormatWriter.Flags, null, CancellationToken.None);
                            }
                            else
                            {
                                var f = file.File3rdPartyExtensions;
                                if (f != null && f.ExportableExtensions != null && f.ExportableExtensions.Length > 0)
                                {
                                    string ext = f.ExportableExtensions[0];
                                    string fileName = SerializationCommon.ResolveFileName(FormatWriter.FileDirectory, file.Name, ext);
                                    await file.ExportAsync(dir, fileName, EFileFormat.ThirdParty, ext, FormatWriter.Flags, null, CancellationToken.None);
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
        protected internal abstract Task AddChildren(int attribCount, int elementCount, int elementStringCount, List<MemberTreeNode> members);
    }
}
