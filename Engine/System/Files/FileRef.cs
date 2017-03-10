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
        string FilePath { get; }
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
        public SingleFileRef(string filePath) : base(filePath) { }
        public SingleFileRef(string filePath, Type type) : base(filePath, type) { }
        public SingleFileRef(T file, string filePath) : base(filePath)
        {
            if (file != null)
                file._filePath = filePath;
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
                Engine.AddLoadedFile(_refPath, _file);
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

            if (string.IsNullOrEmpty(_refPath))
                _file = Activator.CreateInstance(_subType) as T;
            else
            {
                if (Engine.LoadedFiles.ContainsKey(_refPath))
                {
                    List<FileObject> files = Engine.LoadedFiles[_refPath];
                    if (files.Count > 0)
                        _file = files[0] as T;
                    else
                        GetFile();
                }
                else
                    GetFile();
            }

            _file._filePath = RefPathAbsolute;
            _file._references.Add(this);

            return _file;
        }
        private void GetFile()
        {
            string absolutePath = RefPathAbsolute;
            //if (!System.IO.File.Exists(absolutePath))
            //    throw new FileNotFoundException();
            if (!System.IO.File.Exists(absolutePath) || IsSpecial())
                _file = Activator.CreateInstance(_subType, absolutePath) as T;
            else if (IsXML())
                _file = FromXML(_subType, absolutePath) as T;
            else
                _file = FromBinary(_subType, absolutePath) as T;
            Engine.AddLoadedFile(_refPath, _file);
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
        public override void Write(XmlWriter writer) { Write(writer, false); }
        public void Write(XmlWriter writer, bool writeInternal)
        {
            base.Write(writer);
            if (writeInternal && File != null)
                _file.Write(writer);
            else
                writer.WriteAttributeString("path", _refPath);
            writer.WriteEndElement();
        }
        public override void Write(VoidPtr address, StringTable table)
        {
            base.Write(address, table);
        }
        public override void Read(VoidPtr address, VoidPtr strings)
        {
            base.Read(address, strings);
        }
        public override void Read(XMLReader reader)
        {
            base.Read(reader);
        }

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }

        public static implicit operator T(SingleFileRef<T> fileRef) { return fileRef?.GetInstance(); }
        public static implicit operator SingleFileRef<T>(T file) { return new SingleFileRef<T>(file); }
        public static implicit operator SingleFileRef<T>(Type type) { return new SingleFileRef<T>(type); }
        public static implicit operator SingleFileRef<T>(string relativePath) { return new SingleFileRef<T>(relativePath); }
    }
    public class MultiFileRef<T> : FileRef<T>, IMultiFileRef where T : FileObject
    {
        public override ResourceType ResourceType { get { return ResourceType.MultiFileRef; } }
        
        public MultiFileRef(Type type) : base(type) { }
        public MultiFileRef(string filePath) : base(filePath) { }
        public MultiFileRef(string filePath, Type type) : base(filePath, type) { }
        public override T GetInstance()
        {
            if (string.IsNullOrEmpty(_refPath))
                return Activator.CreateInstance(_subType) as T;

            string absolutePath = RefPathAbsolute;
            if (!File.Exists(absolutePath))
                throw new FileNotFoundException();

            T file;
            if (IsSpecial())
                file = Activator.CreateInstance(_subType, absolutePath) as T;
            else if (IsXML())
                file = FromXML(_subType, absolutePath) as T;
            else
                file = FromBinary(_subType, absolutePath) as T;

            file._references.Add(this);
            return file;
        }

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Indicates that this variable references a file that must be loaded.
    /// </summary>
    public abstract class FileRef<T> : FileObject where T : FileObject
    {
        public FileRef()
        {
            _subType = typeof(T);
        }
        public FileRef(Type type)
        {
            if (type.IsSubclassOf(typeof(T)))
                _subType = type;
            else
                throw new Exception(type.ToString() + " does not inherit " + typeof(T).ToString());
        }
        public FileRef(string filePath)
        {
            _subType = typeof(T);
            //if (Path.HasExtension(filePath) && FileManager.GetTypeWithExtension(Path.GetExtension(filePath)) != _subType)
            //    throw new InvalidOperationException("Extension does not match type");
            RefPathAbsolute = filePath;
        }
        public FileRef(string filePath, Type type)
        {
            if (type.IsSubclassOf(typeof(T)))
                _subType = type;
            else
                throw new Exception(type.ToString() + " does not inherit " + typeof(T).ToString());
            //if (Path.HasExtension(filePath) && FileManager.GetTypeWithExtension(Path.GetExtension(filePath)) != _subType)
            //    throw new InvalidOperationException("Extension does not match type");
            RefPathAbsolute = filePath;
        }

        protected Type _subType = null;
        protected string _refPath = null;
        private string _absolutePath = null;

        public string RefPathAbsolute
        {
            get { return _absolutePath; }
            set
            {
                _refPath = value;
                if (_refPath.StartsWith("\\"))
                    _absolutePath = Engine.StartupPath + _refPath;
                else
                    _absolutePath = _refPath;
            }
        }
        public string Extension() { return Path.GetExtension(_refPath).ToLower().Substring(1); }
        public bool IsXML()
        {
            string ext = Extension();
            return ext == "xml" || ext.StartsWith("x");
        }
        public bool IsSpecial()
        {
            string ext = Extension();
            return FileManager.IsSpecial(ext);
        }
        public abstract T GetInstance();
        protected override int OnCalculateSize(StringTable table)
        {
            table.Add(RefPathAbsolute);
            return FileRefHeader.Size;
        }
        public override void Write(XmlWriter writer)
        {

        }
        public override void Write(VoidPtr address, StringTable table)
        {

        }
        public override void Read(VoidPtr address, VoidPtr strings)
        {

        }
        public override void Read(XMLReader reader)
        {

        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FileRefHeader
    {
        public const int Size = 4;

        Bin32 _data;

        public bool IsInternal
        {
            get => _data[31];
            set => _data[31] = value;
        }
        public bool AllowMultiLoad
        {
            get => _data[30];
            set => _data[30] = value;
        }
        private uint Value
        {
            get => _data[0, 30];
            set => _data[0, 30] = value;
        }

        /// <summary>
        /// String address if external, actual data address if internal
        /// </summary>
        public VoidPtr DataAddress
        {
            get => Address + Value;
            set => Value = (uint)(value - Address);
        }

        public string FilePath => DataAddress.GetString();

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}
