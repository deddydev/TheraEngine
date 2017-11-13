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
        Type ReferencedType { get; }
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
        private event Action Loaded;

        [TSerialize("File", Condition = "StoreInternally")]
        private T _file;

        [TSerialize(Condition = "!StoreInternally", XmlNodeType = EXmlNodeType.Attribute)]
        public override string ReferencePath
        {
            get => base.ReferencePath;
            set
            {
                base.ReferencePath = value;
                StoredInternally = string.IsNullOrWhiteSpace(base.ReferencePath);
            }
        }
        public bool IsLoaded => _file != null;

        /// <summary>
        /// If true, the referenced file will be written within the parent file's data
        /// and loaded with the parent file instead of being loaded on demand from the external file.
        /// </summary>
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool StoredInternally { get; private set; } = true;

        public SingleFileRef() : base(typeof(T)) { }
        public SingleFileRef(Type type) : base(type) { }

        public void RegisterLoadEvent(Action onLoaded)
        {
            if (onLoaded == null)
                return;
            
            Loaded += onLoaded;

            if (_file != null)
                onLoaded();
        }
        public void UnregisterLoadEvent(Action onLoaded)
        {
            if (onLoaded == null)
                return;
            Loaded -= onLoaded;
        }

        public SingleFileRef(string filePath) : base(filePath) { StoredInternally = false; }
        public SingleFileRef(string filePath, Type type) : base(filePath, type) { StoredInternally = false; }
        public SingleFileRef(string filePath, T file, bool exportNow) : base(filePath)
        {
            StoredInternally = false;
            if (file != null)
                file.FilePath = ReferencePath;
            File = file;
            if (exportNow && File != null)
                ExportReference();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="createIfNotFound"></param>
        public SingleFileRef(string filePath, Func<T> createIfNotFound) : base(filePath)
        {
            StoredInternally = false;
            if (!System.IO.File.Exists(ReferencePath) || DetermineType(ReferencePath) != typeof(T))
            {
                T file = createIfNotFound?.Invoke();
                if (file != null)
                    file.FilePath = ReferencePath;
                File = file;
                if (File != null)
                    ExportReference();
            }
        }
        public SingleFileRef(string dir, string name, ProprietaryFileFormat format) : base(GetFilePath(dir, name, format, typeof(T))) { }
        public SingleFileRef(string dir, string name, ProprietaryFileFormat format, T file, bool exportNow) : this(dir, name, format)
        {
            StoredInternally = false;
            if (file != null)
                file.FilePath = ReferencePath;
            File = file;
            if (exportNow && File != null)
                ExportReference();
        }
        public SingleFileRef(string dir, string name, ProprietaryFileFormat format, Func<T> createIfNotFound) : this(dir, name, format)
        {
            StoredInternally = false;
            if (!System.IO.File.Exists(ReferencePath) || DetermineType(ReferencePath) != typeof(T))
            {
                T file = createIfNotFound?.Invoke();
                if (file != null)
                    file.FilePath = ReferencePath;
                File = file;
                if (File != null)
                    ExportReference();
            }
        }
        public SingleFileRef(T file) : base(file.FilePath)
        {
            File = file;
        }

        [BrowsableIf("_file != null")]
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
                    string path = _file.FilePath;

                    if (!string.IsNullOrEmpty(path))
                    {
                        ReferencePath = path;

                        if (!(Engine.LoadedFiles.ContainsKey(path) && Engine.LoadedFiles[path].Contains(_file)))
                            Engine.AddLoadedFile(path, _file);
                    }

                    if (!_file.References.Contains(this))
                        _file.References.Add(this);
                }
            }
        }
        
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
        /// Loads or retrieves the single instance of this file.
        /// </summary>
        public T GetInstance()
        {
            if (_file != null)
                return _file;

            string absolutePath = ReferencePath;
            if (absolutePath != null && Engine.LoadedFiles.ContainsKey(absolutePath))
            {
                List<FileObject> files = Engine.LoadedFiles[absolutePath];
                if (files.Count > 0)
                    return _file = files[0] as T;
            }
            
            _file = LoadNewInstance();
            Loaded?.Invoke();
            
            return _file;
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

        public static implicit operator T(SingleFileRef<T> fileRef) => fileRef?.GetInstance();
        public static implicit operator SingleFileRef<T>(T file) => file == null ? null : new SingleFileRef<T>(file);
        public static implicit operator SingleFileRef<T>(Type type) => new SingleFileRef<T>(type);
        public static implicit operator SingleFileRef<T>(string relativePath) => new SingleFileRef<T>(relativePath);
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
                throw new Exception(type.ToString() + " is not assignable to " + typeof(T).ToString());
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
            if (typeof(T).IsAssignableFrom(type))
                _subType = type;
            else
                throw new Exception(type.ToString() + " is not assignable to " + typeof(T).ToString());
            //if (Path.HasExtension(filePath) && FileManager.GetTypeWithExtension(Path.GetExtension(filePath)) != _subType)
            //    throw new InvalidOperationException("Extension does not match type");
            ReferencePath = filePath;
        }

        protected string _refPath = null;
        private string _absolutePath = null;
        protected Type _subType = null;

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
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

        [Browsable(false)]
        public Type ReferencedType => typeof(T);

        protected virtual T LoadNewInstance()
        {
            string absolutePath = ReferencePath;

            if (string.IsNullOrWhiteSpace(absolutePath))
            {
                T file = CreateNewInstance();
                if (file != null)
                {
                    file.FilePath = absolutePath;
                    file.References.Add(this);
                    file.OnLoaded();
                }
                return file;
            }

            if (!File.Exists(absolutePath))
            {
                Engine.LogWarning(string.Format("Could not load file at \"{0}\".", absolutePath));
                return null;
            }

            try
            {
                if (IsSpecial())
                {
                    T file = Activator.CreateInstance(_subType, absolutePath) as T;
                    file.FilePath = absolutePath;
                    file.References.Add(this);
                    file.OnLoaded();
                    return file;
                }
                else
                    switch (GetFormat())
                    {
                        case FileFormat.XML:
                            T xmlFile = FromXML(_subType, absolutePath) as T;
                            xmlFile?.References.Add(this);
                            return xmlFile;
                        case FileFormat.Binary:
                            T binFile = FromBinary(_subType, absolutePath) as T;
                            binFile?.References.Add(this);
                            return binFile;
                        default:
                            Engine.LogWarning(string.Format("Could not load file at \"{0}\". Invalid file format.", absolutePath));
                            break;
                    }
            }
            catch (Exception e)
            {
                Engine.LogWarning(string.Format("Could not load file at \"{0}\".\nException:\n\n{1}", absolutePath, e.ToString()));
            }
            
            return null;
        }

        public string Extension()
        {
            if (_refPath == null)
                return null;
            return Path.GetExtension(_refPath).ToLowerInvariant().Substring(1);
        }
        public FileFormat GetFormat()
        {
            string ext = Extension();
            if (!string.IsNullOrEmpty(ext))
            {
                if (ext.StartsWith("x"))
                    return FileFormat.XML;
            }
            return FileFormat.Binary;
        }

        public void LoadNewInstanceAsync(Action<T> onLoaded, TaskCreationOptions options = TaskCreationOptions.PreferFairness)
            => LoadNewInstanceAsync(options).ContinueWith(task => onLoaded(task.Result));
        public void LoadNewInstanceAsync(Action<T> onLoaded) 
            => LoadNewInstanceAsync().ContinueWith(task => onLoaded(task.Result));
        public async Task<T> LoadNewInstanceAsync() => await Task.Run(() => LoadNewInstance());
        public async Task<T> LoadNewInstanceAsync(TaskCreationOptions options) => await Task.Factory.StartNew(() => LoadNewInstance(), options);

        public T CreateNewInstance()
        {
            if (_subType.IsAbstract)
            {
                Engine.LogWarning("Can't automatically instantiate an abstract class: " + _subType.GetFriendlyName());
            }
            else if (_subType.IsInterface)
            {
                Engine.LogWarning("Can't automatically instantiate an interface: " + _subType.GetFriendlyName());
            }
            else if (_subType.GetConstructor(new Type[0]) == null)
            {
                Engine.LogWarning("Can't automatically instantiate a class with no parameterless constructor: " + _subType.GetFriendlyName());
            }
            else
            {
                return Activator.CreateInstance(_subType) as T;
            }
            return null;
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
        public override string ToString() => ReferencePath;
    }
}
