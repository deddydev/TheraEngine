using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using TheraEngine.Core.Tools;
using TheraEngine.Core.Files;
using System.Threading.Tasks;
using System.Threading;
using static TheraEngine.Core.Files.Serialization.TSerializer.WriterBinary;

namespace TheraEngine.Core.Files.Serialization
{
    public class BinaryMemberTreeNode : MemberTreeNode
    {
        public BinaryMemberTreeNode(object root, TSerializer.AbstractWriter writer) : base(root, writer) { }
        public BinaryMemberTreeNode(object obj, VarInfo memberInfo, TSerializer.AbstractWriter writer) : base(obj, memberInfo, writer) { }

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
                MethodInfo[] customMethods = ObjectType.GetMethods(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                    Where(x => x.GetCustomAttribute<CustomBinarySerializeSizeMethod>() != null).ToArray();

                int flagCount = 0;
                size += ObjectWriter.GetSize(customMethods, ref flagCount, table);
                size += (FlagSize = flagCount.Align(8) / 8); //Align to nearest byte
            }

            return CalculatedSize = size.Align(4);
        }

        protected internal override async Task CollectMemberInfo(MemberTreeNode member)
        {
            if (member.Object == null)
                return;

            TSerialize attrib = member?.MemberInfo?.Attrib;
            if (attrib != null)
            {
                if (attrib.State && !member.FormatWriter.Flags.HasFlag(ESerializeFlags.SerializeState))
                    return;
                if (attrib.Config && !member.FormatWriter.Flags.HasFlag(ESerializeFlags.SerializeConfig))
                    return;
            }

            MethodInfo customMethod = CustomMethods.FirstOrDefault(
                x => string.Equals(member.MemberInfo.Name, x.GetCustomAttribute<CustomSerializeMethod>().Name));
            
            ChildMembers = new List<MemberTreeNode>();

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

            ChildMembers.Add(member);
            await member.CollectSerializedMembers();
        }
    }
    public class XMLMemberTreeNode : MemberTreeNode
    {
        public XMLMemberTreeNode(object root, TSerializer.AbstractWriter writer) 
            : base(root, writer)
        {

        }
        public XMLMemberTreeNode(object obj, VarInfo memberInfo, TSerializer.AbstractWriter writer)
            : base(obj, memberInfo, writer)
        {

        }

        public string ElementName { get; set; }
        public List<(string Name, string Value)> Attributes { get; internal set; }
        public List<MemberTreeNode> ChildElements { get; internal set; }
        public string ChildStringData { get; internal set; }
        public int NonAttributeCount { get; private set; }

        protected internal override async Task CollectMemberInfo(MemberTreeNode childMember)
        {
            if (childMember.Object == null)
                return;

            TSerialize attrib = childMember?.MemberInfo?.Attrib;
            if (attrib != null)
            {
                if (attrib.State && !childMember.FormatWriter.Flags.HasFlag(ESerializeFlags.SerializeState))
                    return;
                if (attrib.Config && !childMember.FormatWriter.Flags.HasFlag(ESerializeFlags.SerializeConfig))
                    return;
            }

            MethodInfo customMethod = CustomMethods.FirstOrDefault(
                x => string.Equals(childMember.MemberInfo.Name, x.GetCustomAttribute<CustomSerializeMethod>().Name));

            ElementName = SerializationCommon.GetTypeName(MemberInfo.VariableType);
            Attributes = new List<(string Name, string Value)>();
            ChildElements = new List<MemberTreeNode>();
            ChildStringData = null;

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
                else
                {
                    Engine.LogWarning($"Method {customMethod.GetFriendlyName()} is marked with a CustomSerializeMethod attribute, but the arguments are not correct. There must be one argument of type MemberTreeNode.");
                }
            }

            if (attrib != null)
            {
                if (attrib.IsXmlElementString)
                {
                    if (SerializationCommon.GetString(childMember.Object, childMember.MemberInfo.VariableType, out string result))
                    {
                        if (NonAttributeCount == 1)
                            ChildStringData = result;
                        else
                            Attributes.Add((childMember.MemberInfo.Name, result));
                        return;
                    }
                    else
                        Engine.LogWarning(ObjectType.Name + " cannot be written as a string.");
                }
                else if (attrib.IsXmlAttribute)
                {
                    if (SerializationCommon.GetString(childMember.Object, ObjectType, out string result))
                    {
                        Attributes.Add((childMember.MemberInfo.Name, result));
                        return;
                    }
                    else
                        Engine.LogWarning(ObjectType.Name + " cannot be written as a string.");
                }
            }

            ChildElements.Add(childMember);
            await childMember.CollectSerializedMembers();
        }
    }
    public abstract class MemberTreeNode
    {
        /// <summary>
        /// The writer that handles what format this information is being written in,
        /// such as binary, XML, or JSON.
        /// </summary>
        public TSerializer.AbstractWriter FormatWriter { get; }
        /// <summary>
        /// All information pertaining to the definition of this member.
        /// </summary>
        public VarInfo MemberInfo { get; }
        /// <summary>
        /// The value assigned to this member.
        /// </summary>
        public object Object { get; }
        /// <summary>
        /// The type of the object assigned to this member.
        /// For the member's type, see MemberInfo.VariableType.
        /// </summary>
        public Type ObjectType { get; }
        /// <summary>
        /// <see langword="true"/> if the object's type inherits from the member's type instead of matching it exactly.
        /// </summary>
        public bool IsDerivedType { get; }
        /// <summary>
        /// The class handling how to collect all members of any given object.
        /// Most classes will use <see cref="CommonObjectWriter"/>.
        /// </summary>
        public BaseObjectWriter ObjectWriter { get; internal set; }
        /// <summary>
        /// Methods for serializing data in a specific manner.
        /// </summary>
        public IEnumerable<MethodInfo> CustomMethods { get; }

        public MemberTreeNode(object root, TSerializer.AbstractWriter writer)
            : this(root, root == null ? null : new VarInfo(root.GetType(), null), writer) { }
        public MemberTreeNode(object obj, VarInfo memberInfo, TSerializer.AbstractWriter writer)
        {
            Object = obj;
            MemberInfo = memberInfo;
            FormatWriter = writer;
            ObjectType = Object?.GetType();
            IsDerivedType = ObjectType != MemberInfo.VariableType;
            CustomMethods = ObjectType?.GetMethods(
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy).
                Where(x => x.GetCustomAttribute<CustomSerializeMethod>() != null);
            DetermineObjectWriter();
        }

        internal protected abstract Task CollectMemberInfo(MemberTreeNode member);
        
        private void DetermineObjectWriter()
        {
            Type baseObjWriterType = typeof(BaseObjectWriter);
            var types = Engine.FindTypes(type => 
                baseObjWriterType.IsAssignableFrom(type) && 
                type.GetCustomAttributeExt<ObjectWriterKind>() != null,
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
            if (MemberInfo == null)
                return;

            await FileObjectCheck();
            await ObjectWriter.CollectSerializedMembers();
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
        public override string ToString() => MemberInfo.Name;
    }
}
