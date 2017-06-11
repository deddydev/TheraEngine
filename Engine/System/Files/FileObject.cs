using TheraEngine.Files.Serialization;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System;
using SevenZip;

namespace TheraEngine.Files
{
    public enum FileFormat
    {
        Binary      = 0,
        XML         = 1,
        //ThirdParty  = 2,
        //Programatic = 3,
    }

    //TODO: this class can probably be removed, since any class can be serialized

    [FileClass("", "")]
    public abstract class FileObject : ObjectBase, ICodeProgress
    {
        [Browsable(false)]
        public FileClass FileHeader
            => GetFileHeader(GetType());
        public static FileClass GetFileHeader(Type t)
            => (FileClass)Attribute.GetCustomAttribute(t, typeof(FileClass));

        private List<IFileRef> _references = new List<IFileRef>();
        private string _filePath;
        private int _calculatedSize;

        public FileObject() { OnLoaded(); }
        protected virtual void OnLoaded() { }

        public string FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }
        [Browsable(false)]
        public int CalculatedSize => _calculatedSize;
        [Browsable(false)]
        public List<IFileRef> References { get => _references; set => _references = value; }

        public int CalculateSize(StringTable table)
        {
            _calculatedSize = OnCalculateSize(table);
            return _calculatedSize;
        }
        protected virtual int OnCalculateSize(StringTable table) => throw new NotImplementedException();
        public virtual void Write(VoidPtr address, StringTable table) => throw new NotImplementedException();
        public virtual void Read(VoidPtr address, VoidPtr strings) => throw new NotImplementedException();
        public virtual void Write(XmlWriter writer) => throw new NotImplementedException();
        public virtual void Read(XMLReader reader) => throw new NotImplementedException();
        public void Unload()
        {
            if (!string.IsNullOrEmpty(_filePath) && Engine.LoadedFiles.ContainsKey(_filePath))
                Engine.LoadedFiles.Remove(_filePath);
            List<IFileRef> oldRefs = new List<IFileRef>(_references);
            foreach (IFileRef r in oldRefs)
                if (r is ISingleFileRef)
                    ((ISingleFileRef)r).UnloadReference();
            OnUnload();
        }

        public static T Import<T>(string fileName) where T : FileObject
            => Import<T>(fileName, GetFormat(fileName));
        
        public static T Import<T>(string fileName, FileFormat format) where T : FileObject
        {
            switch (format)
            {
                case FileFormat.Binary:
                    return FromBinary<T>(fileName);
                case FileFormat.XML:
                    return FromXML<T>(fileName);
            }
            return default(T);
        }
        public static FileObject Import(Type t, string fileName, FileFormat format)
        {
            switch (format)
            {
                case FileFormat.Binary:
                    return FromBinary(t, fileName);
                case FileFormat.XML:
                    return FromXML(t, fileName);
            }
            return null;
        }

        protected virtual void OnUnload() { }
        
        public void Export()
        {
            if (string.IsNullOrEmpty(_filePath))
                throw new Exception("File has no path to export to.");
            GetDirNameFmt(_filePath, out string dir, out string name, out FileFormat fmt);
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
            }
        }

        public unsafe static T FromBinary<T>(string filePath) where T : FileObject
            => FromBinary(typeof(T), filePath) as T;
        public unsafe static FileObject FromBinary(Type t, string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            FileObject obj;
            if (GetFileHeader(t).ManualBinSerialize)
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
                obj._filePath = filePath;
                Engine.AddLoadedFile(obj._filePath, obj);
            }
            return obj;
        }
        private unsafe void ToBinary(string directory, string fileName)
        {
            Type t = GetType();

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            fileName = String.IsNullOrEmpty(fileName) ? "NewFile" : fileName;

            if (!directory.EndsWith("\\"))
                directory += "\\";

            _filePath = directory + fileName + "." + FileHeader.GetProperExtension(FileFormat.Binary);

            if (FileHeader.ManualBinSerialize)
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
        public static FileFormat GetFormat(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            switch (ext[1])
            {
                default:
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
        public static string GetFilePath(string dir, string name, FileFormat format, Type fileType)
        {
            return dir + "\\" + name + "." + GetFileHeader(fileType).GetProperExtension(format);
        }

        public static T FromXML<T>(string filePath) where T : FileObject
            => FromXML(typeof(T), filePath) as T;
        public static unsafe FileObject FromXML(Type t, string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            FileObject obj;
            if (GetFileHeader(t).ManualXmlSerialize)
            {
                using (FileMap map = FileMap.FromFile(filePath))
                using (XMLReader reader = new XMLReader(map.Address, map.Length))
                {
                    obj = SerializationCommon.CreateObject(t) as FileObject;
                    if (obj != null && reader.BeginElement())
                    {
                        //if (reader.Name.Equals(t.ToString(), true))
                        obj.Read(reader);
                        //else
                        //    throw new Exception("File was not of expected type.");
                        reader.EndElement();
                    }
                }
            }
            else
                obj = (FileObject)CustomXmlSerializer.Deserialize(filePath, t);
            if (obj != null)
            {
                obj._filePath = filePath;
                Engine.AddLoadedFile(obj._filePath, obj);
            }
            return obj;
        }
        private static XmlWriterSettings _writerSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace
        };

        private void ToXML(string directory, string fileName)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            fileName = String.IsNullOrEmpty(fileName) ? "NewFile" : fileName;

            if (!directory.EndsWith("\\"))
                directory += "\\";

            _filePath = directory + fileName + "." + FileHeader.GetProperExtension(FileFormat.XML);

            if (FileHeader.ManualXmlSerialize)
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

        public void SetProgress(long inSize, long outSize)
        {

        }
    }
}
