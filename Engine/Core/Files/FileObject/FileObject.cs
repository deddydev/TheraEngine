using SevenZip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Reflection.Attributes;
using static TheraEngine.Core.Files.Serialization.TDeserializer.ReaderBinary;
using static TheraEngine.Core.Files.Serialization.TSerializer.WriterBinary;

namespace TheraEngine.Core.Files
{
    /// <summary>
    /// Base class for classes that can be stored as files.
    /// </summary>
    //[FileExt("tasset")]
    //[FileDef("Thera Engine Asset")]
    public abstract partial class TFileObject : TObject, IFileObject
    {
        /// <summary>
        /// If true, this object was originally constructed via code.
        /// If false, this object was originally deserialized from a file.
        /// </summary>
        [Browsable(false)]
        public bool ConstructedProgrammatically { get; internal set; } = true;
        [Browsable(false)]
        public FileDef FileDefinition => GetFileDefinition(GetType());
        [Browsable(false)]
        public FileExt FileExtension => GetFileExtension(GetType());
        [Browsable(false)]
        public File3rdParty File3rdPartyExtensions => GetFile3rdPartyExtensions(GetType());

        public delegate T Del3rdPartyImportFileMethod<T>(string path) where T : IFileObject;
        public delegate Task<T> Del3rdPartyImportFileMethodAsync<T>(string path, IProgress<float> progress, CancellationToken cancel) where T : IFileObject;

        public delegate void Del3rdPartyExportFileMethod(object obj, string path);
        public delegate Task Del3rdPartyExportFileMethodAsync(object obj, string path, IProgress<float> progress, CancellationToken cancel);
        
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
                if (_rootFile == null || _rootFile == this)
                    return _filePath;

                return _rootFile.FilePath;
            }
            set => _filePath = value;
        }
        [Browsable(false)]
        [TString(false, true, false)]
        [Category("Object")]
        public string Original3rdPartyPath { get; set; }
        [Browsable(false)]
        public string DirectoryPath => !string.IsNullOrEmpty(FilePath) && FilePath.IsValidExistingPath() ? Path.GetDirectoryName(FilePath) : string.Empty;
        [Browsable(false)]
        public List<IFileRef> References { get; set; } = new List<IFileRef>();

        [Browsable(false)]
        public TFileObject RootFile
        {
            get => _rootFile ?? this;
            internal set => _rootFile = value;
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
        public string GetFilter(bool proprietary = true, bool import3rdParty = false, bool export3rdParty = false)
            => GetFilter(GetType(), proprietary, import3rdParty, export3rdParty);
        
        public async void Export()
            => await ExportAsync(ESerializeFlags.Default);
        public async void Export(ESerializeFlags flags) 
            => await ExportAsync(flags);
        public async void Export(string path, ESerializeFlags flags = ESerializeFlags.Default)
            => await ExportAsync(path, flags);
        public async void Export(string directory, string fileName, ESerializeFlags flags = ESerializeFlags.Default)
            => await ExportAsync(directory, fileName, flags);
        public async void Export(ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
            => await ExportAsync(flags, progress, cancel);
        public async void Export(string path, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
            => await ExportAsync(FilePath, flags, progress, cancel);
        public async void Export(string directory, string fileName, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
            => await ExportAsync(directory, fileName, flags, progress, cancel);
        public async void Export(string directory, string fileName, EFileFormat format, string thirdPartyExt, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
            => await ExportAsync(directory, fileName, format, thirdPartyExt, flags, progress, cancel);

        public async Task ExportAsync() => await ExportAsync(ESerializeFlags.Default);
        public async Task ExportAsync(ESerializeFlags flags)
        {
            if (Engine.BeginOperation != null)
            {
                int op = Engine.BeginOperation($"Exporting file to {FilePath}...", out Progress<float> progress, out CancellationTokenSource cancel);
                await ExportAsync(flags, progress, cancel.Token);
                if (Engine.EndOperation != null)
                    Engine.EndOperation(op);
                else
                    ((IProgress<float>)progress).Report(1.0f);
            }
            else
            {
                await ExportAsync(flags, null, CancellationToken.None);
            }
        }
        public async Task ExportAsync(
            string directory,
            string fileName,
            ESerializeFlags flags = ESerializeFlags.Default)
        {
            if (Engine.BeginOperation != null)
            {
                int op = Engine.BeginOperation($"Exporting file to {FilePath}...", out Progress<float> progress, out CancellationTokenSource cancel);
                await ExportAsync(directory, fileName, flags, progress, cancel.Token);
                if (Engine.EndOperation != null)
                    Engine.EndOperation(op);
                else
                    ((IProgress<float>)progress).Report(1.0f);
            }
            else
            {
                await ExportAsync(directory, fileName, flags, null, CancellationToken.None);
            }
        }
        public async Task ExportAsync(
            string path,
            ESerializeFlags flags = ESerializeFlags.Default)
        {
            if (Engine.BeginOperation != null)
            {
                int op = Engine.BeginOperation($"Exporting file to {FilePath}...", out Progress<float> progress, out CancellationTokenSource cancel);
                await ExportAsync(path, flags, progress, cancel.Token);
                if (Engine.EndOperation != null)
                    Engine.EndOperation(op);
                else
                    ((IProgress<float>)progress).Report(1.0f);
            }
            else
            {
                await ExportAsync(path, flags, null, CancellationToken.None);
            }
        }

        public async Task ExportAsync(ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
            => await ExportAsync(FilePath, flags, progress, cancel);
        public async Task ExportAsync(string path, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
        {
            if (string.IsNullOrEmpty(path))
            {
                Engine.LogWarning("File was not exported; file path is not valid.");
                return;
            }

            Type type = GetType();
            FileExt extAttrib = FileExtension;
            File3rdParty tpAttrib = GetFile3rdPartyExtensions(type);
            GetDirNameFmt(path, out string dir, out string name, out EFileFormat pathFormat, out string ext);

            if (extAttrib != null && pathFormat != EFileFormat.ThirdParty)
            {
                ext = extAttrib.GetProperExtension((EProprietaryFileFormat)(int)pathFormat);
                await ExportAsync(dir, name, pathFormat, ext, flags, progress, cancel);
                return;
            }
            else if (tpAttrib != null)
            {
                bool hasWildcard = tpAttrib.ExportableExtensions.Contains("*");
                bool hasExt = tpAttrib.ExportableExtensions.Contains(ext.ToLowerInvariant());
                if (!hasWildcard && !hasExt && tpAttrib.ExportableExtensions.Length > 0)
                    ext = tpAttrib.ExportableExtensions[0];

                await ExportAsync(dir, name, EFileFormat.ThirdParty, ext, flags, progress, cancel);
                return;
            }

            Engine.LogWarning("{0} cannot be exported with extension '{1}'.", type.GetFriendlyName(), ext);
        }
        public async Task ExportAsync(
            string directory,
            string fileName,
            ESerializeFlags flags, 
            IProgress<float> progress,
            CancellationToken cancel)
        {
            string ext = null;
            FileExt fileExt = FileExtension;
            if (fileExt != null)
            {
                ext = fileExt.GetProperExtension((EProprietaryFileFormat)fileExt.PreferredFormat);
            }
            else
            {
                File3rdParty tp = File3rdPartyExtensions;
                if (tp != null &&
                    tp.ExportableExtensions != null &&
                    tp.ExportableExtensions.Length > 0)
                {
                    ext = tp.ExportableExtensions[0];
                }
            }
            if (ext != null)
            {
                EFileFormat format = GetFormat(ext, out string ext2);
                await ExportAsync(directory, fileName, format, ext, flags, progress, cancel);
            }
            else
                Engine.LogWarning("File was not exported; cannot resolve extension for {0}.", GetType().GetFriendlyName());
        }
        public async Task ExportAsync(
            string directory,
            string fileName,
            EFileFormat format,
            string thirdPartyExt,
            ESerializeFlags flags, 
            IProgress<float> progress, 
            CancellationToken cancel)
        {
            switch (format)
            {
                case EFileFormat.ThirdParty:
                    await Export3rdPartyAsync(directory, fileName, thirdPartyExt, progress, cancel);
                    break;
                case EFileFormat.XML:
                    await ExportXMLAsync(directory, fileName, flags, progress, cancel);
                    break;
                case EFileFormat.Binary:
                    await ExportBinaryAsync(directory, fileName, flags, progress, cancel);
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
            => await TSerializer.ExportXMLAsync(this, directory, fileName, flags, progress, cancel);
        
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
            => await TSerializer.ExportBinaryAsync(this,
                directory, fileName, flags, progress, cancel,
                endian, encrypted, compressed, encryptionPassword, compressionProgress);
        
        public async Task Export3rdPartyAsync(
            string directory,
            string fileName,
            string thirdPartyExt,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            if (string.IsNullOrWhiteSpace(directory) || directory.IsExistingDirectoryPath() != true)
            {
                Engine.LogWarning($"Cannot export {fileName}.{thirdPartyExt}; directory is null.");
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

            File3rdParty ext = File3rdPartyExtensions;

            if (ext == null)
            {
                Engine.LogWarning("No File3rdParty attribute specified for " + GetType().GetFriendlyName());
                return;
            }
            if (!ext.ExportableExtensions.Contains(thirdPartyExt))
            {
                Engine.LogWarning($"{GetType().GetFriendlyName()} cannot be exported as {thirdPartyExt}.");
                return;
            }

            FilePath = directory + fileName + "." + thirdPartyExt;

            Delegate exporter = Get3rdPartyExporter(GetType(), thirdPartyExt);
            if (exporter != null)
            {
                if (exporter is Del3rdPartyExportFileMethod method)
                    method.Invoke(this, FilePath);
                else if (exporter is Del3rdPartyExportFileMethodAsync methodAsync)
                    await methodAsync.Invoke(this, FilePath, progress, cancel);
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
            Engine.PrintLine($"Saved {thirdPartyExt} file to {0}", FilePath);
        }

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

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class ThirdPartyLoader : Attribute
        {
            public string Extension { get; private set; }
            public bool Async { get; private set; }
            public ThirdPartyLoader(string extension, bool isAsync = false)
            {
                Extension = extension;
                Async = isAsync;
            }
        }
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class ThirdPartyExporter : Attribute
        {
            public string Extension { get; private set; }
            public bool Async { get; private set; }
            public ThirdPartyExporter(string extension, bool isAsync = false)
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
        ChangedOnly = 0x10,
        Default = SerializeConfig | ExportGlobalRefs | ExportLocalRefs | ChangedOnly,
        All = 0xF,
    }
    public enum EProprietaryFileFormat
    {
        Binary = 0,
        XML = 1,
    }
    [Flags]
    public enum EProprietaryFileFormatFlag
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
        FileDef FileDefinition { get; }
        FileExt FileExtension { get; }
        File3rdParty File3rdPartyExtensions { get; }
        /// <summary>
        /// Returns the file object that serves as the owner of this one.
        /// </summary>
        TFileObject RootFile { get; }
        void Unload();

        string GetFilePath(string dir, string name, EProprietaryFileFormat format);
        string GetFilter(bool proprietary = true, bool import3rdParty = false, bool export3rdParty = false);

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
        void Export(string path, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);
        void Export(string directory, string fileName, ESerializeFlags flags = ESerializeFlags.Default);
        void Export(string directory, string fileName, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);
        void Export(string directory, string fileName, EFileFormat format, string thirdPartyExt, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);

        Task ExportAsync();
        Task ExportAsync(ESerializeFlags flags);
        Task ExportAsync(ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);
        Task ExportAsync(string path, ESerializeFlags flags = ESerializeFlags.Default);
        Task ExportAsync(string path, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);
        Task ExportAsync(string directory, string fileName, ESerializeFlags flags = ESerializeFlags.Default);
        Task ExportAsync(string directory, string fileName, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);
        Task ExportAsync(string directory, string fileName, EFileFormat format, string thirdPartyExt, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);
    }
}
