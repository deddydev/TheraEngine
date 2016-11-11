using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Reflection;
using System.Runtime.InteropServices;
using CustomEngine.Worlds;

namespace System
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
        FileFormat _format = FileFormat.Programatic;
        public FileObject()
        {
            _filePath = null;
        }
        public FileObject(FileFormat format, string path)
        {
            _format = format;
            _filePath = path;
        }

        protected bool _isLoaded = false, _isLoading = false;
        protected string _filePath;

        private int _calculatedSize;
        bool _isCalculatingSize, _isWriting;

        public int CalculatedSize { get { return _calculatedSize; } }

        public bool IsCalculatingSize { get { return _isCalculatingSize; } }
        public bool IsSaving { get { return IsCalculatingSize || IsWriting; } }
        public bool IsWriting { get { return _isWriting; } }

        public virtual int CalculateSize()
        {
            return FileCommonHeader.Size;
        }
        protected virtual unsafe void Write(VoidPtr address)
        {
            
        }
        protected virtual unsafe void Read(VoidPtr address)
        {

        }
        public void Unload()
        {
            OnUnloading();
            _isLoaded = false;
        }
        protected virtual void OnUnloading() { }
        public void Load()
        {
            _isLoading = true;
            switch (_format)
            {
                case FileFormat.Programatic:
                    OnLoading();
                    break;
                case FileFormat.ThirdParty:

                    break;
                case FileFormat.Binary:
                    ReadFromBinary();
                    break;
                case FileFormat.XML:
                    ReadFromXML();
                    break;
            }
            _isLoading = false;
            _isLoaded = true;
        }
        protected virtual void OnLoading() { }
        [Default]
        public string FilePath { get { return _filePath; } }
        public bool IsLoading { get { return _isLoading; } }
        [State]
        public bool IsLoaded { get { return _isLoaded; } }

        public delegate string TagRetriever();
        static Dictionary<string, Type> _tags = new Dictionary<string, Type>();
        static FileObject()
        {
            Delegate del;
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
                if (t.IsSubclassOf(typeof(FileObject)))
                    if ((del = Delegate.CreateDelegate(typeof(TagRetriever), t, "GetTag", false, false)) != null)
                        _tags.Add(((TagRetriever)del)(), t);
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

            if (_tags.ContainsKey(tag))
            {
                FileObject obj = Activator.CreateInstance(_tags[tag]) as FileObject;
                obj.Read(hdr->FileHeader);
                return obj;
            }
            throw new Exception();
        }
        public unsafe void ReadFromBinary()
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException();

            FileMap map = FileMap.FromFile(_filePath);
            FileCommonHeader* hdr = (FileCommonHeader*)map.Address;
            string tag = hdr->Tag;

            if (_tags.ContainsKey(tag))
            {
                Read(hdr->FileHeader);
                map.Dispose();
            }
            else
                throw new Exception();
        }
        public unsafe void ToBinary(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 8, FileOptions.SequentialScan))
            {
                int size = CalculateSize();
                stream.SetLength(size);
                FileMap map = FileMap.FromStream(stream);
                FileCommonHeader* hdr = (FileCommonHeader*)map.Address;
                Type t = GetType();
                hdr->Tag = FindTag(t);
                hdr->_length = size;
                Write(hdr->FileHeader);
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
            {
                XmlSerializer serializer = new XmlSerializer(t);
                FileObject obj = serializer.Deserialize(stream) as FileObject;
                obj._isLoaded = true;
                return obj;
            }
        }
        public void ReadFromXML()
        {

        }
        public void ToXML(string filePath)
        {
            _isWriting = true;
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }
            _isWriting = false;
        }

        public static Type FindType(string tag)
        {
            if (!_tags.ContainsKey(tag))
                throw new Exception("FileObject class does not exist for tag " + tag);
            return _tags[tag];
        }
        public static string FindTag(Type type)
        {
            if (!_tags.ContainsValue(type))
                throw new Exception("Tag does not exist for FileObject " + type.ToString());
            return _tags.FirstOrDefault(x => x.Value == type).Key;
        }
    }
    [Serializable]
    public class FileRef<T> : FileObject where T : FileObject
    {
        public static Dictionary<string, FileObject> LoadedFiles = new Dictionary<string, FileObject>();

        public FileRef(string tag, bool xml, string path, bool loadNow = false)
        {
            _tag = tag;
            _isXML = xml;
            _path = path;
            if (loadNow)
                Load();
        }

        [XmlAttribute("Tag")]
        private string _tag;
        [XmlAttribute("IsXML")]
        private bool _isXML;
        
        [XmlElement("ExternalPath")]
        private string _path;
        [XmlElement("File")]
        private T _file;

        public void SetFile(T value) { _file = value; }
        public T GetFile() { return _file ?? LoadFile(); }
        public T LoadFile()
        {
            if (LoadedFiles.ContainsKey(_path))
                return LoadedFiles[_path] as T;

            if (_isXML)
                return _file = FromXML(FindType(_tag), _path) as T;
            else
                return _file = FromBinary(FindType(_tag), _path) as T;
        }

        public static implicit operator T(FileRef<T> fileRef) { return fileRef.GetFile(); }

        internal static string GetTag() { return "FREF"; }
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
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DataReferenceOffset
    {
        buint _data;

        public bool IsInternal { get { return (_data & 0x80000000) != 0; } }
        private uint Value { get { return _data & 0x7FFFFFFF; } }
        public VoidPtr DataAddress
        {
            get { return Address + Value; }
            set { _data = (uint)(value - Address); }
        }
        public string FilePath { get { return new string((sbyte*)DataAddress); } }

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}
