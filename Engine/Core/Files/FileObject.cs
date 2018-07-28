using TheraEngine.Files.Serialization;
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

namespace TheraEngine.Files
{
    [Flags]
    public enum ESerializeFlags
    {
        None = 0,
        /// <summary>
        /// If set, exports properties TSerialize.Config set to true.
        /// </summary>
        SerializeConfig = 0x1,
        /// <summary>
        /// If set, exports properties TSerialize.State set to true.
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
        Default = SerializeConfig | ExportGlobalRefs | ExportLocalRefs,
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
        TFileObject Root { get; }
        void Unload();
        void Export(ESerializeFlags flags = ESerializeFlags.Default);
        void Export(string path, ESerializeFlags flags = ESerializeFlags.Default);
        void Export(string directory, string fileName, ESerializeFlags flags = ESerializeFlags.Default);
        void Export(string directory, string fileName, EFileFormat format, string thirdPartyExt = null, ESerializeFlags flags = ESerializeFlags.Default);
        string GetFilePath(string dir, string name, EProprietaryFileFormat format);
        string GetFilter(bool proprietary = true, bool import3rdParty = false, bool export3rdParty = false);
        void Read3rdParty(string path);
    }
    //[FileExt("tasset")]
    //[FileDef("Thera Engine Asset")]
    public abstract class TFileObject : TObject, IFileObject
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

        [Browsable(false)]
        public FileDef FileDefinition => GetFileDefinition(GetType());
        public static FileDef GetFileDefinition(Type classType)
        {
            return Attribute.GetCustomAttribute(classType, typeof(FileDef), true) as FileDef;
            //throw new Exception("No FileDef attribute specified for " + t.ToString());
        }
        [Browsable(false)]
        public FileExt FileExtension => GetFileExtension(GetType());
        public static FileExt GetFileExtension(Type classType)
        {
            return Attribute.GetCustomAttribute(classType, typeof(FileExt), true) as FileExt;
        }
        [Browsable(false)]
        public File3rdParty File3rdPartyExtensions => GetFile3rdPartyExtensions(GetType());
        public static File3rdParty GetFile3rdPartyExtensions(Type classType)
        {
            return Attribute.GetCustomAttribute(classType, typeof(File3rdParty), true) as File3rdParty;
            //throw new Exception("No File3rdParty attribute specified for " + t.ToString());
        }
        
        public delegate TFileObject DelThirdPartyFileMethod(string path);
        public delegate Task<TFileObject> DelThirdPartyFileMethodAsync(string path);

        static TFileObject()
        {
            _3rdPartyLoaders = new Dictionary<string, Dictionary<Type, Delegate>>();
            _3rdPartyExporters = new Dictionary<string, Dictionary<Type, Delegate>>();
            try
            {
                var types = Engine.FindTypes(t => t.IsSubclassOf(typeof(TFileObject)) && !t.IsAbstract, true, Assembly.GetEntryAssembly()).ToArray();
                foreach (Type t in types)
                {
                    File3rdParty attrib = GetFile3rdPartyExtensions(t);
                    if (attrib == null)
                        continue;
                    foreach (string ext3rd in attrib.ImportableExtensions)
                    {
                        string extLower = ext3rd.ToLowerInvariant();
                        Dictionary<Type, Delegate> d;
                        if (_3rdPartyLoaders.ContainsKey(extLower))
                            d = _3rdPartyLoaders[extLower];
                        else
                            _3rdPartyLoaders.Add(extLower, d = new Dictionary<Type, Delegate>());
                        if (!d.ContainsKey(t))
                        {
                            var methods = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                .Where(x => string.Equals(x.GetCustomAttribute<ThirdPartyLoader>()?.Extension, extLower, StringComparison.InvariantCultureIgnoreCase))
                                .ToArray();
                            if (methods.Length > 0)
                            {
                                MethodInfo m = methods[0];
                                bool async = m.GetCustomAttribute<ThirdPartyLoader>().Async;
                                if (async)
                                {
                                    if (Delegate.CreateDelegate(typeof(DelThirdPartyFileMethodAsync), m) is DelThirdPartyFileMethodAsync result)
                                        d.Add(t, result);
                                }
                                else
                                {
                                    if (Delegate.CreateDelegate(typeof(DelThirdPartyFileMethod), m) is DelThirdPartyFileMethod result)
                                        d.Add(t, result);
                                }
                            }
                        }
                        else
                            throw new Exception(t.GetFriendlyName() + " has already been added to the third party loader list for " + extLower);
                    }
                }
            }
            catch
            {

            }
        }
        public TFileObject() { }
        //internal protected virtual void OnLoaded() { }

        [Browsable(false)]
        [TString(false, true, false)]
        [Category("Object")]
        public string FilePath { get; set; }
        [Browsable(false)]
        public string DirectoryPath => !string.IsNullOrEmpty(FilePath) && FilePath.IsValidPath() ? Path.GetDirectoryName(FilePath) : string.Empty;
        [Browsable(false)]
        public int CalculatedSize { get; private set; }
        [Browsable(false)]
        public List<IFileRef> References { get; set; } = new List<IFileRef>();
        [Browsable(false)]
        public TFileObject Root { get; internal set; }

        public void Unload()
        {
            List<IFileRef> oldRefs = new List<IFileRef>(References);
            foreach (IFileRef r in oldRefs)
                r.UnloadReference();
            OnUnload();
        }
        protected virtual void OnUnload() { }

        public static Type DetermineType(string path)
        {
            EFileFormat f = GetFormat(path, out string ext);
            switch (f)
            {
                case EFileFormat.XML:
                    return CustomXmlSerializer.DetermineType(path);
                case EFileFormat.Binary:
                    return CustomBinarySerializer.DetermineType(path);
                default:
                    Type[] types = DetermineThirdPartyTypes(ext);
                    return types.Length > 0 ? types[0] : null;
            }
        }

        public static Type[] DetermineThirdPartyTypes(string ext)
        {
            return Engine.FindTypes(t => typeof(TFileObject).IsAssignableFrom(t) && (t.GetCustomAttribute<File3rdParty>()?.HasExtension(ext) ?? false), true, Assembly.GetEntryAssembly()).ToArray();
        }

        public static EFileFormat GetFormat(string path, out string ext)
        {
            int index = path.LastIndexOf('.') + 1;
            if (index != 0)
                ext = path.Substring(index).ToLowerInvariant();
            else
                ext = path.ToLowerInvariant();

            if (File3rdParty.Has3rdPartyExtension(ext))
                return EFileFormat.ThirdParty;

            //TODO: return raw format
            if (ext.Length == 0)
                return EFileFormat.ThirdParty;

            switch (ext[0])
            {
                default: return EFileFormat.ThirdParty;
                case 'b': return EFileFormat.Binary;
                case 'x': return EFileFormat.XML;
            }
        }

        /// <summary>
        /// Extracts the directory, file name, and file format out of a file path.
        /// </summary>
        /// <param name="path">The path to the file. Does not need to be absolute.</param>
        /// <param name="dir">The path to the folder that the file resides in.</param>
        /// <param name="name">The name of the file in the path.</param>
        /// <param name="fmt">The format the data is written in.</param>
        /// <param name="ext">The extension of the file.</param>
        public static void GetDirNameFmt(string path, out string dir, out string name, out EFileFormat fmt, out string ext)
        {
            dir = Path.GetDirectoryName(path);
            name = Path.GetFileNameWithoutExtension(path);
            fmt = GetFormat(path, out ext);
        }

        /// <summary>
        /// Creates the full file path to a file given separate parameters.
        /// </summary>
        /// <param name="dir">The path to the folder the file resides in.</param>
        /// <param name="name">The name of the file.</param>
        /// <param name="format">The format the data is written in.</param>
        /// <param name="fileType">The type of file object.</param>
        /// <returns>An absolute path to the file.</returns>
        public static string GetFilePath(string dir, string name, EProprietaryFileFormat format, Type fileType)
            => Path.Combine(dir, name + "." + GetFileExtension(fileType).GetProperExtension(format));
        public static string GetFilePath<T>(string dir, string name, EProprietaryFileFormat format) where T : TFileObject
            => Path.Combine(dir, name + "." + GetFileExtension(typeof(T)).GetProperExtension(format));
        public static string GetFilePath(string dir, string name, string thirdPartyExtension)
        {
            if (thirdPartyExtension[0] != '.')
                thirdPartyExtension = "." + thirdPartyExtension;
            return Path.Combine(dir, name + thirdPartyExtension);
        }
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
        public static string GetFilter<T>(bool proprietary = true, bool import3rdParty = false, bool export3rdParty = false) where T : TFileObject
            => GetFilter(typeof(T), proprietary, import3rdParty, export3rdParty);
        /// <summary>
        /// Returns the filter for all extensions related to this format.
        /// </summary>
        /// <param name="classType">The type of the class to get the filter for.</param>
        /// <param name="proprietary">Add the TheraEngine proprietary formats (binary, xml, etc)</param>
        /// <param name="import3rdParty">Add any importable 3rd party file formats</param>
        /// <param name="export3rdParty">Add any exportable 3rd party file formats</param>
        /// <returns>The filter to be used in open file dialog, save file dialog, etc</returns>
        public static string GetFilter(Type classType, bool proprietary = true, bool import3rdParty = false, bool export3rdParty = false)
        {
            string allTypes = "";
            string eachType = "";
            bool first = true;
            FileDef def = GetFileDefinition(classType);
            if (proprietary)
            {
                FileExt ext = GetFileExtension(classType);
                foreach (string type in Enum.GetNames(typeof(EProprietaryFileFormat)))
                {
                    if (first)
                        first = false;
                    else
                    {
                        allTypes += ";";
                        eachType += "|";
                    }
                    string fmt = String.Format("*.{0}{1}", type.Substring(0, 1).ToLowerInvariant(), ext.Extension);
                    eachType += String.Format("{0} [{2}] ({1})|{1}", def.UserFriendlyName, fmt, type);
                    allTypes += fmt;
                }
            }
            if (import3rdParty || export3rdParty)
            {
                File3rdParty ext = GetFile3rdPartyExtensions(classType);
                if (import3rdParty)
                {
                    foreach (string ext3rd in ext.ImportableExtensions)
                    {
                        string extLower = ext3rd.ToLowerInvariant();
                        if (first)
                            first = false;
                        else
                        {
                            allTypes += ";";
                            eachType += "|";
                        }
                        string fmt = String.Format("*.{0}", extLower);
                        if (File3rdParty.ExtensionNames3rdParty.ContainsKey(extLower))
                            eachType += String.Format("{0} ({1})|{1}", File3rdParty.ExtensionNames3rdParty[extLower], fmt);
                        else
                            eachType += String.Format("{0} file ({1})|{1}", extLower, fmt);
                        allTypes += fmt;
                    }
                }
                if (export3rdParty)
                {
                    foreach (string ext3rd in ext.ExportableExtensions)
                    {
                        //Don't rewrite extension if it was already written as importable
                        if (import3rdParty && 
                            ext.ImportableExtensions != null && 
                            ext.ImportableExtensions.Contains(ext3rd, StringComparer.InvariantCultureIgnoreCase))
                            continue;

                        string extLower = ext3rd.ToLowerInvariant();

                        if (first)
                            first = false;
                        else
                        {
                            allTypes += ";";
                            eachType += "|";
                        }
                        string fmt = String.Format("*.{0}", extLower);
                        if (File3rdParty.ExtensionNames3rdParty.ContainsKey(extLower))
                            eachType += String.Format("{0} ({1})|{1}", File3rdParty.ExtensionNames3rdParty[extLower], fmt);
                        else
                            eachType += String.Format("{0} file ({1})|{1}", extLower, fmt);
                        allTypes += fmt;
                    }
                }
            }

            string allTypesFull = String.Format("{0} ({1})|{1}", def.UserFriendlyName, allTypes);
            return allTypesFull + "|" + eachType;
        }

        #region Import/Export
        public static async Task<T> LoadAsync<T>(string filePath) where T : TFileObject
            => await LoadAsync(typeof(T), filePath, null, CancellationToken.None) as T;
        /// <summary>
        /// Opens a new instance of the given file at the given file path.
        /// </summary>
        /// <typeparam name="T">The type of the file object to load.</typeparam>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>A new instance of the file.</returns>
        public static async Task<T> LoadAsync<T>(
            string filePath, IProgress<float> progress, CancellationToken cancel) where T : TFileObject
            => await LoadAsync(typeof(T), filePath, progress, cancel) as T;
        /// <summary>
        /// Opens a new instance of the file object at the given file path.
        /// </summary>
        /// <param name="type">The type of the file object to load.</param>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>A new instance of the file.</returns>
        public static async Task<TFileObject> LoadAsync(
            Type type, string filePath, IProgress<float> progress, CancellationToken cancel)
        {
            FileExt extAttrib = GetFileExtension(type);
            File3rdParty tpAttrib = GetFile3rdPartyExtensions(type);
            EFileFormat fmt = GetFormat(filePath, out string ext);
            if (extAttrib != null && fmt != EFileFormat.ThirdParty)
            {
                EProprietaryFileFormat pfmt = fmt == EFileFormat.Binary ?
                    EProprietaryFileFormat.Binary :
                    EProprietaryFileFormat.XML;

                string fileExt = extAttrib.GetProperExtension(pfmt);
                if (string.Equals(ext, fileExt))
                    return fmt == EFileFormat.XML ?
                        await FromXMLAsync(type, filePath, progress, cancel) :
                        await FromBinaryAsync(type, filePath, progress, cancel);
            }
            else if (tpAttrib != null)
            {
                bool hasWildcard = tpAttrib.ImportableExtensions.Contains("*");
                bool hasExt = tpAttrib.ImportableExtensions.Contains(ext.ToLowerInvariant());
                if (hasWildcard || hasExt)
                    return await Read3rdPartyAsync(type, filePath, progress, cancel);
            }

            Engine.LogWarning("{0} cannot be loaded as {1}.", filePath, type.GetFriendlyName());
            return default;
        }
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

        #endregion

        #region XML
        internal static async Task<T> FromXMLAsync<T>(
            string filePath, IProgress<float> progress, CancellationToken cancel)
            where T : TFileObject
            => await FromXMLAsync(typeof(T), filePath, progress, cancel) as T;
        internal static async Task<TFileObject> FromXMLAsync(
            Type type, string filePath, IProgress<float> progress, CancellationToken cancel)
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(filePath))
                    return null;

                TFileObject file;
                FileExt ext = GetFileExtension(type);
                if (ext?.ManualXmlConfigSerialize ?? false)
                {
                    unsafe
                    {
                        using (FileMap map = FileMap.FromFile(filePath))
                        {
                            XMLReader reader = new XMLReader(map.Address, map.Length, true);
                            file = SerializationCommon.CreateObject(type) as TFileObject;
                            if (file != null && reader.BeginElement())
                            {
                                file.FilePath = filePath;
                                //if (reader.Name.Equals(t.ToString(), true))
                                file.Read(reader);
                                //else
                                //    throw new Exception("File was not of expected type.");
                                reader.EndElement();
                            }
                        }
                    }
                }
                else
                    file = new CustomXmlSerializer().Deserialize(filePath);
                if (file != null)
                    file.FilePath = filePath;
                return file;
            });
        }
        private static XmlWriterSettings _writerSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineChars = Environment.NewLine,
            NewLineHandling = NewLineHandling.Replace
        };
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

            fileName = String.IsNullOrEmpty(fileName) ? "NewFile" : fileName;

            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                directory += Path.DirectorySeparatorChar;

            FileExt ext = FileExtension;

            if (ext == null)
                throw new Exception("No FileExt attribute specified for " + GetType().GetFriendlyName());

            FilePath = directory + fileName + "." + ext.GetProperExtension(EProprietaryFileFormat.XML);

            if (ext.ManualXmlConfigSerialize)
            {
                using (FileStream stream = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.SequentialScan))
                using (XmlWriter writer = XmlWriter.Create(stream, _writerSettings))
                {
                    writer.Flush();
                    stream.Position = 0;

                    writer.WriteStartDocument();
                    Write(writer, flags);
                    writer.WriteEndDocument();
                }
            }
            else
                new CustomXmlSerializer().Serialize(this, FilePath, flags);

            Engine.PrintLine("Saved XML file to {0}", FilePath);
        }
        /// <summary>
        /// Writes this object to an xml file using the given xml writer.
        /// Override if the FileClass attribute for this class specifies ManualXmlSerialize.
        /// </summary>
        /// <param name="writer">The xml writer to write the file with.</param>
        internal protected virtual void Write(XmlWriter writer, ESerializeFlags flags)
            => throw new NotImplementedException("Override of \"internal protected virtual void Write(XmlWriter writer)\" required when using ManualXmlSerialize in FileClass attribute.");
        /// <summary>
        /// Reads this object from an xml file using the given xml reader.
        /// Override if the FileClass attribute for this class specifies ManualXmlSerialize.
        /// </summary>
        /// <param name="reader">The xml reader to read the file with.</param>
        internal protected virtual void Read(XMLReader reader)
            => throw new NotImplementedException("Override of \"internal protected virtual void Read(XMLReader reader)\" required when using ManualXmlSerialize in FileClass attribute.");

        #endregion

        #region Binary

        internal static async Task<T> FromBinaryAsync<T>(string filePath, IProgress<float> progress, CancellationToken cancel) where T : TFileObject
            => await FromBinaryAsync(typeof(T), filePath, progress, cancel) as T;
        internal static async Task<TFileObject> FromBinaryAsync(
            Type type, string filePath, IProgress<float> progress, CancellationToken cancel)
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(filePath))
                    return null;

                TFileObject file;
                if (GetFileExtension(type).ManualBinConfigSerialize)
                {
                    file = SerializationCommon.CreateObject(type) as TFileObject;
                    if (file != null)
                    {
                        unsafe
                        {
                            file.FilePath = filePath;
                            FileMap map = FileMap.FromFile(filePath);
                            FileCommonHeader* hdr = (FileCommonHeader*)map.Address;
                            file.Read(hdr->Data, hdr->Strings);
                        }
                    }
                }
                else
                    file = CustomBinarySerializer.Deserialize(filePath, type) as TFileObject;

                if (file != null)
                    file.FilePath = filePath;

                return file;
            });
        }
        internal unsafe void ToBinary(
            string directory,
            string fileName,
            ESerializeFlags flags = ESerializeFlags.Default)
        {
            Type t = GetType();

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            fileName = String.IsNullOrEmpty(fileName) ? "NewFile" : fileName;

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
                    StringTable table = new StringTable();
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
        internal int CalculateSize(StringTable table, ESerializeFlags flags)
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
        protected virtual int OnCalculateSize(StringTable table, ESerializeFlags flags)
            => throw new NotImplementedException("Override of \"protected virtual int OnCalculateSize(StringTable table)\" required when using ManualBinarySerialize in FileClass attribute.");
        /// <summary>
        /// Writes this object to the given address.
        /// The size of this object is CalculatedSize.
        /// Override if the FileClass attribute for this class specifies ManualBinSerialize.
        /// </summary>
        /// <param name="address">The address to write to.</param>
        /// <param name="table">The table of all strings added in OnCalculateSize.</param>
        internal protected virtual void Write(VoidPtr address, StringTable table, ESerializeFlags flags)
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

        public static async Task<T> Read3rdPartyAsync<T>(string filePath) where T : TFileObject
            => await Read3rdPartyAsync(typeof(T), filePath, null, CancellationToken.None) as T;

        public static async Task<T> Read3rdPartyAsync<T>(string filePath, IProgress<float> progress, CancellationToken cancel) where T : TFileObject
            => await Read3rdPartyAsync(typeof(T), filePath, progress, cancel) as T;

        public static async Task<TFileObject> Read3rdPartyAsync(Type classType, string filePath, IProgress<float> progress, CancellationToken cancel)
        {
            string ext = Path.GetExtension(filePath).Substring(1);
            Delegate loader = Get3rdPartyLoader(classType, ext);
            if (loader != null)
            {
                if (loader is DelThirdPartyFileMethod method)
                    return method.Invoke(filePath);
                else if (loader is DelThirdPartyFileMethodAsync methodAsync)
                    return await methodAsync.Invoke(filePath);
                else
                    return null;
            }
            else
            {
                TFileObject obj = Activator.CreateInstance(classType) as TFileObject;
                obj.Read3rdParty(filePath);
                return obj;
            }
        }
        private static Dictionary<string, Dictionary<Type, Delegate>> _3rdPartyLoaders;
        private static Dictionary<string, Dictionary<Type, Delegate>> _3rdPartyExporters;
        public static Delegate Get3rdPartyLoader(Type fileType, string extension)
        {
            extension = extension.ToLowerInvariant();
            if (_3rdPartyLoaders != null && _3rdPartyLoaders.ContainsKey(extension))
            {
                var t = _3rdPartyLoaders[extension];
                if (t.ContainsKey(fileType))
                    return t[fileType];
            }
            return null;
        }
        public static Delegate Get3rdPartyExporter(Type fileType, string extension)
        {
            extension = extension.ToLowerInvariant();
            if (_3rdPartyExporters != null && _3rdPartyExporters.ContainsKey(extension))
            {
                var t = _3rdPartyExporters[extension];
                if (t.ContainsKey(fileType))
                    return t[fileType];
            }
            return null;
        }
        public static void Register3rdPartyLoader<T>(string extension, DelThirdPartyFileMethod loadMethod) where T : TFileObject
        {
            Register3rdParty<T>(_3rdPartyLoaders, extension, loadMethod);
        }
        public static void Register3rdPartyExporter<T>(string extension, DelThirdPartyFileMethod exportMethod) where T : TFileObject
        {
            Register3rdParty<T>(_3rdPartyExporters, extension, exportMethod);
        }
        private static void Register3rdParty<T>(
            Dictionary<string, Dictionary<Type, Delegate>> methodDic, 
            string extension, 
            DelThirdPartyFileMethod method)
            where T : TFileObject
        {
            extension = extension.ToLowerInvariant();

            if (methodDic == null)
                methodDic = new Dictionary<string, Dictionary<Type, Delegate>>();

            Dictionary<Type, Delegate> typesforExt;
            if (!methodDic.ContainsKey(extension))
                methodDic.Add(extension, typesforExt = new Dictionary<Type, Delegate>());
            else
                typesforExt = methodDic[extension];
            
            Type fileType = typeof(T);
            if (!typesforExt.ContainsKey(fileType))
                typesforExt.Add(fileType, method);
            else
                throw new Exception("Registered " + extension + " for " + fileType.GetFriendlyName() + " too many times.");
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
