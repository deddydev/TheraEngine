using TheraEngine.Files.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

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

    }
    public abstract class FileObject : ObjectBase, IFileObject
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
        public FileClass FileHeader => GetFileHeader(GetType());
        public static FileClass GetFileHeader(Type t)
        {
            return Attribute.GetCustomAttribute(t, typeof(FileClass)) as FileClass;
            //throw new Exception("No FileClass attribute specified for " + t.ToString());
        }

        private List<IFileRef> _references = new List<IFileRef>();
        private string _filePath;
        private int _calculatedSize;

        public delegate FileObject DelLoadThirdPartyFile(string path);
        static FileObject()
        {
            _thirdPartyLoaders = new Dictionary<string, Dictionary<Type, DelLoadThirdPartyFile>>();
            try
            {
                var types =
                (
                    from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                    where !domainAssembly.IsDynamic
                    from assemblyType in domainAssembly.GetExportedTypes()
                    where assemblyType.IsSubclassOf(typeof(FileObject)) && !assemblyType.IsAbstract
                    select assemblyType
                )
                .ToArray();

                foreach (Type t in types)
                {
                    FileClass c = GetFileHeader(t);
                    if (c != null && c.ImportableExtensions != null)
                    {
                        foreach (string ext3rd in c.ImportableExtensions)
                        {
                            string extLower = ext3rd.ToLowerInvariant();
                            Dictionary<Type, DelLoadThirdPartyFile> d;
                            if (_thirdPartyLoaders.ContainsKey(extLower))
                                d = _thirdPartyLoaders[extLower];
                            else
                                _thirdPartyLoaders.Add(extLower, d = new Dictionary<Type, DelLoadThirdPartyFile>());
                            if (!d.ContainsKey(t))
                            {
                                var methods = t.GetMethods(
                                    BindingFlags.NonPublic |
                                    BindingFlags.Public |
                                    BindingFlags.Static)
                                    .Where(x => string.Equals(x.GetCustomAttribute<ThirdPartyLoader>()?.Extension, extLower, StringComparison.InvariantCultureIgnoreCase))
                                    .ToArray();
                                if (methods.Length > 0 && Delegate.CreateDelegate(typeof(DelLoadThirdPartyFile), methods[0]) is DelLoadThirdPartyFile result)
                                    d.Add(t, result);
                            }
                            else
                                throw new Exception(t.GetFriendlyName() + " has already been added to the third party loader list for " + extLower);
                        }
                    }
                }
            }
            catch
            {

            }
        }
        public FileObject() { }
        internal protected virtual void OnLoaded() { }

        [Category("File Object")]
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
            if (!string.IsNullOrEmpty(_filePath) && Engine.LoadedFiles.ContainsKey(_filePath))
            {
                Engine.LoadedFiles.TryRemove(_filePath, out List<FileObject> value);
            }

            List<IFileRef> oldRefs = new List<IFileRef>(_references);
            foreach (IFileRef r in oldRefs)
                if (r is ISingleFileRef)
                    ((ISingleFileRef)r).UnloadReference();
            OnUnload();
        }
        protected virtual void OnUnload() { }

        public static Type DetermineType(string path)
        {
            FileFormat f = GetFormat(path);
            switch (f)
            {
                case FileFormat.XML:
                    return CustomXmlSerializer.DetermineType(path);
                case FileFormat.Binary:
                    return CustomBinarySerializer.DetermineType(path);
                default:
                    return null;
            }
        }
        public static FileFormat GetFormat(string path)
        {
            string ext = Path.GetExtension(path).Substring(1);
            if (FileClass.Has3rdPartyExtension(ext))
                return FileFormat.ThirdParty;
            switch (ext.ToLowerInvariant()[0])
            {
                default:
                    return FileFormat.ThirdParty;
                case 'b':
                    return FileFormat.Binary;
                case 'x':
                    return FileFormat.XML;
            }
        }
        public static void GetDirNameFmt(string path, out string dir, out string name, out FileFormat fmt)
        {
            dir = Path.GetDirectoryName(path);
            name = Path.GetFileNameWithoutExtension(path);
            fmt = GetFormat(path);
        }
        public static string GetFilePath(string dir, string name, ProprietaryFileFormat format, Type fileType)
        {
            if (!dir.EndsWith("\\"))
                dir += "\\";
            return dir + name + "." + GetFileHeader(fileType).GetProperExtension(format);
        }

        #region Import/Export
        public static T FromFile<T>(string fileName) where T : FileObject
        {
            switch (GetFormat(fileName))
            {
                case FileFormat.ThirdParty:
                    return FromThirdParty<T>(fileName);
                case FileFormat.Binary:
                    return FromBinary<T>(fileName);
                case FileFormat.XML:
                    return FromXML<T>(fileName);
            }
            return default(T);
        }
        public static FileObject FromFile(Type type, string fileName)
        {
            switch (GetFormat(fileName))
            {
                case FileFormat.ThirdParty:
                    return FromThirdParty(type, fileName);
                case FileFormat.Binary:
                    return FromBinary(type, fileName);
                case FileFormat.XML:
                    return FromXML(type, fileName);
            }
            return null;
        }
        public void Export()
        {
            if (string.IsNullOrEmpty(_filePath))
                throw new Exception("File has no path to export to.");
            GetDirNameFmt(_filePath, out string dir, out string name, out FileFormat fmt);
            Export(dir, name, fmt);
        }
        public void Export(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("File path is not valid.");
            GetDirNameFmt(path, out string dir, out string name, out FileFormat fmt);
            Export(dir, name, fmt);
        }
        public void Export(string directory, string fileName, FileFormat format)
        {
            switch (format)
            {
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
        internal static unsafe FileObject FromXML(Type t, string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            FileObject file;
            if (GetFileHeader(t).ManualXmlConfigSerialize)
            {
                using (FileMap map = FileMap.FromFile(filePath))
                using (XMLReader reader = new XMLReader(map.Address, map.Length, true))
                {
                    file = SerializationCommon.CreateObject(t) as FileObject;
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
                Engine.AddLoadedFile(file._filePath, file, false);
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

            FilePath = directory + fileName + "." + FileHeader.GetProperExtension(ProprietaryFileFormat.XML);

            if (FileHeader.ManualXmlConfigSerialize)
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
        #endregion

        #region Binary

        internal unsafe static T FromBinary<T>(string filePath) where T : FileObject
            => FromBinary(typeof(T), filePath) as T;
        internal unsafe static FileObject FromBinary(Type t, string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            FileObject obj;
            if (GetFileHeader(t).ManualBinConfigSerialize)
            {
                obj = SerializationCommon.CreateObject(t) as FileObject;
                if (obj != null)
                {
                    FileMap map = FileMap.FromFile(filePath);
                    FileCommonHeader* hdr = (FileCommonHeader*)map.Address;
                    obj.Read(hdr->Data, hdr->Strings);
                }
            }
            else
                obj = CustomBinarySerializer.Deserialize(filePath, t) as FileObject;

            if (obj != null)
            {
                obj.FilePath = filePath;
                Engine.AddLoadedFile(obj._filePath, obj, false);
            }
            return obj;
        }
        internal unsafe void ToBinary(string directory, string fileName)
        {
            Type t = GetType();

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            fileName = String.IsNullOrEmpty(fileName) ? "NewFile" : fileName;

            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                directory += Path.DirectorySeparatorChar;

            FilePath = directory + fileName + "." + FileHeader.GetProperExtension(ProprietaryFileFormat.Binary);

            if (FileHeader.ManualBinConfigSerialize)
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
                        hdr->_endian = (byte)Endian.EOrder.Big;
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

        #region 3rd Party
        internal unsafe static T FromThirdParty<T>(string filePath) where T : FileObject
            => FromThirdParty(typeof(T), filePath) as T;
        internal unsafe static FileObject FromThirdParty(Type t, string filePath)
        {
            string ext = Path.GetExtension(filePath).Substring(1);
            return GetThirdPartyLoader(t, ext)?.Invoke(filePath);
        }
        private static Dictionary<string, Dictionary<Type, DelLoadThirdPartyFile>> _thirdPartyLoaders;
        public static DelLoadThirdPartyFile GetThirdPartyLoader(Type fileType, string extension)
        {
            extension = extension.ToLowerInvariant();
            if (_thirdPartyLoaders != null && _thirdPartyLoaders.ContainsKey(extension))
            {
                var t = _thirdPartyLoaders[extension];
                if (t.ContainsKey(fileType))
                    return t[fileType];
            }
            return null;
        }
        public static void RegisterThirdPartyLoader<T>(string extension, DelLoadThirdPartyFile loaderMethod) where T : FileObject
        {
            extension = extension.ToLowerInvariant();

            if (_thirdPartyLoaders == null)
                _thirdPartyLoaders = new Dictionary<string, Dictionary<Type, DelLoadThirdPartyFile>>();

            Dictionary<Type, DelLoadThirdPartyFile> typesforExt;
            if (!_thirdPartyLoaders.ContainsKey(extension))
                _thirdPartyLoaders.Add(extension, typesforExt = new Dictionary<Type, DelLoadThirdPartyFile>());
            else
                typesforExt = _thirdPartyLoaders[extension];

            Type fileType = typeof(T);
            if (!typesforExt.ContainsKey(fileType))
                typesforExt.Add(fileType, loaderMethod);
            else
                throw new Exception("Registered " + extension + " for " + fileType.GetFriendlyName() + " too many times.");
        }
        #endregion
    }
}
