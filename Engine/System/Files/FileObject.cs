using System;
using System.Collections.Generic;
using System.IO;
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
        internal string _filePath;

        private int _calculatedSize;
        bool _isCalculatingSize, _isWriting;

        public int CalculatedSize { get { return _calculatedSize; } }

        public bool IsCalculatingSize { get { return _isCalculatingSize; } }
        public bool IsSaving { get { return IsCalculatingSize || IsWriting; } }
        public bool IsWriting { get { return _isWriting; } }

        public virtual int CalculateSize(StringTable table)
        {
            return FileCommonHeader.Size;
        }
        public virtual unsafe void Write(VoidPtr address) { }
        public virtual unsafe void Read(VoidPtr address) { }
        public virtual void Write(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().ToString());
            if (!string.IsNullOrEmpty(_name))
                writer.WriteAttributeString("name", _name);
        }
        public virtual void Read(XmlReader reader) { }
        public void Unload()
        {
            if (!string.IsNullOrEmpty(_filePath) && Engine.LoadedFiles.ContainsKey(_filePath))
                Engine.LoadedFiles.Remove(_filePath);
            List<IFileRef> oldRefs = new List<IFileRef>(_references);
            oldRefs.ForEach(x => x.UnloadReference());
            OnUnload();
        }
        protected virtual void OnUnload() { }
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
            obj.Read(hdr->FileHeader);
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
                int size = CalculateSize(table).Align(4);
                size += table.GetTotalSize();
                stream.SetLength(size);
                using (FileMap map = FileMap.FromStream(stream))
                {
                    FileCommonHeader* hdr = (FileCommonHeader*)map.Address;
                    hdr->Tag = FileManager.GetTag(t);
                    hdr->_length = size;
                    Write(hdr->FileHeader);
                }
            }
        }
        public static T FromXML<T>(string filePath) where T : FileObject
        {
            return FromXML(typeof(T), filePath) as T;
        }
        public static FileObject FromXML(Type t, string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            using (TextReader stream = new StreamReader(filePath))
            using (XmlReader reader = XmlReader.Create(stream))
            {
                reader.MoveToContent();

                FileObject obj = Activator.CreateInstance(t) as FileObject;
                obj.Read(reader);

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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FileCommonHeader
    {
        public const int Size = 8;

        public buint _tag;
        public bint _length;

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
        public VoidPtr FileHeader { get { return Address + Size; } }
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}
