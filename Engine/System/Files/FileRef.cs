using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CustomEngine.Files
{
    public interface IFileRef
    {
        string FilePathRelative { get; }
        FileObject File { get; set; }
    }
    /// <summary>
    /// Indicates that this variable references a file that must be loaded.
    /// </summary>
    public class FileRef<T> : FileObject, IFileRef where T : FileObject
    {
        public FileRef()
        {
            _relativePath = null;
            _file = null;
            _subType = typeof(T);
        }
        public FileRef(T file)
        {
            _relativePath = null;
            _file = file;
            _subType = typeof(T);
        }
        public FileRef(Type type)
        {
            _relativePath = null;
            _file = null;
            if (type.IsSubclassOf(typeof(T)))
                _subType = type;
            else
                throw new Exception(type.ToString() + " does not inherit " + typeof(T).ToString());
        }
        public FileRef(string relativePath, bool loadNow = false)
        {
            _relativePath = relativePath;
            _subType = typeof(T);
            if (loadNow)
                LoadFile();
        }
        public FileRef(string relativePath, Type type, bool loadNow = false)
        {
            _relativePath = relativePath;
            if (type.IsSubclassOf(typeof(T)))
                _subType = type;
            else
                throw new Exception(type.ToString() + " does not inherit " + typeof(T).ToString());
            if (loadNow)
                LoadFile();
        }

        private Type _subType = null;
        private string _relativePath;
        private T _file;
        
        public string FilePathAbsolute { get { return Engine.StartupPath + _relativePath; } }
        public string FilePathRelative { get { return _relativePath; } }
        public FileObject File
        {
            get { return LoadFile(); }
            set { SetFile(value as T, false); }
        }
        public string Extension() { return Path.GetExtension(_relativePath).ToLower().Substring(1); }
        public bool IsXML()
        {
            string ext = Extension();
            return ext == "xml" || ext.StartsWith("x");
        }
        public void SetFile(T value, bool exportToPath)
        {
            _file = value;
            if (_file != null)
            {
                if (exportToPath)
                {
                    if (IsXML())
                        _file.ToXML(FilePathAbsolute);
                    else
                        _file.ToBinary(FilePathAbsolute);
                }
                if (Engine.LoadedFiles.ContainsKey(_relativePath))
                    Engine.LoadedFiles[_relativePath] = _file;
                else
                    Engine.LoadedFiles.Add(_relativePath, _file);
                _file.Unloaded += FileUnloaded;
            }
        }
        public T LoadFile()
        {
            if (_file != null)
                return _file;

            if (string.IsNullOrEmpty(_relativePath))
                _file = Activator.CreateInstance(_subType) as T;
            else
            {
                string absolutePath = FilePathAbsolute;
                if (!System.IO.File.Exists(absolutePath))
                    throw new FileNotFoundException();

                if (Engine.LoadedFiles.ContainsKey(_relativePath))
                    _file = Engine.LoadedFiles[_relativePath] as T;
                else
                {
                    if (IsXML())
                        _file = FromXML(_subType, absolutePath) as T;
                    else
                        _file = FromBinary(_subType, absolutePath) as T;
                    Engine.LoadedFiles.Add(_relativePath, _file);
                }
            }

            _file._filePath = _relativePath;
            _file.Unloaded += FileUnloaded;

            return _file;
        }
        private void FileUnloaded() { _file = null; }
        public override int CalculateSize(StringTable table)
        {
            table.Add(_relativePath);
            return FileRefHeader.Size;
        }
        public override void Write(VoidPtr address)
        {
            throw new NotImplementedException();
        }
        public override void Read(VoidPtr address)
        {
            throw new NotImplementedException();
        }
        public void Write(XmlWriter writer, bool writeInternal)
        {
            base.Write(writer);
            if (writeInternal && File != null)
                _file.Write(writer);
            else
                writer.WriteAttributeString("path", _relativePath);
            writer.WriteEndElement();
        }
        public override void Write(XmlWriter writer) { Write(writer, false); }
        public override void Read(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public static implicit operator T(FileRef<T> fileRef) { return fileRef == null ? null : fileRef.LoadFile(); }
        public static implicit operator FileRef<T>(T file) { return new FileRef<T>(file); }
        public static implicit operator FileRef<T>(Type type) { return new FileRef<T>(type); }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FileRefHeader
    {
        public const int Size = 4;

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
