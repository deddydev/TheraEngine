using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;

namespace CustomEngine.Files
{
    public enum FileFormat
    {
        Binary      = 0,
        XML         = 1,
        ThirdParty  = 2,
        Programatic = 3,
    }
    public abstract class FileObject : ObjectBase
    {
        public FileObject() { OnLoaded(); }
        
        public List<IFileRef> _references = new List<IFileRef>();

        public string FilePathAbsolute { get { return Engine.StartupPath + _filePath; } }
        public string FilePathRelative { get { return _filePath; } }
        internal string _filePath;

        private int _calculatedSize;
        bool _isCalculatingSize, _isWriting;

        public int CalculatedSize { get { return _calculatedSize; } }

        public bool IsCalculatingSize { get { return _isCalculatingSize; } }
        public bool IsSaving { get { return IsCalculatingSize || IsWriting; } }
        public bool IsWriting { get { return _isWriting; } }

        public abstract ResourceType ResourceType { get; }

        public virtual int CalculateSize(StringTable table)
        {
            return FileCommonHeader.Size;
        }
        public abstract void Write(VoidPtr address, StringTable table);
        public abstract void Read(VoidPtr address, VoidPtr strings);
        public abstract void Write(XmlWriter writer);
        public abstract void Read(XMLReader reader);
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
        protected virtual void OnUnload() { }

        public void Export(bool asXML)
        {
            if (string.IsNullOrEmpty(_filePath))
                throw new Exception("No path to export to.");
            string directory = Path.GetDirectoryName(_filePath);
            if (asXML)
                ToXML(directory);
            else
                ToBinary(directory);
        }
        public void Export(string directory, bool asXML)
        {
            if (asXML)
                ToXML(directory);
            else
                ToBinary(directory);
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
        public unsafe void ToBinary(string filePath)
        {
            Type t = GetType();
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            filePath = directory + "\\" + _name + ".b" + FileManager.GetExtension(t);
            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 8, FileOptions.SequentialScan))
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
        }
        public static T FromXML<T>(string filePath) where T : FileObject
        {
            return FromXML(typeof(T), filePath) as T;
        }
        public static unsafe FileObject FromXML(Type t, string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            using (FileMap map = FileMap.FromFile(filePath))
            using (XMLReader reader = new XMLReader(map.Address, map.Length))
            {
                FileObject obj = Activator.CreateInstance(t) as FileObject;
                obj._filePath = filePath;

                if (reader.BeginElement())
                {
                    if (reader.Name.Equals(t.ToString(), true))
                        obj.Read(reader);
                    else
                        throw new Exception("File was not of expected type.");
                    reader.EndElement();
                }

                Engine.AddLoadedFile(obj._filePath, obj);

                return obj;
            }
        }
        private static XmlWriterSettings _writerSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace
        };

        public void ToXML(string filePath)
        {
            _isWriting = true;
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            filePath = directory + "\\" + _name + ".x" + FileManager.GetExtension(GetType());
            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.SequentialScan))
            using (XmlWriter writer = XmlWriter.Create(stream, _writerSettings))
            {
                writer.Flush();
                stream.Position = 0;

                writer.WriteStartDocument();
                Write(writer);
                writer.WriteEndDocument();
            }
            _isWriting = false;
        }
        protected void WriteSettings(VoidPtr address, StringTable table)
        {
            var info = GetType().GetFields().Where(
                prop => Attribute.IsDefined(prop, typeof(Default)));
        }
        protected void WriteState(VoidPtr address, StringTable table)
        {
            var info = GetType().GetFields().Where(
                prop => Attribute.IsDefined(prop, typeof(State)));
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FileCommonHeader
    {
        public const int Size = 0xC;

        public buint _tag;
        public bint _fileLength;
        public bint _stringTableLength;

        public string Tag
        {
            get { return new string((sbyte*)_tag.Address); }
            set
            {
                sbyte* dPtr = (sbyte*)Address;
                for (int i = 0; i < 4; ++i)
                    *dPtr++ = (sbyte)value[i >= value.Length ? ' ' : value[i]];
            }
        }
        public VoidPtr Strings { get { return Address + Size; } }
        public VoidPtr FileHeader { get { return Address + Size + _stringTableLength; } }
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}
