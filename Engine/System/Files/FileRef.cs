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
        string FilePathAbsolute { get; }
    }
    public interface ISingleFileRef : IFileRef
    {
        void UnloadReference();
    }
    public interface IMultiFileRef : IFileRef
    {

    }
    /// <summary>
    /// Allows only one loaded instance of this file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleFileRef<T> : FileRef<T>, ISingleFileRef where T : FileObject
    {
        public override ResourceType ResourceType { get { return ResourceType.SingleFileRef; } }

        T _file;

        public SingleFileRef(Type type) : base(type) { }
        public SingleFileRef(string relativeFilePath) : base(relativeFilePath) { }
        public SingleFileRef(string relativeFilePath, Type type) : base(relativeFilePath, type) { }
        public SingleFileRef(T file, string relativeFilePath) : base(relativeFilePath)
        {
            if (file != null)
                file._filePath = relativeFilePath;
            _file = file;
        }
        public SingleFileRef(T file) : base(file._filePath)
        {
            _file = file;
        }
        public T File
        {
            get { return GetInstance(); }
            set { SetFile(value, false); }
        }
        public void SetFile(T value, bool exportToPath)
        {
            if (_file == value)
            {
                if (exportToPath)
                    ExportFile();
                return;
            }

            if (_file != null && _file._references.Contains(this))
                _file._references.Remove(this);

            _file = value;
            if (_file != null)
            {
                if (exportToPath)
                    ExportFile();
                Engine.AddLoadedFile(_relativeRefPath, _file);
                _file._references.Add(this);
            }
        }
        public void ExportFile()
        {
            string dir = Path.GetDirectoryName(RefPathAbsolute);
            _file.Export(dir, IsXML());
        }
        public override T GetInstance()
        {
            if (_file != null)
                return _file;

            if (string.IsNullOrEmpty(_relativeRefPath))
                _file = Activator.CreateInstance(_subType) as T;
            else
            {
                if (Engine.LoadedFiles.ContainsKey(_relativeRefPath))
                    _file = Engine.LoadedFiles[_relativeRefPath] as T;
                else
                {
                    string absolutePath = RefPathAbsolute;
                    if (!System.IO.File.Exists(absolutePath))
                        throw new FileNotFoundException();
                    if (IsXML())
                        _file = FromXML(_subType, absolutePath) as T;
                    else
                        _file = FromBinary(_subType, absolutePath) as T;
                    Engine.AddLoadedFile(_relativeRefPath, _file);
                }
            }

            _file._filePath = _relativeRefPath;
            _file._references.Add(this);

            return _file;
        }
        public void UnloadReference()
        {
            if (_file != null)
            {
                if (_file._references.Contains(this))
                    _file._references.Remove(this);
                if (_file._references.Count == 0)
                    _file.Unload();
                _file = null;
            }
        }
        private void FileUnloaded() { _file = null; }
        public void Write(XmlWriter writer, bool writeInternal)
        {
            base.Write(writer);
            if (writeInternal && File != null)
                _file.Write(writer);
            else
                writer.WriteAttributeString("path", _relativeRefPath);
            writer.WriteEndElement();
        }
        public override void Write(XmlWriter writer) { Write(writer, false); }
        public static implicit operator T(SingleFileRef<T> fileRef) { return fileRef == null ? null : fileRef.GetInstance(); }
        public static implicit operator SingleFileRef<T>(T file) { return new SingleFileRef<T>(file); }
        public static implicit operator SingleFileRef<T>(Type type) { return new SingleFileRef<T>(type); }
        public static implicit operator SingleFileRef<T>(string relativePath) { return new SingleFileRef<T>(relativePath); }
    }
    public class MultiFileRef<T> : FileRef<T>, IMultiFileRef where T : FileObject
    {
        public override ResourceType ResourceType { get { return ResourceType.MultiFileRef; } }
        
        public MultiFileRef(Type type) : base(type) { }
        public MultiFileRef(string relativeFilePath) : base(relativeFilePath) { }
        public MultiFileRef(string relativeFilePath, Type type) : base(relativeFilePath, type) { }
        public override T GetInstance()
        {
            if (string.IsNullOrEmpty(_relativeRefPath))
                return Activator.CreateInstance(_subType) as T;

            string absolutePath = RefPathAbsolute;
            if (!File.Exists(absolutePath))
                throw new FileNotFoundException();

            T file;
            if (IsXML())
                file = FromXML(_subType, absolutePath) as T;
            else
                file = FromBinary(_subType, absolutePath) as T;

            file._references.Add(this);
            return file;
        }
    }
    /// <summary>
    /// Indicates that this variable references a file that must be loaded.
    /// </summary>
    public abstract class FileRef<T> : FileObject where T : FileObject
    {
        public FileRef()
        {
            _relativeRefPath = null;
            _subType = typeof(T);
        }
        public FileRef(Type type)
        {
            _relativeRefPath = null;
            if (type.IsSubclassOf(typeof(T)))
                _subType = type;
            else
                throw new Exception(type.ToString() + " does not inherit " + typeof(T).ToString());
        }
        public FileRef(string relativePath)
        {
            _relativeRefPath = relativePath;
            _subType = typeof(T);
        }
        public FileRef(string relativePath, Type type)
        {
            _relativeRefPath = relativePath;
            if (type.IsSubclassOf(typeof(T)))
                _subType = type;
            else
                throw new Exception(type.ToString() + " does not inherit " + typeof(T).ToString());
        }

        protected Type _subType = null;
        protected string _relativeRefPath;

        public string RefPathAbsolute { get { return Engine.StartupPath + _relativeRefPath; } }
        public string RefPathRelative { get { return _relativeRefPath; } }
        public string Extension() { return Path.GetExtension(_relativeRefPath).ToLower().Substring(1); }
        public bool IsXML()
        {
            string ext = Extension();
            return ext == "xml" || ext.StartsWith("x");
        }
        public abstract T GetInstance();
        public override int CalculateSize(StringTable table)
        {
            table.Add(_relativeRefPath);
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
        public override void Read(XmlReader reader)
        {
            throw new NotImplementedException();
        }
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
