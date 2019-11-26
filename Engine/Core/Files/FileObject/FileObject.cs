using Extensions;
using SevenZip;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Reflection.Attributes;
using static TheraEngine.Core.Files.Serialization.Deserializer.ReaderBinary;
using static TheraEngine.Core.Files.Serialization.Serializer.WriterBinary;

namespace TheraEngine.Core.Files
{
    /// <summary>
    /// Base class for classes that can be stored as files.
    /// </summary>
    //[FileExt("tasset")]
    //[FileDef("Thera Engine Asset")]
    //[Serializable]
    public abstract partial class TFileObject : TObject, IFileObject
    {
        [Browsable(false)]
        public TFileDef FileDefinition => GetFileDefinition(GetType());
        [Browsable(false)]
        public TFileExt FileExtension => GetFileExtension(GetType());
        [Browsable(false)]
        public TFile3rdPartyExt File3rdPartyExtensions => GetFile3rdPartyExtensions(GetType());

        public delegate T Del3rdPartyImportFileMethod<T>(string path) where T : IFileObject;
        public delegate Task<T> Del3rdPartyImportFileMethodAsync<T>(string path, IProgress<float> progress, CancellationToken cancel) where T : IFileObject;

        public delegate void Del3rdPartyExportFileMethod<T>(T obj, string path) where T : IFileObject;
        public delegate Task Del3rdPartyExportFileMethodAsync<T>(T obj, string path, IProgress<float> progress, CancellationToken cancel) where T : IFileObject;

        public TFileObject() { }

        private TFileObject _rootFile = null;
        private string _filePath;

        [Browsable(false)]
        [TString(false, true, false)]
        [Category("Object")]
        public virtual string FilePath
        {
            get
            {
                if (_rootFile is null || _rootFile == this)
                    return _filePath;

                return _rootFile.FilePath;
            }
            set => _filePath = value.IsValidPath() ? Path.GetFullPath(value) : value;
        }
        [Browsable(false)]
        [TString(false, true, false)]
        [Category("Object")]
        public string Original3rdPartyPath { get; set; }
        [Browsable(false)]
        public string DirectoryPath => !string.IsNullOrEmpty(FilePath) && FilePath.IsValidPath() ? Path.GetDirectoryName(FilePath) : string.Empty;
        [Browsable(false)]
        public List<IFileRef> References { get; set; } = new List<IFileRef>();

        [Browsable(false)]
        public IFileObject RootFile
        {
            get => _rootFile ?? this;
            set => _rootFile = value as TFileObject;
        }

        public void Unload()
        {
            List<IFileRef> oldRefs = new List<IFileRef>(References);
            foreach (IFileRef r in oldRefs)
                r.UnloadReference();
            OnUnload();
        }
        protected virtual void OnUnload() { }
        
        /// <summary>
        /// Creates the full file path to a file given separate parameters.
        /// </summary>
        /// <param name="dir">The path to the folder the file resides in.</param>
        /// <param name="name">The name of the file.</param>
        /// <param name="format">The format the data is written in.</param>
        /// <returns>An absolute path to the file.</returns>
        public string GetFilePath(string dir, string name, EProprietaryFileFormat format)
            => GetFilePath(dir, name, format, GetType());
        public string GetFilter(
            bool proprietary = true,
            bool thirdParty = true,
            bool import3rdParty = false, 
            bool export3rdParty = false)
            => GetFilter(GetType(), proprietary, thirdParty, import3rdParty, export3rdParty);

        #region Export Sync

        public async void Export()
            => await ExportAsync(ESerializeFlags.Default);

        public async void Export(ESerializeFlags flags) 
            => await ExportAsync(flags);
        public async void Export(ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
            => await ExportAsync(flags, progress, cancel);

        public async void Export(string path, ESerializeFlags flags = ESerializeFlags.Default)
            => await ExportAsync(path, flags);
        public async void Export(string path, IProgress<float> progress, CancellationToken cancel)
            => await ExportAsync(FilePath, ESerializeFlags.Default, progress, cancel);
        public async void Export(string path, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
            => await ExportAsync(FilePath, flags, progress, cancel);

        public async void Export(string directory, string fileName, ESerializeFlags flags = ESerializeFlags.Default, EProprietaryFileFormat? propFormat = null)
            => await ExportAsync(directory, fileName, flags);
        public async void Export(string directory, string fileName, ESerializeFlags flags, EProprietaryFileFormat? propFormat, IProgress<float> progress, CancellationToken cancel)
            => await ExportAsync(directory, fileName, flags, propFormat, progress, cancel);
        public async void Export(string directory, string fileName, ESerializeFlags flags, EFileFormat format, string thirdPartyExt, IProgress<float> progress, CancellationToken cancel)
            => await ExportAsync(directory, fileName, flags, format, thirdPartyExt, progress, cancel);

        #endregion

        #region Export Async

        public async Task ExportAsync() 
            => await ExportAsync(ESerializeFlags.Default);

        public async Task ExportAsync(ESerializeFlags flags)
            => await ExportAsync(FilePath, flags);

        public async Task ExportAsync(
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel)
            => await ExportAsync(FilePath, flags, progress, cancel);

        public async Task ExportAsync(
            string path,
            ESerializeFlags flags = ESerializeFlags.Default)
        {
            if (Engine.BeginOperation is null)
                await ExportAsync(path, flags, null, CancellationToken.None);
            else
            {
                int op = Engine.BeginOperation($"Exporting file to {path}...", $"{path} exported successfully.", out Progress<float> progress, out CancellationTokenSource cancel);
                await ExportAsync(path, flags, progress, cancel.Token);
                if (Engine.EndOperation != null)
                    Engine.EndOperation(op);
                else
                    ((IProgress<float>)progress).Report(1.0f);
            }
        }
        public async Task ExportAsync(
            string path,
            IProgress<float> progress, 
            CancellationToken cancel)
            => await ExportAsync(path, ESerializeFlags.Default, progress, cancel);
        public async Task ExportAsync(
            string path,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            if (string.IsNullOrEmpty(path))
            {
                Engine.LogWarning("File was not exported; file path is not valid.");
                return;
            }

            Type type = GetType();
            TFileExt extAttrib = FileExtension;
            TFile3rdPartyExt tpAttrib;

            GetDirNameFmt(path, out string dir, out string name, out EFileFormat pathFormat, out string ext);

            if (extAttrib != null && pathFormat != EFileFormat.ThirdParty)
            {
                ext = extAttrib.GetFullExtension((EProprietaryFileFormat)(int)pathFormat);
                await ExportAsync(dir, name, flags, pathFormat, ext, progress, cancel);
                return;
            }
            else if ((tpAttrib = GetFile3rdPartyExtensions(type)) != null)
            {
                bool hasWildcard = tpAttrib.HasExtension("*", true);
                bool hasExt = tpAttrib.HasExtension(ext, false);
                if (!hasWildcard && !hasExt)
                {
                    //if (tpAttrib.Extensions.Length > 0)
                    //{
                    //    ext = tpAttrib.Extensions[0];
                    //}
                    //else
                    //{

                    //}
                }
                else
                {
                    await ExportAsync(dir, name, flags, EFileFormat.ThirdParty, ext, progress, cancel);
                    return;
                }
            }

            Engine.LogWarning($"{type.GetFriendlyName()} cannot be exported with extension '{ext}'.");
        }

        public async Task ExportAsync(
            string directory,
            string fileName,
            EProprietaryFileFormat? propFormat = null)
            => await ExportAsync(directory, fileName, ESerializeFlags.Default, propFormat);

        public async Task ExportAsync(
            string directory,
            string fileName,
            ESerializeFlags flags = ESerializeFlags.Default,
            EProprietaryFileFormat? propFormat = null)
        {
            if (Engine.BeginOperation is null)
                await ExportAsync(directory, fileName, flags, propFormat, null, CancellationToken.None);
            else
            {
                int op = Engine.BeginOperation($"Exporting file to {FilePath}...", $"{FilePath} exported successfully.", out Progress<float> progress, out CancellationTokenSource cancel);
                await ExportAsync(directory, fileName, flags, propFormat, progress, cancel.Token);
                if (Engine.EndOperation != null)
                    Engine.EndOperation(op);
                else
                    ((IProgress<float>)progress).Report(1.0f);
            }
        }
        public async Task ExportAsync(
            string directory,
            string fileName,
            ESerializeFlags flags,
            EProprietaryFileFormat? propFormat,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            string ext = null;
            TFileExt fileExt = FileExtension;
            if (fileExt != null)
                ext = fileExt.GetFullExtension(propFormat ?? fileExt.PreferredFormat);
            else
            {
                TFile3rdPartyExt tp = File3rdPartyExtensions;
                if (tp?.HasAnyExtensions ?? false)
                    ext = tp.Extensions[0];
            }
            if (ext != null)
            {
                EFileFormat format = GetFormat(ext, out _);
                await ExportAsync(directory, fileName, flags, format, ext, progress, cancel);
            }
            else
                Engine.LogWarning($"File was not exported; cannot resolve extension for {GetType().GetFriendlyName()}.");
        }
        public async Task ExportAsync(
            string directory,
            string fileName,
            ESerializeFlags flags,
            EFileFormat format,
            string thirdPartyExt,
            IProgress<float> progress, 
            CancellationToken cancel)
        {
            switch (format)
            {
                case EFileFormat.ThirdParty:
                    await Export3rdPartyAsync(directory, fileName, thirdPartyExt, progress, cancel);
                    break;
                case EFileFormat.Binary:
                    //await ExportBinaryAsync(directory, fileName, flags, progress, cancel);
                    //break;
                case EFileFormat.XML:
                    await ExportXMLAsync(directory, fileName, flags, progress, cancel);
                    break;
                default:
                    throw new InvalidOperationException("Not a valid file format.");
            }
        }

        public async Task ExportXMLAsync(
            string directory,
            string fileName,
            ESerializeFlags flags, 
            IProgress<float> progress,
            CancellationToken cancel)
            => await Serializer.ExportXMLAsync(this, directory, fileName, flags, progress, cancel);
        
        public async Task ExportBinaryAsync(
            string directory,
            string fileName,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel,
            Endian.EOrder endian = Endian.EOrder.Big,
            bool encrypted = false,
            bool compressed = false,
            string encryptionPassword = null,
            ICodeProgress compressionProgress = null)
            => await Serializer.ExportBinaryAsync(this,
                directory, fileName, flags, progress, cancel,
                endian, encrypted, compressed, encryptionPassword, compressionProgress);
        
        public static ConcurrentHashSet<string> ExportingPaths = new ConcurrentHashSet<string>();
        public async Task Export3rdPartyAsync(
            string directory,
            string fileName,
            string thirdPartyExt,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            if (string.IsNullOrWhiteSpace(directory) ||
                directory.IsExistingDirectoryPath() != true)
            {
                Engine.LogWarning($"Cannot export {fileName}.{thirdPartyExt}; directory is not valid.");
                return;
            }

            if (thirdPartyExt.StartsWith("."))
                thirdPartyExt = thirdPartyExt.Substring(1);
            thirdPartyExt = thirdPartyExt.ToLowerInvariant();

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            fileName = string.IsNullOrEmpty(fileName) ? "NewFile" : fileName;

            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                directory += Path.DirectorySeparatorChar;

            TFile3rdPartyExt ext = File3rdPartyExtensions;

            if (ext is null)
            {
                Engine.LogWarning($"No {nameof(TFile3rdPartyExt)} attribute specified for {GetType().GetFriendlyName()}.");
                return;
            }
            if (!ext.HasExtension(thirdPartyExt, false))
            {
                Engine.LogWarning($"{GetType().GetFriendlyName()} cannot be exported as {thirdPartyExt}.");
                return;
            }

            FilePath = directory + fileName + "." + thirdPartyExt;

            ExportingPaths.Add(FilePath);
            Delegate exporter = Engine.DomainProxy.Get3rdPartyExporter(GetType(), thirdPartyExt);
            if (exporter != null)
            {
                Type genericDef = exporter.GetType().GetGenericTypeDefinition();
                if (genericDef == typeof(Del3rdPartyExportFileMethod<>))
                    exporter.DynamicInvoke(this, FilePath);
                else if (genericDef == typeof(Del3rdPartyExportFileMethodAsync<>))
                    await (Task)exporter.DynamicInvoke(this, FilePath, progress, cancel);
                else
                {
                    if (ext.AsyncManualWrite)
                        await ManualWrite3rdPartyAsync(FilePath);
                    else
                        ManualWrite3rdParty(FilePath);
                }
            }
            else
            {
                if (ext.AsyncManualWrite)
                    await ManualWrite3rdPartyAsync(FilePath);
                else
                    ManualWrite3rdParty(FilePath);
            }
            Engine.PrintLine($"Saved {thirdPartyExt} file to {FilePath}");
            ExportingPaths.TryRemove(FilePath);
        }

        #endregion

        #region Manual Read/Write
        /// <summary>
        /// When 'IsThirdParty' is true in the FileClass attribute, this method is called to write the object to a path.
        /// </summary>
        /// <param name="filePath">The path of the file to write.</param>
        public virtual Task ManualWrite3rdPartyAsync(string filePath)
            => throw new NotImplementedException("Override of \"internal protected virtual void ManualWrite3rdParty(string filePath)\" required when 'IsThirdParty' is true in FileClass attribute.");
        /// <summary>
        /// When 'IsThirdParty' is true in the FileClass attribute, this method is called to read the object from a path.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        public virtual Task ManualRead3rdPartyAsync(string filePath)
            => throw new NotImplementedException("Override of \"internal protected virtual void ManualRead3rdParty(string filePath)\" required when 'IsThirdParty' is true in FileClass attribute.");
        /// <summary>
        /// When 'IsThirdParty' is true in the FileClass attribute, this method is called to write the object to a path.
        /// </summary>
        /// <param name="filePath">The path of the file to write.</param>
        public virtual void ManualWrite3rdParty(string filePath)
            => throw new NotImplementedException("Override of \"internal protected virtual void ManualWrite3rdParty(string filePath)\" required when 'IsThirdParty' is true in FileClass attribute.");
        /// <summary>
        /// When 'IsThirdParty' is true in the FileClass attribute, this method is called to read the object from a path.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        public virtual void ManualRead3rdParty(string filePath)
            => throw new NotImplementedException("Override of \"internal protected virtual void ManualRead3rdParty(string filePath)\" required when 'IsThirdParty' is true in FileClass attribute.");
        /// <summary>
        /// Override if the FileClass attribute for this class specifies ManualXmlSerialize.
        /// </summary>
        /// <param name="node">The tree node containing information for this object.</param>
        public virtual void ManualWrite(SerializeElement node)
            => throw new NotImplementedException("Override of \"internal protected virtual void ManualWrite(MemberTreeNode node)\" required when using ManualXmlSerialize in FileClass attribute.");
        /// <summary>
        /// Override if the FileClass attribute for this class specifies ManualXmlSerialize.
        /// </summary>
        /// <param name="node">The tree node containing information for this object.</param>
        public virtual void ManualRead(SerializeElement node)
            => throw new NotImplementedException("Override of \"internal protected virtual void ManualRead(MemberTreeNode node)\" required when using ManualXmlSerialize in FileClass attribute.");
        /// <summary>
        /// Override if the FileClass attribute for this class specifies ManualBinSerialize.
        /// </summary>
        /// <param name="stringTable">The string table to add any strings to.</param>
        /// <param name="flags">The serialization flags for this export.</param>
        /// <returns>The size of this object in bytes.</returns>
        public virtual int ManualGetSizeBinary(BinaryStringTableWriter stringTable, ESerializeFlags flags)
            => throw new NotImplementedException("Override of \"internal protected virtual void ManualGetSizeBinary(BinaryStringTableWriter stringTable, ESerializeFlags flags)\" required when using ManualBinSerialize in FileClass attribute.");
        /// <summary>
        /// Override if the FileClass attribute for this class specifies ManualBinSerialize.
        /// </summary>
        /// <param name="address">Address to write to.</param>
        /// <param name="length">Length of the memory allocated for this object.</param>
        /// <param name="stringTable">The string table to retrieve string offsets from.</param>
        /// <param name="flags">The serialization flags for this export.</param>
        public virtual void ManualWriteBinary(VoidPtr address, int length, BinaryStringTableWriter stringTable, ESerializeFlags flags)
            => throw new NotImplementedException("Override of \"internal protected virtual void ManualWriteBinary(VoidPtr address, int length, BinaryStringTableWriter stringTable, ESerializeFlags flags)\" required when using ManualBinSerialize in FileClass attribute.");
        /// <summary>
        /// Override if the FileClass attribute for this class specifies ManualBinSerialize.
        /// </summary>
        /// <param name="address">Address to read from.</param>
        /// <param name="length">Length of the memory allocated for this object.</param>
        /// <param name="stringTable">The string table to retrieve strings from using offsets.</param>
        /// <param name="flags">The serialization flags for this export.</param>
        public virtual void ManualReadBinary(VoidPtr address, int length, BinaryStringTableReader stringTable)
            => throw new NotImplementedException("Override of \"internal protected virtual void ManualReadBinary(VoidPtr address, int length, BinaryStringTableReader stringTable)\" required when using ManualBinSerialize in FileClass attribute.");
        #endregion

        /// <summary>
        /// Method declaration if not async:
        /// static ClassName Load(string path)
        /// <para>&nbsp;</para>
        /// Method declaration if async:
        /// static async Task<ClassName> LoadAsync(string path, IProgress<float> progress, CancellationToken cancel)
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class ThirdPartyLoader : Attribute
        {
            public string Extension { get; private set; }
            public bool Async { get; private set; }

            /// <summary>
            /// Method declaration if not async:
            /// static ClassName Load(string path)
            /// <para>&nbsp;</para>
            /// Method declaration if async:
            /// static async Task<ClassName> LoadAsync(string path, IProgress<float> progress, CancellationToken cancel)
            /// </summary>
            /// <param name="extension">The extension this method will handle.</param>
            /// <param name="isAsync">If the method is declared as async or not.</param>
            public ThirdPartyLoader(string extension, bool isAsync)
            {
                Extension = extension;
                Async = isAsync;
            }
        }
        /// <summary>
        /// Method declaration if not async:
        /// static void Export(ClassName obj, string path)
        /// <para>&nbsp;</para>
        /// Method declaration if async:
        /// static async Task ExportAsync(ClassName obj, string path, IProgress<float> progress, CancellationToken cancel)
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class ThirdPartyExporter : Attribute
        {
            public string Extension { get; private set; }
            public bool Async { get; private set; }

            /// <summary>
            /// Method declaration if not async:
            /// static void Export(ClassName obj, string path)
            /// <para>&nbsp;</para>
            /// Method declaration if async:
            /// static async Task ExportAsync(ClassName obj, string path, IProgress<float> progress, CancellationToken cancel)
            /// </summary>
            /// <param name="extension">The extension this method will handle.</param>
            /// <param name="isAsync">If the method is declared as async or not.</param>
            public ThirdPartyExporter(string extension, bool isAsync)
            {
                Extension = extension;
                Async = isAsync;
            }
        }
    }
    [Flags]
    public enum ESerializeFlags
    {
        None = 0,
        /// <summary>
        /// If set, exports properties with TSerialize.Config set to true.
        /// </summary>
        SerializeConfig = 0x1,
        /// <summary>
        /// If set, exports properties with TSerialize.State set to true.
        /// </summary>
        SerializeState = 0x2,
        /// <summary>
        /// If set, exports local file refs if they point to an external path and are loaded.
        /// </summary>
        ExportLocalRefs = 0x4,
        /// <summary>
        /// If set, exports global file refs if they point to an external path and are loaded.
        /// </summary>
        ExportGlobalRefs = 0x8,
        /// <summary>
        /// If set, only exports members that have been changed from the value that was set when they were first deserialized or constructed.
        /// </summary>
        WriteChangedMembersOnly = 0x10,
        /// <summary>
        /// If set, writes null members as empty elements.
        /// </summary>
        WriteDefaultMembers = 0x20,

        Default = SerializeConfig | SerializeState | ExportGlobalRefs | ExportLocalRefs | WriteChangedMembersOnly,

        All = -1,
    }
    public enum EProprietaryFileFormat
    {
        Binary = 0,
        XML = 1,
    }
    [Flags]
    public enum EProprietaryFileFormatFlag : ushort
    {
        None = 0b0000,
        Binary = 0b0001,
        XML = 0b0010,
        //JSON    = 0b0100,
        //Text    = 0b1000,
        All = 0b1111,
    }
    public enum EFileFormat
    {
        Binary = 0,
        XML = 1,
        ThirdParty = 2,
        //Programatic = 3,
    }
    public interface IFileObject : IObject
    {
        string FilePath { get; set; }
        //List<IFileRef> References { get; set; }
        TFileDef FileDefinition { get; }
        TFileExt FileExtension { get; }
        TFile3rdPartyExt File3rdPartyExtensions { get; }
        /// <summary>
        /// Returns the file object that serves as the owner of this one.
        /// </summary>
        IFileObject RootFile { get; set; }

        void Unload();

        string GetFilePath(string dir, string name, EProprietaryFileFormat format);
        string GetFilter(bool proprietary = true, bool thirdParty = true, bool import3rdParty = false, bool export3rdParty = false);

        Task ManualWrite3rdPartyAsync(string filePath);
        Task ManualRead3rdPartyAsync(string filePath);
        void ManualWrite3rdParty(string filePath);
        void ManualRead3rdParty(string filePath);
        void ManualWrite(SerializeElement node);
        void ManualRead(SerializeElement node);
        int ManualGetSizeBinary(BinaryStringTableWriter stringTable, ESerializeFlags flags);
        void ManualWriteBinary(VoidPtr address, int length, BinaryStringTableWriter stringTable, ESerializeFlags flags);
        void ManualReadBinary(VoidPtr address, int length, BinaryStringTableReader stringTable);

        void Export();

        void Export(ESerializeFlags flags);
        void Export(ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);

        void Export(string path, ESerializeFlags flags = ESerializeFlags.Default);
        void Export(string path, IProgress<float> progress, CancellationToken cancel);
        void Export(string path, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);

        void Export(string directory, string fileName, ESerializeFlags flags = ESerializeFlags.Default, EProprietaryFileFormat? propFormat = null);
        void Export(string directory, string fileName, ESerializeFlags flags, EProprietaryFileFormat? propFormat, IProgress<float> progress, CancellationToken cancel);
        void Export(string directory, string fileName, ESerializeFlags flags, EFileFormat format, string thirdPartyExt, IProgress<float> progress, CancellationToken cancel);

        Task ExportAsync();

        Task ExportAsync(ESerializeFlags flags);
        Task ExportAsync(ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);

        Task ExportAsync(string path, ESerializeFlags flags = ESerializeFlags.Default);
        Task ExportAsync(string path, IProgress<float> progress, CancellationToken cancel);
        Task ExportAsync(string path, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);

        Task ExportAsync(string directory, string fileName, ESerializeFlags flags = ESerializeFlags.Default, EProprietaryFileFormat? propFormat = null);
        Task ExportAsync(string directory, string fileName, ESerializeFlags flags, EProprietaryFileFormat? propFormat, IProgress<float> progress, CancellationToken cancel);
        Task ExportAsync(string directory, string fileName, ESerializeFlags flags, EFileFormat format, string thirdPartyExt, IProgress<float> progress, CancellationToken cancel);

        Task ExportAsync(string directory, string fileName, EProprietaryFileFormat? propFormat = null);
    }
}
