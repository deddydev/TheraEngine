using TheraEngine.Files.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System;
using System.Linq;
using System.Reflection;
using TheraEngine.Core.Reflection.Attributes;

namespace TheraEngine.Files
{
    public enum ProprietaryFileFormat
    {
        Binary = 0,
        XML = 1,
    }
    public enum FileFormat
    {
        Binary      = 0,
        XML         = 1,
        ThirdParty  = 2,
        //Programatic = 3,
    }
    public interface IFileObject : IObjectBase
    {
        string FilePath { get; set; }
        List<IFileRef> References { get; set; }
        FileDef FileDefinition { get; }
        FileExt FileExtension { get; }
        File3rdParty File3rdPartyExtensions { get; }
        void Unload();
        void Export();
        void Export(string path);
        void Export(string directory, string fileName, FileFormat format, string thirdPartyExt = null);
        string GetFilePath(string dir, string name, ProprietaryFileFormat format);
        string GetFilter(bool proprietary = true, bool import3rdParty = false, bool export3rdParty = false);
    }
    //[FileExt("tasset")]
    //[FileDef("Thera Engine Asset")]
    public abstract class FileObject : TObject, IFileObject
    {
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class ThirdPartyLoader : Attribute
        {
            public string Extension { get; private set; }
            public ThirdPartyLoader(string extension)
            {
                Extension = extension;
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
            //throw new Exception("No FileExt attribute specified for " + t.ToString());
        }
        [Browsable(false)]
        public File3rdParty File3rdPartyExtensions => GetFile3rdPartyExtensions(GetType());
        public static File3rdParty GetFile3rdPartyExtensions(Type classType)
        {
            return Attribute.GetCustomAttribute(classType, typeof(File3rdParty), true) as File3rdParty;
            //throw new Exception("No File3rdParty attribute specified for " + t.ToString());
        }

        private List<IFileRef> _references = new List<IFileRef>();
        private string _filePath;
        private int _calculatedSize;

        public delegate FileObject DelThirdPartyFileMethod(string path);
        static FileObject()
        {
            _3rdPartyLoaders = new Dictionary<string, Dictionary<Type, DelThirdPartyFileMethod>>();
            _3rdPartyExporters = new Dictionary<string, Dictionary<Type, DelThirdPartyFileMethod>>();
            try
            {
                var types = Engine.FindTypes(t => t.IsSubclassOf(typeof(FileObject)) && !t.IsAbstract).ToArray();
                foreach (Type t in types)
                {
                    File3rdParty attrib = GetFile3rdPartyExtensions(t);
                    if (attrib == null)
                        continue;
                    foreach (string ext3rd in attrib.ImportableExtensions)
                    {
                        string extLower = ext3rd.ToLowerInvariant();
                        Dictionary<Type, DelThirdPartyFileMethod> d;
                        if (_3rdPartyLoaders.ContainsKey(extLower))
                            d = _3rdPartyLoaders[extLower];
                        else
                            _3rdPartyLoaders.Add(extLower, d = new Dictionary<Type, DelThirdPartyFileMethod>());
                        if (!d.ContainsKey(t))
                        {
                            var methods = t.GetMethods(
                                BindingFlags.NonPublic |
                                BindingFlags.Public |
                                BindingFlags.Static)
                                .Where(x => string.Equals(x.GetCustomAttribute<ThirdPartyLoader>()?.Extension, extLower, StringComparison.InvariantCultureIgnoreCase))
                                .ToArray();
                            if (methods.Length > 0 && Delegate.CreateDelegate(typeof(DelThirdPartyFileMethod), methods[0]) is DelThirdPartyFileMethod result)
                                d.Add(t, result);
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
        public FileObject() { }
        internal protected virtual void OnLoaded() { }

        [TString(false, true, false)]
        [Category("Object")]
        public string FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }
        [Browsable(false)]
        public int CalculatedSize => _calculatedSize;
        [Browsable(false)]
        public List<IFileRef> References { get => _references; set => _references = value; }

        public void Unload()
        {
            List<IFileRef> oldRefs = new List<IFileRef>(_references);
            foreach (IFileRef r in oldRefs)
                r.UnloadReference();
            OnUnload();
        }
        protected virtual void OnUnload() { }

        public static Type DetermineType(string path)
        {
            FileFormat f = GetFormat(path, out string ext);
            switch (f)
            {
                case FileFormat.XML:
                    return CustomXmlSerializer.DetermineType(path);
                case FileFormat.Binary:
                    return CustomBinarySerializer.DetermineType(path);
                default:
                    Type[] types = DetermineThirdPartyTypes(ext);
                    return types.Length > 0 ? types[0] : null;
            }
        }

        public static Type[] DetermineThirdPartyTypes(string ext)
        {
            return Engine.FindTypes(t => 
            typeof(FileObject).IsAssignableFrom(t) && 
            (t.GetCustomAttribute<File3rdParty>()?.HasExtension(ext) ?? false)).ToArray();
        }

        public static FileFormat GetFormat(string path, out string ext)
        {
            int index = path.LastIndexOf('.') + 1;
            if (index != 0)
                ext = path.Substring(index).ToLowerInvariant();
            else
                ext = path.ToLowerInvariant();
            if (File3rdParty.Has3rdPartyExtension(ext))
                return FileFormat.ThirdParty;
            switch (ext[0])
            {
                default: return FileFormat.ThirdParty;
                case 'b': return FileFormat.Binary;
                case 'x': return FileFormat.XML;
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
        public static void GetDirNameFmt(string path, out string dir, out string name, out FileFormat fmt, out string ext)
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
        public static string GetFilePath(string dir, string name, ProprietaryFileFormat format, Type fileType)
        {
            return Path.Combine(dir, name + "." + GetFileExtension(fileType).GetProperExtension(format));
        }
        public static string GetFilePath<T>(string dir, string name, ProprietaryFileFormat format) where T : FileObject
        {
            return Path.Combine(dir, name + "." + GetFileExtension(typeof(T)).GetProperExtension(format));
        }
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
        public string GetFilePath(string dir, string name, ProprietaryFileFormat format)
        {
            return GetFilePath(dir, name, format, GetType());
        }

        public string GetFilter(bool proprietary = true, bool import3rdParty = false, bool export3rdParty = false)
        {
            return GetFilter(GetType(), proprietary, import3rdParty, export3rdParty);
        }
        public static string GetFilter<T>(bool proprietary = true, bool import3rdParty = false, bool export3rdParty = false) where T : FileObject
        {
            return GetFilter(typeof(T), proprietary, import3rdParty, export3rdParty);
        }
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
                foreach (string type in Enum.GetNames(typeof(ProprietaryFileFormat)))
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
        /// <summary>
        /// Opens a new instance of the given file at the given file path.
        /// </summary>
        /// <typeparam name="T">The type of the file object to load.</typeparam>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>A new instance of the file.</returns>
        public static T Load<T>(string filePath) where T : FileObject
        {
            switch (GetFormat(filePath, out string ext))
            {
                case FileFormat.ThirdParty:
                    return Read3rdParty<T>(filePath);
                case FileFormat.Binary:
                    return FromBinary<T>(filePath);
                case FileFormat.XML:
                    return FromXML<T>(filePath);
            }
            return default(T);
        }
        /// <summary>
        /// Opens a new instance of the file object at the given file path.
        /// </summary>
        /// <param name="type">The type of the file object to load.</param>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>A new instance of the file.</returns>
        public static FileObject Load(Type type, string filePath)
        {
            switch (GetFormat(filePath, out string ext))
            {
                case FileFormat.ThirdParty:
                    return Read3rdParty(type, filePath);
                case FileFormat.Binary:
                    return FromBinary(type, filePath);
                case FileFormat.XML:
                    return FromXML(type, filePath);
            }
            return null;
        }
        [GridCallable("Save")]
        public void Export()
        {
            if (string.IsNullOrEmpty(_filePath) || !_filePath.IsValidPath())
            {
                Engine.LogWarning("File was not exported; no path to export to.");
                return;
            }
            GetDirNameFmt(_filePath, out string dir, out string name, out FileFormat fmt, out string thirdPartyExt);
            Export(dir, name, fmt, thirdPartyExt);
        }
        [GridCallable("Save")]
        public void Export(string path)
        {
            if (string.IsNullOrEmpty(path) || !path.IsValidPath())
            {
                Engine.LogWarning("File was not exported; file path is not valid.");
                return;
            }
            GetDirNameFmt(path, out string dir, out string name, out FileFormat fmt, out string thirdPartyExt);
            Export(dir, name, fmt, thirdPartyExt);
        }
        [GridCallable("Save")]
        public void Export(string directory, string fileName)
        {
            string ext = null;
            FileExt fileExt = FileExtension;
            if (fileExt != null)
            {
                ext = fileExt.GetProperExtension((ProprietaryFileFormat)fileExt.PreferredFormat);
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
                FileFormat format = GetFormat(ext, out string ext2);
                Export(directory, fileName, format, ext);
            }
            else
                Engine.LogWarning("File was not exported; cannot assume extension for {0}.", GetType().GetFriendlyName());
        }
        [GridCallable("Save")]
        public void Export(string directory, string fileName, FileFormat format, string thirdPartyExt = null)
        {
            switch (format)
            {
                case FileFormat.ThirdParty:
                    To3rdParty(directory, fileName, thirdPartyExt);
                    break;
                case FileFormat.XML:
                    ToXML(directory, fileName);
                    break;
                case FileFormat.Binary:
                    ToBinary(directory, fileName);
                    break;
                default:
                    throw new InvalidOperationException("Not a valid file format.");
            }
        }

        #endregion

        #region XML
        internal static T FromXML<T>(string filePath) where T : FileObject
            => FromXML(typeof(T), filePath) as T;
        internal static unsafe FileObject FromXML(Type type, string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            FileObject file;
            if (GetFileExtension(type).ManualXmlConfigSerialize)
            {
                using (FileMap map = FileMap.FromFile(filePath))
                using (XMLReader reader = new XMLReader(map.Address, map.Length, true))
                {
                    file = SerializationCommon.CreateObject(type) as FileObject;
                    if (file != null && reader.BeginElement())
                    {
                        //if (reader.Name.Equals(t.ToString(), true))
                        file.Read(reader);
                        //else
                        //    throw new Exception("File was not of expected type.");
                        reader.EndElement();
                    }
                }
            }
            else
                file = CustomXmlSerializer.Deserialize(filePath) as FileObject;
            if (file != null)
            {
                file.FilePath = filePath;
                file.OnLoaded();
            }
            return file;
        }
        private static XmlWriterSettings _writerSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineChars = Environment.NewLine,
            NewLineHandling = NewLineHandling.Replace
        };
        internal void ToXML(string directory, string fileName)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            fileName = String.IsNullOrEmpty(fileName) ? "NewFile" : fileName;

            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                directory += Path.DirectorySeparatorChar;

            FileExt ext = FileExtension;

            FilePath = directory + fileName + "." + ext.GetProperExtension(ProprietaryFileFormat.XML);

            if (ext.ManualXmlConfigSerialize)
            {
                using (FileStream stream = new FileStream(_filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.SequentialScan))
                using (XmlWriter writer = XmlWriter.Create(stream, _writerSettings))
                {
                    writer.Flush();
                    stream.Position = 0;

                    writer.WriteStartDocument();
                    Write(writer);
                    writer.WriteEndDocument();
                }
            }
            else
                CustomXmlSerializer.Serialize(this, _filePath);
        }
        /// <summary>
        /// Writes this object to an xml file using the given xml writer.
        /// Override if the FileClass attribute for this class specifies ManualXmlSerialize.
        /// </summary>
        /// <param name="writer">The xml writer to write the file with.</param>
        internal protected virtual void Write(XmlWriter writer)
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

        internal unsafe static T FromBinary<T>(string filePath) where T : FileObject
            => FromBinary(typeof(T), filePath) as T;
        internal unsafe static FileObject FromBinary(Type type, string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            FileObject file;
            if (GetFileExtension(type).ManualBinConfigSerialize)
            {
                file = SerializationCommon.CreateObject(type) as FileObject;
                if (file != null)
                {
                    FileMap map = FileMap.FromFile(filePath);
                    FileCommonHeader* hdr = (FileCommonHeader*)map.Address;
                    file.Read(hdr->Data, hdr->Strings);
                }
            }
            else
                file = CustomBinarySerializer.Deserialize(filePath, type) as FileObject;

            if (file != null)
            {
                file.FilePath = filePath;
                file.OnLoaded();
            }
            return file;
        }
        internal unsafe void ToBinary(string directory, string fileName)
        {
            Type t = GetType();

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            fileName = String.IsNullOrEmpty(fileName) ? "NewFile" : fileName;

            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                directory += Path.DirectorySeparatorChar;

            FileExt ext = FileExtension;

            FilePath = directory + fileName + "." + ext.GetProperExtension(ProprietaryFileFormat.Binary);

            if (ext.ManualBinConfigSerialize)
            {
                using (FileStream stream = new FileStream(_filePath,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.ReadWrite,
                    8,
                    FileOptions.RandomAccess))
                {
                    StringTable table = new StringTable();
                    int dataSize = CalculateSize(table).Align(4);
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
                        Write(hdr->Data, table);
                    }
                }
            }
            else
            {
                CustomBinarySerializer.Serialize(this, _filePath, Endian.EOrder.Big, true, true, "test", out byte[] encryptionSalt, out byte[] integrityHash, null);
            }
        }

        /// <summary>
        /// Calculates the size of this object, in bytes.
        /// </summary>
        /// <param name="table">The string table to populate with strings.</param>
        /// <returns>The size of the object, in bytes.</returns>
        internal int CalculateSize(StringTable table)
        {
            _calculatedSize = OnCalculateSize(table);
            return _calculatedSize;
        }
        /// <summary>
        /// Calculates the size of this object, in bytes.
        /// Override if the FileClass attribute for this class specifies ManualBinSerialize.
        /// </summary>
        /// <param name="table">The string table. Add strings to this as you wish, and use their addresses when writing later.</param>
        /// <returns>The size of the object, in bytes.</returns>
        protected virtual int OnCalculateSize(StringTable table)
            => throw new NotImplementedException("Override of \"protected virtual int OnCalculateSize(StringTable table)\" required when using ManualBinarySerialize in FileClass attribute.");
        /// <summary>
        /// Writes this object to the given address.
        /// The size of this object is CalculatedSize.
        /// Override if the FileClass attribute for this class specifies ManualBinSerialize.
        /// </summary>
        /// <param name="address">The address to write to.</param>
        /// <param name="table">The table of all strings added in OnCalculateSize.</param>
        internal protected virtual void Write(VoidPtr address, StringTable table)
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
            Write3rdParty(GetFilePath(directory, fileName, thirdPartyExt));
        }
        internal unsafe static T Read3rdParty<T>(string filePath) where T : FileObject
            => Read3rdParty(typeof(T), filePath) as T;
        internal unsafe static FileObject Read3rdParty(Type classType, string filePath)
        {
            string ext = Path.GetExtension(filePath).Substring(1);
            return Get3rdPartyLoader(classType, ext)?.Invoke(filePath);
        }
        private static Dictionary<string, Dictionary<Type, DelThirdPartyFileMethod>> _3rdPartyLoaders;
        private static Dictionary<string, Dictionary<Type, DelThirdPartyFileMethod>> _3rdPartyExporters;
        public static DelThirdPartyFileMethod Get3rdPartyLoader(Type fileType, string extension)
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
        public static DelThirdPartyFileMethod Get3rdPartyExporter(Type fileType, string extension)
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
        public static void Register3rdPartyLoader<T>(string extension, DelThirdPartyFileMethod loadMethod) where T : FileObject
        {
            Register3rdParty<T>(_3rdPartyLoaders, extension, loadMethod);
        }
        public static void Register3rdPartyExporter<T>(string extension, DelThirdPartyFileMethod exportMethod) where T : FileObject
        {
            Register3rdParty<T>(_3rdPartyExporters, extension, exportMethod);
        }
        private static void Register3rdParty<T>(Dictionary<string, Dictionary<Type, DelThirdPartyFileMethod>> methodDic, string extension, DelThirdPartyFileMethod method) where T : FileObject
        {
            extension = extension.ToLowerInvariant();

            if (methodDic == null)
                methodDic = new Dictionary<string, Dictionary<Type, DelThirdPartyFileMethod>>();

            Dictionary<Type, DelThirdPartyFileMethod> typesforExt;
            if (!methodDic.ContainsKey(extension))
                methodDic.Add(extension, typesforExt = new Dictionary<Type, DelThirdPartyFileMethod>());
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
        internal protected virtual void Write3rdParty(string filePath)
            => throw new NotImplementedException("Override of \"internal protected virtual void WriteThirdParty(string filePath)\" required when 'IsThirdParty' is true in FileClass attribute.");
        /// <summary>
        /// When 'IsThirdParty' is true in the FileClass attribute, this method is called to read the object from a path.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        internal protected virtual void Read3rdParty(string filePath)
            => throw new NotImplementedException("Override of \"internal protected virtual void ReadThirdParty(string filePath)\" required when 'IsThirdParty' is true in FileClass attribute.");
        #endregion
    }
}
