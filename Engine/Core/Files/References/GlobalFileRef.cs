using Extensions;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files
{
    public interface IGlobalFileRef : IFileRef
    {

    }
    /// <summary>
    /// Add this interface to a class to use that class as the handler for global instances.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGlobalFilesContext<T> where T : class, IFileObject
    {
        ConcurrentDictionary<string, T> GlobalFileInstances { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    //public interface IGlobalFilesContext
    //{
    //    ConcurrentDictionary<string, IGlobalFileRef> GlobalFileInstances { get; }
    //}

    /// <summary>
    /// Allows only one loaded instance of this file throughout the program.
    /// File can be loaded on-demand or preloaded.
    /// </summary>
    [Serializable]
    [TFileExt("gref")]
    [TFileDef("Global File Reference")]
    public class GlobalFileRef<T> : FileRef<T>, IGlobalFileRef where T : class, IFileObject
    {
        public GlobalFileRef()
            : base() { }
        public GlobalFileRef(Type type) 
            : base(type) { }
        public GlobalFileRef(T file)
            : base(file) { }
        public GlobalFileRef(string filePath)
            : base(filePath) { }
        public GlobalFileRef(string filePath, Type type) 
            : base(filePath, type) { }
        public GlobalFileRef(string filePath, T file) 
            : base(filePath, file) { }
        public GlobalFileRef(string filePath, Func<T> createIfNotFound)
            : base(filePath, createIfNotFound) { }
        public GlobalFileRef(string dir, string name, EProprietaryFileFormat format) 
            : base(dir, name, format) { }
        public GlobalFileRef(string dir, string name, EProprietaryFileFormat format, T file) 
            : base(dir, name, format, file) { }
        public GlobalFileRef(string dir, string name, EProprietaryFileFormat format, Func<T> createIfNotFound)
            : base(dir, name, format, createIfNotFound) { }

        internal static bool AddGlobalFileInstance(T file, string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            GlobalFileInstances.AddOrUpdate(path, file, (key, oldValue) => file);

            return true;
        }
        internal static bool RemoveGlobalFileInstance(string absRefPath)
        {
            if (string.IsNullOrEmpty(absRefPath))
                return false;

            return GlobalFileInstances.TryRemove(absRefPath, out IFileObject value);
        }

        protected override bool RegisterInstance()
        {
            T file = IsLoaded ? File : null;
            if (Context != null)
            {
                if (string.IsNullOrEmpty(Path.Path) || IsLoaded)
                    return false;

                Context.GlobalFileInstances.AddOrUpdate(Path.Path, file, (key, oldValue) => file);

                return true;
            }
            else
                return AddGlobalFileInstance(file, Path.Path);
        }

        protected override void OnAbsoluteRefPathChanged(string oldPath, string newPath)
        {
            base.OnAbsoluteRefPathChanged(oldPath, newPath);
            if (IsLoaded)
            {
                if (Context != null)
                {
                    if (!string.IsNullOrEmpty(oldPath))
                        Context.GlobalFileInstances.TryRemove(oldPath, out T value);
                    if (!string.IsNullOrEmpty(newPath))
                    {
                        T file = File;
                        Context.GlobalFileInstances.AddOrUpdate(newPath, file, (key, oldValue) => file);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(oldPath))
                    RemoveGlobalFileInstance(oldPath);
                if (!string.IsNullOrEmpty(newPath))
                    AddGlobalFileInstance(default(T), Path.Path);
            }
        }

        /// <summary>
        /// This is the context used to determine if the file is loaded or not.
        /// If null, uses the engine itself as a global context.
        /// </summary>
        [Browsable(false)]
        public IGlobalFilesContext<T> Context { get; set; } = null;

        /// <summary>
        /// Instances of files that are loaded only once and are accessable by all global references to that file.
        /// </summary>
        public static ConcurrentDictionary<string, IFileObject> GlobalFileInstances { get; } = new ConcurrentDictionary<string, IFileObject>();
        public bool LoadInGameDomain { get; set; }

        public override async Task<T> GetInstanceAsync(IProgress<float> progress, CancellationToken cancel)
        {
            if (_file != null)
                return _file;

            IsLoading = true;
            T value = null;
            try
            {
                string absolutePath = Path.Path;
                if (absolutePath != null)
                {
                    if (Context is null)
                    {
                        if (GlobalFileInstances.TryGetValue(absolutePath, out IFileObject file))
                        {
                            if (file != null)
                            {
                                if (file is T casted)
                                {
                                    //casted.References.Add(this);
                                    File = casted;
                                    return casted;
                                }
                                else
                                    Engine.LogWarning(file.GetType().GetFriendlyName() + " cannot be casted to " + typeof(T).GetFriendlyName());
                            }
                        }
                    }
                    else
                    {
                        if (Context.GlobalFileInstances.TryGetValue(absolutePath, out T file))
                        {
                            File = file;
                            return file;
                        }
                    }
                }

                bool allowLoad = !LoadAttempted;
                var (instance, _, loadAttempted) = await LoadNewInstanceAsync(progress, cancel, allowLoad);
                if (allowLoad)
                    LoadAttempted = loadAttempted;

                File = value = instance;

                if (absolutePath != null)
                {
                    if (Context is null)
                    {
                        GlobalFileInstances.AddOrUpdate(absolutePath, instance, (key, oldValue) => instance);
                    }
                    else
                    {
                        Context.GlobalFileInstances.AddOrUpdate(absolutePath, instance, (key, oldValue) => instance);
                    }
                }
            }
            catch
            {

            }
            finally
            {
                IsLoading = false;
            }
            return value;
        }
        
        public static implicit operator GlobalFileRef<T>(T file) => file is null ? null : new GlobalFileRef<T>(file);
        public static implicit operator GlobalFileRef<T>(Type type) => new GlobalFileRef<T>(type);
        public static implicit operator GlobalFileRef<T>(string relativePath) => new GlobalFileRef<T>(relativePath);

        public override string ToString()
        {
            return $"Global File Ref [{(IsLoaded ? "Loaded" : "Unloaded")}]: {(string.IsNullOrWhiteSpace(Path.Path) ? "<null path>" : Path.Path)}";
        }
    }
}
