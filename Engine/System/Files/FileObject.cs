using CustomEngine.Files.Serialization;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System;

namespace CustomEngine.Files
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CompressionHeader
    {
        public const int Size = 0x4;

        public Bin32 _flags;

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FileCommonHeader
    {
        public const int Size = 0x8;
        
        public bint _fileLength;
        public bint _stringTableLength;

        public VoidPtr Strings => Address + Size;
        public VoidPtr Data => Address + Size + _stringTableLength;
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
    public enum FileFormat
    {
        Binary      = 0,
        XML         = 1,
        //ThirdParty  = 2,
        //Programatic = 3,
    }
    [FileClass("", "")]
    public abstract class FileObject : ObjectBase
    {
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
            internal set => _filePath = value;
        }
        public int CalculatedSize => _calculatedSize;

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
        {
            return Import<T>(fileName, Path.GetExtension(fileName)[1] == 'x');
        }
        public static T Import<T>(string fileName, bool isXML) where T : FileObject
        {
            if (isXML)
                return FromXML<T>(fileName);
            else
                return FromBinary<T>(fileName);
        }
        public static FileObject Import(Type t, string fileName, bool isXML)
        {
            if (isXML)
                return FromXML(t, fileName);
            else
                return FromBinary(t, fileName);
        }

        protected virtual void OnUnload() { }
        
        public void Export(FileFormat format)
        {
            if (string.IsNullOrEmpty(_filePath))
                throw new Exception("File has no path to export to.");
            Export(Path.GetDirectoryName(_filePath), Path.GetFileNameWithoutExtension(_filePath), format);
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
        {
            return FromBinary(typeof(T), filePath) as T;
        }
        public unsafe static FileObject FromBinary(Type t, string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            FileMap map = FileMap.FromFile(filePath);
            FileCommonHeader* hdr = (FileCommonHeader*)map.Address;
            string tag = hdr->Tag;

            Type t2 = FileManager.GetType(tag);
            if (t != t2)
                throw new Exception("Type mismatch: want " + t.ToString() + ", got " + t2.ToString());

            FileObject obj = Activator.CreateInstance(t) as FileObject;
            obj._filePath = filePath;
            obj.Read(hdr->Data, hdr->Strings);

            Engine.AddLoadedFile(obj._filePath, obj);

            return obj;
        }
        private unsafe void ToBinary(string directory, string fileName)
        {
            Type t = GetType();
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            fileName = String.IsNullOrEmpty(fileName) ? "NewFile" : fileName;
            _filePath = directory + "\\" + fileName + ".b" + FileHeader.Extension.ToLower();

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
                        Write(hdr->Data, table);
                    }
                }
            }
            else
                CustomBinarySerializer.Serialize(this, _filePath, Endian.EOrder.Big, false, null);
        }
        public static T FromXML<T>(string filePath) where T : FileObject
        {
            return FromXML(typeof(T), filePath) as T;
        }
        public static unsafe FileObject FromXML(Type t, string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            if (GetFileHeader(t).ManualXmlSerialize)
            {
                using (FileMap map = FileMap.FromFile(filePath))
                using (XMLReader reader = new XMLReader(map.Address, map.Length))
                {
                    FileObject obj = Activator.CreateInstance(t) as FileObject;
                    obj._filePath = filePath;

                    if (reader.BeginElement())
                    {
                        //if (reader.Name.Equals(t.ToString(), true))
                        obj.Read(reader);
                        //else
                        //    throw new Exception("File was not of expected type.");
                        reader.EndElement();
                    }

                    Engine.AddLoadedFile(obj._filePath, obj);

                    return obj;
                }
            }
            else
            {
                return (FileObject)CustomXmlSerializer.Deserialize(filePath, t);
            }
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
            _filePath = directory + fileName + ".x" + FileHeader.Extension.ToLower();

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
    }
}
