using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TheraEngine.Core.Reflection.Attributes;

namespace TheraEngine.Core.Files
{
    public interface IFileRef : IFileLoader
    {
        IFileObject File { get; }
        bool IsLoaded { get; set; }
        bool StoredInternally { get; }
        void UnloadReference();
        void ExportReference(ESerializeFlags flags = ESerializeFlags.Default);
        void ExportReference(string dir, string name, EFileFormat format, string thirdPartyExt = null, bool setPath = true, ESerializeFlags flags = ESerializeFlags.Default);
    }
    /// <summary>
    /// Indicates that this variable references a file that must be loaded.
    /// </summary>
    [FileExt("ref")]
    public abstract class FileRef<T> : FileLoader<T>, IFileRef where T : class, IFileObject
    {
        #region Constructors
        public FileRef() : base() { }
        public FileRef(Type type) : base(type) { }
        public FileRef(string filePath) : base(filePath) { }
        public FileRef(string filePath, Type type) : base(filePath, type) { }
        public FileRef(string filePath, T file, bool exportNow) : this(filePath)
        {
            if (file != null)
                file.FilePath = ReferencePathAbsolute;
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
            if (!System.IO.File.Exists(ReferencePathAbsolute) || DetermineType(ReferencePathAbsolute) != typeof(T))
            {
                T file = createIfNotFound?.Invoke();
                if (file != null)
                    file.FilePath = ReferencePathAbsolute;
                File = file;
                if (File != null)
                    ExportReference();
            }
        }
        public FileRef(string dir, string name, EProprietaryFileFormat format) : base(dir, name, format) { }
        public FileRef(string dir, string name, EProprietaryFileFormat format, T file, bool exportNow) : this(dir, name, format)
        {
            if (file != null)
                file.FilePath = ReferencePathAbsolute;
            File = file;
            if (exportNow && File != null)
                ExportReference();
        }
        public FileRef(string dir, string name, EProprietaryFileFormat format, Func<T> createIfNotFound) : this(dir, name, format)
        {
            if (!System.IO.File.Exists(ReferencePathAbsolute) || DetermineType(ReferencePathAbsolute) != typeof(T))
            {
                T file = createIfNotFound?.Invoke();
                if (file != null)
                    file.FilePath = ReferencePathAbsolute;
                File = file;
                if (File != null)
                    ExportReference();
            }
        }
        public FileRef(T file) : this(file?.FilePath)
        {
            File = file;
        }
        #endregion

        //TODO: export map actors externally, relative to map / world file location
        [TSerialize(nameof(File), Condition = nameof(StoredInternally))]
        protected T _file;
        
        /// <summary>
        /// Returns true if the file object is not null, even if it was set using code rather than from a file.
        /// If set to false when true, the file object will be fully unloaded and set to null.
        /// </summary>
        [Description("Returns true if the file object is not null, even if it was set using code rather than from a file.\n" +
            "If set to false when true, the file object will be fully unloaded and set to null.")]
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
        public bool StoredInternally => string.IsNullOrWhiteSpace(ReferencePathAbsolute);

        [Browsable(false)]
        public override bool FileExists => File != null || base.FileExists;

        IFileObject IFileRef.File => File;
        
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
                    //if (_file.References.Contains(this))
                    //    _file.References.Remove(this);
                    Unloaded?.Invoke(_file);
                }

                _file = value;
                if (_file != null)
                {
                    string path = _file.FilePath;
                    if (!string.IsNullOrEmpty(path) && path.IsExistingDirectoryPath() == false)
                    {
                        ReferencePathAbsolute = path;
                        RegisterInstance();
                    }
                    else
                    {
                        ReferencePathAbsolute = null;// Path.DirectorySeparatorChar.ToString();
                    }
                    //if (!_file.References.Contains(this))
                    //    _file.References.Add(this);
                    LoadAttempted = true;
                }
                else
                {
                    ReferencePathAbsolute = null;
                }
            }
        }

        protected abstract bool RegisterInstance();

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

        public void ExportReference(ESerializeFlags flags = ESerializeFlags.Default) => _file?.Export(ReferencePathAbsolute, flags);
        public void ExportReference(string dir, string name, EFileFormat format, string thirdPartyExt = null, bool setPath = true, ESerializeFlags flags = ESerializeFlags.Default)
        {
            if (_file == null)
                return;
            _file.Export(dir, name, format, thirdPartyExt);
            if (setPath)
                ReferencePathAbsolute = _file.FilePath;
        }

        [Browsable(false)]
        public bool LoadAttempted { get; protected set; } = false;
        protected override void OnAbsoluteRefPathChanged(string oldAbsRefPath)
        {
            LoadAttempted = false;
            base.OnAbsoluteRefPathChanged(oldAbsRefPath);
        }
        public T GetInstance()
        {
            if (_file != null || LoadAttempted)
                return _file;

            Func<Task<T>> func = GetInstanceAsync;
            return func.RunSync();
        }
        
        public async Task<T> GetInstanceAsync() => await GetInstanceAsync(null, CancellationToken.None);
        public abstract Task<T> GetInstanceAsync(IProgress<float> progress, CancellationToken cancel);

        /// <summary>
        /// Unloads this reference to the file. 
        /// The file may still be allocated if referenced elsewhere.
        /// Deallocates the file if no other references exist.
        /// </summary>
        public void UnloadReference()
        {
            if (_file != null)
            {
                //_file.References.Remove(this);
                //if (_file.References.Count == 0)
                //    _file.Unload();
                _file = null;
                LoadAttempted = false;
            }
        }

        protected override void OnFileLoaded(T file)
        {
            //file.References.Add(this);
        }

        public static implicit operator T(FileRef<T> fileRef) => fileRef?.GetInstance();
    }
}
