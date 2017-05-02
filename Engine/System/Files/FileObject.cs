using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace CustomEngine.Files
{
    public enum FileFormat
    {
        Binary      = 0,
        XML         = 1,
        ThirdParty  = 2,
        Programatic = 3,
    }
    [FileClass("", "", "")]
    public abstract class FileObject : ObjectBase
    {
        public FileClass FileHeader
            => GetFileHeader(GetType());
        public static FileClass GetFileHeader(Type t)
            => (FileClass)Attribute.GetCustomAttribute(t, typeof(FileClass));
        
        public FileObject() { OnLoaded(); }
        
        public List<IFileRef> _references = new List<IFileRef>();

        internal string _filePath;

        private int _calculatedSize;
        bool _isCalculatingSize, _isWriting;

        public string FilePath => _filePath;
        public int CalculatedSize => _calculatedSize;

        public bool IsCalculatingSize => _isCalculatingSize;
        public bool IsSaving => IsCalculatingSize || IsWriting;
        public bool IsWriting => _isWriting;

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

        public void Export()
        {
            if (string.IsNullOrEmpty(_filePath))
                throw new Exception("No path to export to.");
            char ext = Path.GetExtension(_filePath)[1];
            Export(ext == 'x');
        }
        public void Export(bool asXML)
        {
            if (string.IsNullOrEmpty(_filePath))
                throw new Exception("No path to export to.");
            string directory = Path.GetDirectoryName(_filePath);
            if (asXML)
                ToXML(directory, _name);
            else
                ToBinary(directory, _name);
        }
        public void Export(bool asXML, string fileName)
        {
            if (string.IsNullOrEmpty(_filePath))
                throw new Exception("No path to export to.");
            string directory = Path.GetDirectoryName(_filePath);
            if (asXML)
                ToXML(directory, fileName);
            else
                ToBinary(directory, fileName);
        }
        public void Export(string directory, bool asXML)
        {
            if (string.IsNullOrEmpty(directory))
                throw new Exception("No path to export to.");
            if (asXML)
                ToXML(directory, _name);
            else
                ToBinary(directory, _name);
        }
        public void Export(string directory, string fileName, bool asXML)
        {
            if (asXML)
                ToXML(directory, fileName);
            else
                ToBinary(directory, fileName);
        }

        protected virtual void OnLoaded() { }
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
            obj.Read(hdr->FileHeader, hdr->Strings);

            Engine.AddLoadedFile(obj._filePath, obj);

            return obj;
        }
        private unsafe void ToBinary(string directory, string fileName)
        {
            Type t = GetType();
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            fileName = String.IsNullOrEmpty(fileName) ? "NewFile" : fileName;
            directory = directory + "\\" + fileName + ".b" + FileHeader.Extension.ToLower();
            using (FileStream stream = new FileStream(directory, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 8, FileOptions.SequentialScan))
            {
                if (FileHeader.ManualBinSerialize)
                {
                    StringTable table = new StringTable();
                    int dataSize = CalculateSize(table).Align(4);
                    int stringSize = table.GetTotalSize();
                    int totalSize = dataSize + stringSize;
                    stream.SetLength(totalSize);
                    using (FileMap map = FileMap.FromStream(stream))
                    {
                        FileCommonHeader* hdr = (FileCommonHeader*)map.Address;
                        hdr->Tag = FileManager.GetTag(t);
                        table.WriteTable(hdr);
                        hdr->_fileLength = totalSize;
                        hdr->_stringTableLength = stringSize;
                        Write(hdr->FileHeader, table);
                    }
                }
                else
                {
                    CustomBinarySerializer s = new CustomBinarySerializer();
                    s.Serialize(this, stream);
                }
            }
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
            _isWriting = true;
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            fileName = String.IsNullOrEmpty(fileName) ? "NewFile" : fileName;
            if (!directory.EndsWith("\\"))
                directory += "\\";
            directory = directory + fileName + ".x" + FileHeader.Extension.ToLower();

            if (FileHeader.ManualXmlSerialize)
            {
                using (FileStream stream = new FileStream(directory, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.SequentialScan))
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
            {
                CustomXmlSerializer.Serialize(this, directory);
            }
            
            _isWriting = false;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FileCommonHeader
    {
        public const int Size = 0x10;

        public buint _tag;
        public bint _nameOffset;
        public bint _fileLength;
        public bint _stringTableLength;

        public string Tag
        {
            get { return new string((sbyte*)_tag.Address); }
            set
            {
                sbyte* dPtr = (sbyte*)Address;
                for (int i = 0; i < 4; ++i)
                    *dPtr++ = (sbyte)(i >= value.Length ? ' ' : value[i]);
            }
        }
        public VoidPtr Strings => Address + Size;
        public VoidPtr FileHeader => Address + Size + _stringTableLength;
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}
