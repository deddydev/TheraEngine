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
    public class MemberTreeNode
    {
        public TSerializer.AbstractWriter Writer { get; }
        public object Object { get; }
        public VarInfo Info { get; }

        public int CalculatedSize { get; internal set; }
        public int FlagSize { get; internal set; }
        
        public BaseObjectWriter ObjectWriter { get; private set; }
        
        public MemberTreeNode(object root, TSerializer.AbstractWriter writer)
            : this(root, root == null ? null : new VarInfo(root.GetType(), null), writer) { }
        public MemberTreeNode(object obj, VarInfo info, TSerializer.AbstractWriter writer)
        {
            Object = obj;
            Info = info;
            Writer = writer;
            DetermineInterface();
        }
        private void DetermineInterface()
        {
            Type t = typeof(BaseObjectWriter);
            var types = Engine.FindTypes(x => t.IsAssignableFrom(x) && x.GetCustomAttributeExt<ObjectWriterKind>() != null, true, null).ToArray();
            if (types.Length > 0)
            {
                t = types[0];
                BaseObjectWriter writer = (BaseObjectWriter)Activator.CreateInstance(t);
                writer.Object = Object;
                writer.Writer = Writer;
                writer.Info = Info;
                ObjectWriter = writer;
                writer.Initialize();
            }
        }
        public async Task GenerateChildTree()
        {
            if (Info == null)
                return;

            await FileObjectCheck();
            await ObjectWriter.GenerateChildTree();
        }
        /// <summary>
        /// Performs special processing for classes that implement <see cref="IFileObject"/> and <see cref="IFileRef"/>.
        /// </summary>
        private async Task FileObjectCheck()
        {
            //Update the object's file path
            if (Object is IFileObject fobj)
            {
                fobj.FilePath = Writer.FilePath;
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

                        string root2 = Path.GetPathRoot(Writer.FileDirectory);
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
                            if (fref is IGlobalFileRef && !Writer.Flags.HasFlag(ESerializeFlags.ExportGlobalRefs))
                                return;
                            if (fref is ILocalFileRef && !Writer.Flags.HasFlag(ESerializeFlags.ExportLocalRefs))
                                return;

                            string absPath;
                            if (fref.PathType == EPathType.FileRelative)
                            {
                                string rel = fref.ReferencePathAbsolute.MakePathRelativeTo(Writer.FileDirectory);
                                absPath = Path.GetFullPath(Path.Combine(Writer.FileDirectory, rel));
                                //fref.ReferencePathRelative = absPath.MakePathRelativeTo(_fileDir);
                            }
                            else
                                absPath = fref.ReferencePathAbsolute;

                            string dir = absPath.Contains(".") ? Path.GetDirectoryName(absPath) : absPath;

                            IFileObject file = fref.File;
                            if (file.FileExtension != null)
                            {
                                string fileName = SerializationCommon.ResolveFileName(
                                    Writer.FileDirectory, file.Name, file.FileExtension.GetProperExtension(EProprietaryFileFormat.XML));
                                await file.ExportAsync(dir, fileName, EFileFormat.XML, null, Writer.Flags, null, CancellationToken.None);
                            }
                            else
                            {
                                var f = file.File3rdPartyExtensions;
                                if (f != null && f.ExportableExtensions != null && f.ExportableExtensions.Length > 0)
                                {
                                    string ext = f.ExportableExtensions[0];
                                    string fileName = SerializationCommon.ResolveFileName(Writer.FileDirectory, file.Name, ext);
                                    await file.ExportAsync(dir, fileName, EFileFormat.ThirdParty, ext, Writer.Flags, null, CancellationToken.None);
                                }
                                else
                                    Engine.LogWarning("Cannot export " + file.GetType().GetFriendlyName());
                            }
                        }
                    }
                }
            }
        }
        public int GetSize(BinaryStringTable table)
        {
            if (Object == null)
                return 0;

            int size = 0;

            Type t = Info.VariableType;
            if (t.IsValueType)
                size += Marshal.SizeOf(t);
            else
            {
                MethodInfo[] customMethods = Info.VariableType.GetMethods(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                    Where(x => x.GetCustomAttribute<CustomBinarySerializeSizeMethod>() != null).ToArray();

                int flagCount = 0;
                size += ObjectWriter.GetSize(customMethods, ref flagCount, table);
                size += (FlagSize = flagCount.Align(8) / 8);
            }

            return CalculatedSize = size.Align(4);
        }
        public override string ToString() => Info.Name;
    }
}
