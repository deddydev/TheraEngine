using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace TheraEngine.Files
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
    [FileClass("SREF", "Single File Reference")]
    public class SingleFileRef<T> : FileRef<T>, ISingleFileRef where T : FileObject
    {
        T _file;
        
        public SingleFileRef() : base(typeof(T)) { }
        public SingleFileRef(Type type) : base(type) { }
        public SingleFileRef(string filePath) : base(filePath) { }
        public SingleFileRef(string filePath, Type type) : base(filePath, type) { }
        public SingleFileRef(T file, string filePath) : base(filePath)
        {
            if (file != null)
                file.FilePath = filePath;
            File = file;
        }
        public SingleFileRef(T file, string dir, string name, FileFormat format, bool exportNow) : base(GetFilePath(dir, name, format, typeof(T)))
        {
            if (file != null)
                file.FilePath = ReferencePath;
            File = file;
            if (exportNow && File != null)
                ExportReference();
        }
        public SingleFileRef(T file) : base(file.FilePath)
        {
            File = file;
        }
        public T File
        {
            get => GetInstance();
            set
            {
                if (_file == value)
                    return;

                if (_file != null && _file.References.Contains(this))
                    _file.References.Remove(this);

                _file = value;
                if (_file != null)
                {
                    if (!string.IsNullOrEmpty(_file.FilePath))
                        ReferencePath = _file.FilePath;

                    Engine.AddLoadedFile(_refPath, _file);
                    _file.References.Add(this);
                }
            }
        }
        public void ExportReference()
        {
            _file.Export();
        }
        public void ExportReference(string dir, string name, FileFormat format, bool setPath = true)
        {
            //if (_file == null || string.IsNullOrEmpty(_refPath))
            //    return;
            //string dir = Path.GetDirectoryName(RefPathAbsolute);
            //string name = Path.GetFileNameWithoutExtension(RefPathAbsolute);
            _file.Export(dir, name, format);
            if (setPath)
                ReferencePath = _file.FilePath;
        }
        /// <summary>
        /// Loads or retrieves the previously loaded instance of this file.
        /// </summary>
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

            if (_file != null)
            {
                _file.FilePath = ReferencePath;
                _file.References.Add(this);
            }

            return _file;
        }
        private void GetFile()
        {
            string absolutePath = ReferencePath;
            bool fileExists = System.IO.File.Exists(absolutePath);
            if (!fileExists)
            {
                //File = Activator.CreateInstance(_subType) as T;
                Engine.DebugPrint(string.Format("Could not load file at \"{0}\".", absolutePath));
            }
            else
            {
                try
                {
                    if (IsSpecial())
                        File = Activator.CreateInstance(_subType, absolutePath) as T;
                    else
                        switch (GetFormat())
                        {
                            case FileFormat.XML:
                                File = FromXML(_subType, absolutePath) as T;
                                break;
                            case FileFormat.Binary:
                                File = FromBinary(_subType, absolutePath) as T;
                                break;
                        }
                }
                catch (Exception e)
                {
                    Engine.DebugPrint(string.Format("Could not load file at \"{0}\".\nException:\n\n{1}", absolutePath, e.ToString()));
                }
            }
        }

        private bool IsSpecial()
        {
            FileClass header = GetFileHeader(_subType);
            return header == null ? false : header.IsSpecialDeserialize;
        }
        
        public void UnloadReference()
        {
            if (_file != null)
            {
                if (_file.References.Contains(this))
                    _file.References.Remove(this);
                if (_file.References.Count == 0)
                    _file.Unload();
                _file = null;
            }
        }
        private void FileUnloaded() { _file = null; }

        public static implicit operator T(SingleFileRef<T> fileRef) { return fileRef?.GetInstance(); }
        public static implicit operator SingleFileRef<T>(T file) { return file == null ? null : new SingleFileRef<T>(file); }
        public static implicit operator SingleFileRef<T>(Type type) { return new SingleFileRef<T>(type); }
        public static implicit operator SingleFileRef<T>(string relativePath) { return new SingleFileRef<T>(relativePath); }
    }
    [FileClass("MREF", "Multi File Reference")]
    public class MultiFileRef<T> : FileRef<T>, IMultiFileRef where T : FileObject
    {
        public MultiFileRef(Type type) : base(type) { }
        public MultiFileRef(string filePath) : base(filePath) { }
        public MultiFileRef(string filePath, Type type) : base(filePath, type) { }
        public MultiFileRef(T file) : base(file.FilePath)
        {
            //SetFile(file, !string.IsNullOrEmpty(file._filePath));
        }
        
        public override T GetInstance()
        {
            if (string.IsNullOrEmpty(_refPath))
                return Activator.CreateInstance(_subType) as T;

            string absolutePath = ReferencePath;
            if (!File.Exists(absolutePath))
                throw new FileNotFoundException();

            T file;
            //if (IsSpecial())
            //    file = Activator.CreateInstance(_subType, absolutePath) as T;
            //else
            if (GetFormat() == FileFormat.XML)
                file = FromXML(_subType, absolutePath) as T;
            else
                file = FromBinary(_subType, absolutePath) as T;

            file.References.Add(this);
            return file;
        }

        public static implicit operator T(MultiFileRef<T> fileRef) { return fileRef?.GetInstance(); }
        public static implicit operator MultiFileRef<T>(T file) { return new MultiFileRef<T>(file); }
        public static implicit operator MultiFileRef<T>(Type type) { return new MultiFileRef<T>(type); }
        public static implicit operator MultiFileRef<T>(string relativePath) { return new MultiFileRef<T>(relativePath); }
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
            ReferencePath = filePath;
        }
        public FileRef(string filePath, Type type)
        {
            if (type.IsSubclassOf(typeof(T)))
                _subType = type;
            else
                throw new Exception(type.ToString() + " does not inherit " + typeof(T).ToString());
            //if (Path.HasExtension(filePath) && FileManager.GetTypeWithExtension(Path.GetExtension(filePath)) != _subType)
            //    throw new InvalidOperationException("Extension does not match type");
            ReferencePath = filePath;
        }

        [Serialize("Path", IsXmlAttribute = true)]
        protected string _refPath = null;
        private string _absolutePath = null;
        protected Type _subType = null;
        private bool _storeInternally = false;

        protected bool StoreInternally
        {
            get => _storeInternally;
            set => _storeInternally = value;
        }
        public string ReferencePath
        {
            get
            {
                if (string.IsNullOrEmpty(_absolutePath) && Engine.Settings != null && !string.IsNullOrEmpty(Engine.Game.FilePath) && !string.IsNullOrEmpty(_refPath))
                    _absolutePath = Engine.Game.FilePath + _refPath;
                return _absolutePath;
            }
            set
            {
                _refPath = value;
                _absolutePath = _refPath;
                if (_refPath != null)
                {
                    if (_refPath.StartsWith("\\"))
                        _refPath.Substring(1);
                    if (!_refPath.Contains("\\"))
                        _absolutePath = Engine.StartupPath + _refPath;
                }
            }
        }

        public string Extension()
        {
            if (_refPath == null)
                return null;
            return Path.GetExtension(_refPath).ToLower().Substring(1);
        }
        public FileFormat GetFormat()
        {
            string ext = Extension();
            if (string.IsNullOrEmpty(ext))
                return FileFormat.Binary;
            if (ext == "xml" || ext.StartsWith("x"))
                return FileFormat.XML;
            return FileFormat.Binary;
        }
        //public bool IsSpecial()
        //{
        //    string ext = Extension();
        //    return FileManager.IsSpecial(ext);
        //}
        public abstract T GetInstance();
    }
}
