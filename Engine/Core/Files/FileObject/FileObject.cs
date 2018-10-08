using TheraEngine.Core.Files.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System;
using System.Linq;
using System.Reflection;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Core.Memory;
using System.Threading.Tasks;
using System.Threading;
using TheraEngine.Core.Files.Serialization;

namespace TheraEngine.Core.Files
{
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
    public enum EFileFormat
    {
        Binary      = 0,
        XML         = 1,
        ThirdParty  = 2,
        //Programatic = 3,
    }
    public interface IFileObject : IObject
    {
        string FilePath { get; set; }
        //List<IFileRef> References { get; set; }
        FileDef FileDefinition { get; }
        FileExt FileExtension { get; }
        File3rdParty File3rdPartyExtensions { get; }
        TFileObject RootFile { get; }
        void Unload();
        
        string GetFilePath(string dir, string name, EProprietaryFileFormat format);
        string GetFilter(bool proprietary = true, bool import3rdParty = false, bool export3rdParty = false);
        void Read3rdParty(string path);

        void Export(ESerializeFlags flags = ESerializeFlags.Default);
        void Export(string path, ESerializeFlags flags = ESerializeFlags.Default);
        void Export(string directory, string fileName, ESerializeFlags flags = ESerializeFlags.Default);
        void Export(string directory, string fileName, EFileFormat format, string thirdPartyExt = null, ESerializeFlags flags = ESerializeFlags.Default);

        Task ExportAsync(ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);
        Task ExportAsync(string path, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);
        Task ExportAsync(string directory, string fileName, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);
        Task ExportAsync(string directory, string fileName, EFileFormat format, string thirdPartyExt, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel);
    }
    /// <summary>
    /// Base class for classes that can be stored as files.
    /// </summary>
    //[FileExt("tasset")]
    //[FileDef("Thera Engine Asset")]
    public abstract partial class TFileObject : TObject, IFileObject
    {
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

        [Browsable(false)]
        public FileDef FileDefinition => GetFileDefinition(GetType());
        [Browsable(false)]
        public FileExt FileExtension => GetFileExtension(GetType());
        [Browsable(false)]
        public File3rdParty File3rdPartyExtensions => GetFile3rdPartyExtensions(GetType());
        
        public delegate TFileObject Del3rdPartyImportFileMethod(string path);
        public delegate Task<TFileObject> Del3rdPartyImportFileMethodAsync(string path, IProgress<float> progress, CancellationToken cancel);
        public delegate void Del3rdPartyExportFileMethod(object obj, string path);
        public delegate Task Del3rdPartyExportFileMethodAsync(object obj, string path, IProgress<float> progress, CancellationToken cancel);
        
        public TFileObject() { }

        [Browsable(false)]
        [TString(false, true, false)]
        [Category("Object")]
        public virtual string FilePath { get; set; }
        [Browsable(false)]
        [TString(false, true, false)]
        [Category("Object")]
        public string Original3rdPartyPath { get; set; }
        [Browsable(false)]
        public string DirectoryPath => !string.IsNullOrEmpty(FilePath) && FilePath.IsValidExistingPath() ? Path.GetDirectoryName(FilePath) : string.Empty;
        [Browsable(false)]
        public int CalculatedSize { get; private set; }
        [Browsable(false)]
        public List<IFileRef> References { get; set; } = new List<IFileRef>();
        [Browsable(false)]
        public TFileObject RootFile { get; internal set; }

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

        #region Import/Export
        //[GridCallable("Save")]
        /// <summary>
        /// Writes this file at its FilePath.
        /// Does nothing if this file has no FilePath set.
        /// </summary>
        /// <param name="flags"></param>
        public void Export(ESerializeFlags flags = ESerializeFlags.Default)
            => Export(FilePath, flags);
        //[GridCallable("Save")]
        public void Export(string path, ESerializeFlags flags = ESerializeFlags.Default)
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
                Export(dir, name, pathFormat, ext, flags);
                return;
            }
            else if (tpAttrib != null)
            {
                bool hasWildcard = tpAttrib.ExportableExtensions.Contains("*");
                bool hasExt = tpAttrib.ExportableExtensions.Contains(ext.ToLowerInvariant());
                if (!hasWildcard && !hasExt && tpAttrib.ExportableExtensions.Length > 0)
                    ext = tpAttrib.ExportableExtensions[0];

                Export(dir, name, EFileFormat.ThirdParty, ext, flags);
                return;
            }

            Engine.LogWarning("{0} cannot be exported with extension '{1}'.", type.GetFriendlyName(), ext);
        }
        //[GridCallable("Save")]
        public void Export(
            string directory,
            string fileName,
            ESerializeFlags flags = ESerializeFlags.Default)
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
                Export(directory, fileName, format, ext, flags);
            }
            else
                Engine.LogWarning("File was not exported; cannot resolve extension for {0}.", GetType().GetFriendlyName());
        }
        //[GridCallable("Save")]
        public void Export(
            string directory,
            string fileName,
            EFileFormat format,
            string thirdPartyExt = null,
            ESerializeFlags flags = ESerializeFlags.Default)
        {
            switch (format)
            {
                case EFileFormat.ThirdParty:
                    To3rdParty(directory, fileName, thirdPartyExt);
                    break;
                case EFileFormat.XML:
                    ToXML(directory, fileName, flags);
                    break;
                case EFileFormat.Binary:
                    ToBinary(directory, fileName, flags);
                    break;
                default:
                    throw new InvalidOperationException("Not a valid file format.");
            }
        }
        public async Task ExportAsync(ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
            => await ExportAsync(FilePath, flags, progress, cancel);
        //[GridCallable("Save")]
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
        //[GridCallable("Save")]
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
        //[GridCallable("Save")]
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
                    await To3rdPartyAsync(directory, fileName, thirdPartyExt, progress, cancel);
                    break;
                case EFileFormat.XML:
                    await ToXMLAsync(directory, fileName, flags, progress, cancel);
                    break;
                case EFileFormat.Binary:
                    await ToBinaryAsync(directory, fileName, flags, progress, cancel);
                    break;
                default:
                    throw new InvalidOperationException("Not a valid file format.");
            }
        }
        #endregion

        #region XML
        internal void ToXML(
            string directory,
            string fileName,
            ESerializeFlags flags = ESerializeFlags.Default)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                Engine.LogWarning("Cannot export file to XML; directory is null.");
                return;
            }

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            fileName = string.IsNullOrEmpty(fileName) ? "NewFile" : fileName;

            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                directory += Path.DirectorySeparatorChar;

            FileExt ext = FileExtension;

            if (ext == null)
                throw new Exception("No FileExt attribute specified for " + GetType().GetFriendlyName());

            FilePath = directory + fileName + "." + ext.GetProperExtension(EProprietaryFileFormat.XML);

            if (ext.ManualXmlConfigSerialize)
            {
                using (FileStream stream = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 0x1000, FileOptions.SequentialScan))
                using (XmlWriter writer = XmlWriter.Create(stream, DefaultWriterSettings))
                {
                    writer.Flush();
                    stream.Position = 0;

                    writer.WriteStartDocument();
                    WriteXMLAsync(writer, flags, null, CancellationToken.None).RunSynchronously();
                    writer.WriteEndDocument();
                }
            }
            else
                new CustomXmlSerializer().SerializeAsync(this, FilePath, flags);

            Engine.PrintLine("Saved XML file to {0}", FilePath);
        }
        internal async Task ToXMLAsync(
            string directory,
            string fileName,
            ESerializeFlags flags, 
            IProgress<float> progress,
            CancellationToken cancel)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                Engine.LogWarning("Cannot export file to XML; directory is null.");
                return;
            }

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            fileName = string.IsNullOrEmpty(fileName) ? "NewFile" : fileName;

            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                directory += Path.DirectorySeparatorChar;

            FileExt ext = FileExtension;

            if (ext == null)
                throw new Exception("No FileExt attribute specified for " + GetType().GetFriendlyName());

            FilePath = directory + fileName + "." + ext.GetProperExtension(EProprietaryFileFormat.XML);

            if (ext.ManualXmlConfigSerialize)
            {
                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "\t",
                    NewLineChars = Environment.NewLine,
                    NewLineHandling = NewLineHandling.Replace,
                    Async = true,
                };
                using (FileStream stream = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 0x1000, FileOptions.SequentialScan))
                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    await writer.FlushAsync();
                    stream.Position = 0;

                    await writer.WriteStartDocumentAsync();
                    await WriteXMLAsync(writer, flags, progress, cancel);
                    await writer.WriteEndDocumentAsync();
                }
            }
            else
            {
                CustomXmlSerializer serializer = new CustomXmlSerializer();
                await serializer.SerializeAsync(this, FilePath, flags, progress, cancel);
            }

            Engine.PrintLine("Saved XML file to {0}", FilePath);
        }
        /// <summary>
        /// Writes this object to an xml file using the given xml writer.
        /// Override if the FileClass attribute for this class specifies ManualXmlSerialize.
        /// </summary>
        /// <param name="writer">The xml writer to write the file with.</param>
        internal protected virtual async Task WriteAsync(TSerializer.AbstractWriter writer)
            => throw new NotImplementedException("Override of \"internal protected virtual async Task Write(XmlWriter writer)\" required when using ManualXmlSerialize in FileClass attribute.");
        /// <summary>
        /// Reads this object from an xml file using the given xml reader.
        /// Override if the FileClass attribute for this class specifies ManualXmlSerialize.
        /// </summary>
        /// <param name="reader">The xml reader to read the file with.</param>
        internal protected virtual async Task ReadAsync(TDeserializer.AbstractReader reader)
            => throw new NotImplementedException("Override of \"internal protected virtual async Task Read(XMLReader reader)\" required when using ManualXmlSerialize in FileClass attribute.");

        #endregion

        #region Binary

        internal unsafe void ToBinary(
            string directory,
            string fileName,
            ESerializeFlags flags = ESerializeFlags.Default)
        {
            Type t = GetType();

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            fileName = string.IsNullOrEmpty(fileName) ? "NewFile" : fileName;

            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                directory += Path.DirectorySeparatorChar;

            FileExt ext = FileExtension;

            FilePath = directory + fileName + "." + ext.GetProperExtension(EProprietaryFileFormat.Binary);

            if (ext.ManualBinConfigSerialize)
            {
                using (FileStream stream = new FileStream(FilePath,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.ReadWrite,
                    8,
                    FileOptions.RandomAccess))
                {
                    BinaryStringTable table = new BinaryStringTable();
                    int dataSize = CalculateSize(table, flags).Align(4);
                    int stringSize = table.GetTotalSize();
                    int totalSize = dataSize + stringSize;
                    stream.SetLength(totalSize);
                    using (FileMap map = FileMap.FromStream(stream))
                    {
                        FileCommonHeader* hdr = (FileCommonHeader*)map.Address;
                        table.WriteTable(hdr);
                        hdr->_fileLength = totalSize;
                        hdr->_stringTableLength = stringSize;
                        hdr->_endian = (byte)Engine.ComputerInfo.Endian;
                        Write(hdr->Data, table, flags);
                    }
                }
            }
            else
            {
                CustomBinarySerializer.Serialize(
                    this,
                    FilePath,
                    Endian.EOrder.Big,
                    true,
                    true,
                    "test",
                    out byte[] encryptionSalt,
                    out byte[] integrityHash,
                    null,
                    flags);
            }

            Engine.PrintLine("Saved binary file to {0}", FilePath);
        }

        /// <summary>
        /// Calculates the size of this object, in bytes.
        /// </summary>
        /// <param name="table">The string table to populate with strings.</param>
        /// <returns>The size of the object, in bytes.</returns>
        internal int CalculateSize(BinaryStringTable table, ESerializeFlags flags)
        {
            CalculatedSize = OnCalculateSize(table, flags);
            return CalculatedSize;
        }
        /// <summary>
        /// Calculates the size of this object, in bytes.
        /// Override if the FileClass attribute for this class specifies ManualBinSerialize.
        /// </summary>
        /// <param name="table">The string table. Add strings to this as you wish, and use their addresses when writing later.</param>
        /// <returns>The size of the object, in bytes.</returns>
        protected virtual int OnCalculateSize(BinaryStringTable table, ESerializeFlags flags)
            => throw new NotImplementedException("Override of \"protected virtual int OnCalculateSize(StringTable table)\" required when using ManualBinarySerialize in FileClass attribute.");
        /// <summary>
        /// Writes this object to the given address.
        /// The size of this object is CalculatedSize.
        /// Override if the FileClass attribute for this class specifies ManualBinSerialize.
        /// </summary>
        /// <param name="address">The address to write to.</param>
        /// <param name="table">The table of all strings added in OnCalculateSize.</param>
        internal protected virtual void Write(VoidPtr address, BinaryStringTable table, ESerializeFlags flags)
            => throw new NotImplementedException("Override of \"internal protected virtual void Write(VoidPtr address, StringTable table)\" required when using ManualBinarySerialize in FileClass attribute.");
        /// <summary>
        /// Reads this object from the given address.
        /// Override if the FileClass attribute for this class specifies ManualBinSerialize.
        /// </summary>
        /// <param name="address">The address to read from.</param>
        /// <param name="strings">The string table to get strings from.</param>
        internal protected virtual void Read(VoidPtr address, VoidPtr strings)
            => throw new NotImplementedException("Override of \"internal protected virtual void Read(VoidPtr address, VoidPtr strings)\" required when using ManualBinarySerialize in FileClass attribute.");
        #endregion

        #region 3rd Party
        private void To3rdParty(string directory, string fileName, string thirdPartyExt)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            FilePath = GetFilePath(directory, fileName, thirdPartyExt);
            Write3rdParty(FilePath);
            Engine.PrintLine("Saved third party file to {0}", FilePath);
        }
        /// <summary>
        /// When 'IsThirdParty' is true in the FileClass attribute, this method is called to write the object to a path.
        /// </summary>
        /// <param name="filePath">The path of the file to write.</param>
        public virtual void Write3rdParty(string filePath)
            => throw new NotImplementedException("Override of \"internal protected virtual void WriteThirdParty(string filePath)\" required when 'IsThirdParty' is true in FileClass attribute.");
        /// <summary>
        /// When 'IsThirdParty' is true in the FileClass attribute, this method is called to read the object from a path.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        public virtual void Read3rdParty(string filePath)
            => throw new NotImplementedException("Override of \"internal protected virtual void ReadThirdParty(string filePath)\" required when 'IsThirdParty' is true in FileClass attribute.");
        #endregion
    }
}
