﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

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
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SingleFileRef<T> : FileRef<T>, ISingleFileRef where T : FileObject
    {
        T _file;
        
        public SingleFileRef() : base(typeof(T)) { }
        public SingleFileRef(Type type) : base(type) { }
        public SingleFileRef(string filePath) : base(filePath) { }
        public SingleFileRef(string filePath, Type type) : base(filePath, type) { }
        public SingleFileRef(string filePath, T file, bool exportNow) : base(filePath)
        {
            if (file != null)
                file.FilePath = ReferencePath;
            File = file;
            if (exportNow && File != null)
                ExportReference();
        }
        public SingleFileRef(string dir, string name, FileFormat format) : base(GetFilePath(dir, name, format, typeof(T))) { }
        public SingleFileRef(string dir, string name, FileFormat format, T file, bool exportNow) : this(dir, name, format)
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

        public bool IsLoaded => _file != null;
        public void ExportReference() => _file?.Export();
        public void ExportReference(string dir, string name, FileFormat format, bool setPath = true)
        {
            if (_file == null)
                return;
            _file.Export(dir, name, format);
            if (setPath)
                ReferencePath = _file.FilePath;
        }
        /// <summary>
        /// Loads or retrieves the singular instance of this file.
        /// </summary>
        public override T GetInstance()
        {
            if (_file != null)
                return _file;
            else
                File = LoadNewInstance();
            return _file;
        }
        /// <summary>
        /// Loads a new completely new and unique instance of this file.
        /// </summary>
        public T LoadNewInstance() => base.GetInstance();

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
    /// <summary>
    /// Indicates that this variable references a file that must be loaded.
    /// </summary>
    [FileClass("REF", "File Reference")]
    public class FileRef<T> : FileObject, IFileRef where T : FileObject
    {
        public FileRef()
        {
            _subType = typeof(T);
        }
        public FileRef(Type type)
        {
            if (typeof(T).IsAssignableFrom(type))
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

        protected string _refPath = null;
        private string _absolutePath = null;
        protected Type _subType = null;
        private bool _storeInternally = false;

        protected bool StoreInternally
        {
            get => _storeInternally;
            set => _storeInternally = value;
        }
        [Serialize(IsXmlAttribute = true)]
        public virtual string ReferencePath
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
        private T GetFile()
        {
            string absolutePath = ReferencePath;
            bool fileExists = System.IO.File.Exists(absolutePath);
            if (!fileExists)
            {
                //File = Activator.CreateInstance(_subType) as T;
                Engine.PrintLine(string.Format("Could not load file at \"{0}\".", absolutePath));
            }
            else
            {
                try
                {
                    if (IsSpecial())
                        return Activator.CreateInstance(_subType, absolutePath) as T;
                    else
                        switch (GetFormat())
                        {
                            case FileFormat.XML:
                                return FromXML(_subType, absolutePath) as T;
                            case FileFormat.Binary:
                                return FromBinary(_subType, absolutePath) as T;
                            default:
                                Engine.PrintLine(string.Format("Could not load file at \"{0}\". Invalid file format.", absolutePath));
                                break;
                        }
                }
                catch (Exception e)
                {
                    Engine.PrintLine(string.Format("Could not load file at \"{0}\".\nException:\n\n{1}", absolutePath, e.ToString()));
                }
            }
            return null;
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
        public void GetInstanceAsync() => Task.Run(() => GetInstance());
        public virtual T GetInstance()
        {
            T file = null;
            if (string.IsNullOrEmpty(_refPath))
                file = Activator.CreateInstance(_subType) as T;
            else
            {
                if (Engine.LoadedFiles.ContainsKey(_refPath))
                {
                    List<FileObject> files = Engine.LoadedFiles[_refPath];
                    if (files.Count > 0)
                        file = files[0] as T;
                    else
                        file = GetFile();
                }
                else
                    file = GetFile();
            }

            if (file != null)
            {
                file.FilePath = ReferencePath;
                file.References.Add(this);
            }

            return file;
        }
        /// <summary>
        /// Returns true if the file needs special deserialization handling.
        /// If so, the class needs a constructor that takes the file's absolute path (string) as its only argument.
        /// </summary>
        private bool IsSpecial()
        {
            FileClass header = GetFileHeader(_subType);
            return header == null ? false : header.IsSpecialDeserialize;
        }
        public override string ToString()
        {
            return ReferencePath;
        }
    }
}
