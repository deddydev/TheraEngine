using Extensions;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files
{
    public interface IFileRef : IFileLoader
    {
        bool IsLoaded { get; set; }
        bool StoredInternally { get; }
        bool LoadAttempted { get; }

        IFileObject File { get; set; }

        IFileObject GetInstance();
        Task<IFileObject> GetInstanceAsync();
        Task<IFileObject> GetInstanceAsync(IProgress<float> progress, CancellationToken cancel);

        void UnloadReference();
    }
    public interface IFileRef<T> : IFileLoader<T> where T : class, IFileObject
    {
        event Action<T> Unloaded;

        bool IsLoaded { get; set; }
        bool StoredInternally { get; }
        bool LoadAttempted { get; }

        T File { get; set; }

        T GetInstance();
        Task<T> GetInstanceAsync();
        Task<T> GetInstanceAsync(IProgress<float> progress, CancellationToken cancel);

        void UnloadReference();
    }
    /// <summary>
    /// Indicates that this variable references a file that must be loaded.
    /// </summary>
    [Serializable]
    [TFileExt("ref")]
    public abstract class FileRef<T> : FileLoader<T>, IFileRef<T>, IFileRef where T : class, IFileObject
    {
        #region Constructors
        public FileRef() : this(string.Empty) { }
        public FileRef(TypeProxy type) : this(null, type) { }
        public FileRef(string filePath) : this(filePath, null) { }
        public FileRef(string filePath, TypeProxy type) : base(filePath, type)
        {
            if (AppDomainHelper.IsPrimaryDomain)
                Engine.Instance.DomainProxyDestroying += Instance_DomainProxyPreUnset;
        }
        public FileRef(T file, string filePath) : this(filePath)
        {
            if (file != null)
                file.FilePath = Path.Path;
            File = file;
            //if (exportNow && File != null)
            //    ExportReference();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="createIfNotFound"></param>
        public FileRef(Func<T> createIfNotFound, string filePath) : this(filePath)
        {
            string absPath = Path.Path;
            if (!System.IO.File.Exists(absPath) || DetermineType(Path.Path, out EFileFormat format) != typeof(T))
            {
                T file = createIfNotFound?.Invoke();
                if (file != null)
                    file.FilePath = absPath;
                File = file;
                //if (File != null)
                //    ExportReferenceAsync();
            }
        }
        public FileRef(string dir, string name, EProprietaryFileFormat format) : base(dir, name, format) 
        {
            if (AppDomainHelper.IsPrimaryDomain)
                Engine.Instance.DomainProxyDestroying += Instance_DomainProxyPreUnset;
        }
        public FileRef(string dir, string name, EProprietaryFileFormat format, T file) : this(dir, name, format)
        {
            if (file != null)
                file.FilePath = Path.Path;
            File = file;
            //if (exportNow && File != null)
            //    ExportReference();
        }
        public FileRef(string dir, string name, EProprietaryFileFormat format, Func<T> createIfNotFound) : this(dir, name, format)
        {
            string absPath = Path.Path;
            if (!System.IO.File.Exists(absPath) || DetermineType(absPath) != typeof(T))
            {
                T file = createIfNotFound?.Invoke();
                if (file != null)
                    file.FilePath = absPath;
                File = file;
                //if (File != null)
                //    ExportReference();
            }
        }
        public FileRef(T file) : this(file?.FilePath)
        {
            File = file;
        }
        #endregion

        protected void Instance_DomainProxyPreUnset(EngineDomainProxy obj)
        {
            FileType = null;
            IsLoaded = false;
        }

        //TODO: export map actors externally, relative to map / world file location
        [TSerialize(nameof(File), Condition = nameof(StoredInternally))]
        public T _file;
        
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
                if (value == IsLoaded)
                    return;

                UnloadReference();
                if (value)
                    LoadInstance();
            }
        }
        public async void Reload()
        {
            LoadAttempted = false;
            UnloadReference();
            await GetInstanceAsync();
        }

        /// <summary>
        /// If true, the referenced file will be written within the parent file's data
        /// and loaded with the parent file instead of being loaded on demand from the external file.
        /// </summary>
        //[Browsable(false)]
        [Category("File Reference")]
        public bool StoredInternally => string.IsNullOrWhiteSpace(Path.Path);

        [Browsable(false)]
        public override bool FileExists => IsLoaded || base.FileExists;

        IFileObject IFileRef.File { get => File; set => File = value as T; }
        T IFileRef<T>.File { get => File; set => File = value; }

        [Category("File Reference")]
        //[BrowsableIf("_file != null")]
        public T File
        {
            get => GetInstance();
            set => Set(ref _file, value, PreFileSet, PostFileSet, true);
        }

        private void PreFileSet()
        {
            var temp = _file;
            _file = null;
            //if (_file != null)
            //{
            //    //if (_file.References.Contains(this))
            //    //    _file.References.Remove(this);

            //}
            if (temp != null)
                Unloaded?.Invoke(temp);
        }
        private void PostFileSet()
        {
            if (_file != null)
            {
                AppDomainHelper.Sponsor(_file);

                string path = _file.FilePath;
                if (!string.IsNullOrEmpty(path) && path.IsExistingDirectoryPath() == false)
                {
                    Path.Path = path;
                }
                else
                {
                    //Path.Absolute = null;// Path.DirectorySeparatorChar.ToString();
                }
                //if (!_file.References.Contains(this))
                //    _file.References.Add(this);
                //LoadAttempted = true;
                OnLoaded(_file);
            }
            else
            {
                //Path.Absolute = null;
            }
            RegisterInstance();
        }
        protected abstract void RegisterInstance();

        /// <summary>
        /// 
        /// </summary>
        public event Action<T> Unloaded;

        /// <summary>
        /// Event called when the file this reference points to becomes available.
        /// NOTE: Event is fired on subscribe if the item is currently loaded!
        /// </summary>
        public override event Action<T> Loaded
        {
            add
            {
                base.Loaded += value;
                if (_file != null)
                    value?.Invoke(_file);
            }
            remove
            {
                base.Loaded -= value;
            }
        }

        //public async Task ExportReferenceAsync(ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
        //    => await _file?.ExportAsync(ReferencePathAbsolute, flags, progress, cancel);
        //public async Task ExportReferenceAsync(
        //    string dir,
        //    string name, 
        //    EFileFormat format,
        //    string thirdPartyExt, 
        //    bool setPath,
        //    ESerializeFlags flags,
        //    IProgress<float> progress, 
        //    CancellationToken cancel)
        //{
        //    if (_file is null)
        //        return;

        //    await _file.ExportAsync(dir, name, format, thirdPartyExt, flags, progress, cancel);
        //    if (setPath)
        //        ReferencePathAbsolute = _file.FilePath;
        //}

        //[Browsable(false)]
        [Category("Loading")]
        public bool LoadAttempted { get; protected set; } = false;
        //[Browsable(false)]
        [Category("Loading")]
        public bool IsLoading { get; protected set; } = false;

        protected override void OnAbsoluteRefPathChanged(string oldPath, string newPath)
        {
            LoadAttempted = false;
            base.OnAbsoluteRefPathChanged(oldPath, newPath);
        }

        IFileObject IFileRef.GetInstance() => GetInstance();
        public T GetInstance()
        {
            if (RootFile?.IsSerializing ?? false && !StoredInternally)
                return _file;

            if (IsLoading || IsLoaded)
                return _file;

            GetInstanceAsync();

            return null;
        }

        public async void LoadInstance() 
            => await GetInstanceAsync();

        async Task<IFileObject> IFileRef.GetInstanceAsync() 
            => await GetInstanceAsync();
        async Task<IFileObject> IFileRef.GetInstanceAsync(IProgress<float> progress, CancellationToken cancel)
            => await GetInstanceAsync(progress, cancel);

        public async Task<T> GetInstanceAsync() 
            => await GetInstanceAsync(null, CancellationToken.None);

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
            }
            LoadAttempted = false;
        }

        protected override void OnFileLoaded(T file)
        {
            //file.References.Add(this);
        }

        public static implicit operator T(FileRef<T> fileRef) => fileRef?.GetInstance();
    }
}
