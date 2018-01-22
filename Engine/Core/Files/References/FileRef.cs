using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine.Core.Reflection.Attributes;

namespace TheraEngine.Files
{
    public interface IFileRef : IFileLoader
    {
        FileObject File { get; }
        bool IsLoaded { get; set; }
        bool StoredInternally { get; }
        void UnloadReference();
    }
    /// <summary>
    /// Indicates that this variable references a file that must be loaded.
    /// </summary>
    [FileExt("ref")]
    public abstract class FileRef<T> : FileLoader<T>, IFileRef where T : FileObject
    {
        #region Constructors
        public FileRef() : base() { }
        public FileRef(Type type) : base(type) { }
        public FileRef(string filePath) : base(filePath) { }
        public FileRef(string filePath, Type type) : base(filePath, type) { }
        public FileRef(string filePath, T file, bool exportNow) : this(filePath)
        {
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
        public FileRef(string filePath, Func<T> createIfNotFound) : this(filePath)
        {
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
        public FileRef(string dir, string name, ProprietaryFileFormat format) : base(dir, name, format) { }
        public FileRef(string dir, string name, ProprietaryFileFormat format, T file, bool exportNow) : this(dir, name, format)
        {
            if (file != null)
                file.FilePath = ReferencePath;
            File = file;
            if (exportNow && File != null)
                ExportReference();
        }
        public FileRef(string dir, string name, ProprietaryFileFormat format, Func<T> createIfNotFound) : this(dir, name, format)
        {
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
        public FileRef(T file) : this(file.FilePath)
        {
            File = file;
        }
        #endregion

        [TSerialize("File", Condition = "StoredInternally")]
        protected T _file;
        
        [Category("File Reference")]
        public bool IsLoaded
        {
            get => _file != null;
            set
            {
                if (value)
                    GetInstance();
                else
                    UnloadReference();
            }
        }

        /// <summary>
        /// If true, the referenced file will be written within the parent file's data
        /// and loaded with the parent file instead of being loaded on demand from the external file.
        /// </summary>
        [Browsable(false)]
        [Category("File Reference")]
        public bool StoredInternally => string.IsNullOrWhiteSpace(ReferencePath);

        //[Category("File Reference")]
        //[TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        //public override string ReferencePath
        //{
        //    get => base.ReferencePath;
        //    set => base.ReferencePath = value;
        //}

        FileObject IFileRef.File => File;
        
        [Category("File Reference")]
        [BrowsableIf("_file != null")]
        public T File
        {
            get => GetInstance();
            set
            {
                if (_file == value)
                    return;

                if (_file != null)
                {
                    if (_file.References.Contains(this))
                        _file.References.Remove(this);
                    Unloaded?.Invoke(_file);
                }

                _file = value;
                if (_file != null)
                {
                    string path = _file.FilePath;
                    if (!string.IsNullOrEmpty(path))
                    {
                        ReferencePath = path;
                        if (!RegisterFile(path, _file))
                            OnLoaded();
                    }
                    else
                        OnLoaded();

                    if (!_file.References.Contains(this))
                        _file.References.Add(this);
                }
            }
        }

        protected abstract bool RegisterFile(string path, T file);

        public event Action<T> Unloaded;
        /// <summary>
        /// Method to be called when the file this reference points to becomes (or currently is) available.
        /// </summary>
        public override void RegisterLoadEvent(Action<T> onLoaded)
        {
            base.RegisterLoadEvent(onLoaded);
            if (_file != null)
                onLoaded?.Invoke(_file);
        }
        public void RegisterUnloadEvent(Action<T> unloaded)
        {
            if (unloaded != null)
                Unloaded += unloaded;
        }
        public void UnregisterUnloadEvent(Action<T> unloaded)
        {
            if (unloaded != null)
                Unloaded -= unloaded;
        }

        public void ExportReference() => _file?.Export();
        public void ExportReference(string dir, string name, FileFormat format, string thirdPartyExt = null, bool setPath = true)
        {
            if (_file == null)
                return;
            _file.Export(dir, name, format, thirdPartyExt);
            if (setPath)
                ReferencePath = _file.FilePath;
        }

        /// <summary>
        /// Loads or retrieves the instance of this file.
        /// </summary>
        public abstract T GetInstance();

        /// <summary>
        /// Unloads this reference to the file. 
        /// The file may still be allocated if referenced elsewhere.
        /// Deallocates the file if no other references exist.
        /// </summary>
        public void UnloadReference()
        {
            if (_file != null)
            {
                _file.References.Remove(this);
                if (_file.References.Count == 0)
                    _file.Unload();
                _file = null;
            }
        }

        protected override void OnFileLoaded(T file)
        {
            file.References.Add(this);
        }

        public static implicit operator T(FileRef<T> fileRef) => fileRef?.GetInstance();
    }
}
