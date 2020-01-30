using Extensions;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files
{
    public interface IGlobalFileRef : IFileRef
    {

    }
    public interface IGlobalFileRef<T> : IFileRef<T> where T : class, IFileObject
    {
        IGlobalFilesContext<T> Context { get; set; }
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
    /// Allows only one loaded instance of this file throughout the program.
    /// File can be loaded on-demand or preloaded.
    /// </summary>
    [Serializable]
    [TFileExt("gref")]
    [TFileDef("Global File Reference")]
    public class GlobalFileRef<T> : 
        FileRef<T>, IGlobalFileRef<T>, IGlobalFileRef
        where T : class, IFileObject
    {
        public GlobalFileRef()
            : base() { }
        public GlobalFileRef(TypeProxy type) 
            : base(type) { }
        public GlobalFileRef(T file)
            : base(file) { }
        public GlobalFileRef(string filePath)
            : base(filePath) { }
        public GlobalFileRef(string filePath, TypeProxy type) 
            : base(filePath, type) { }
        public GlobalFileRef(string filePath, T file) 
            : base(file, filePath) { }
        public GlobalFileRef(string filePath, Func<T> createIfNotFound)
            : base(createIfNotFound, filePath) { }
        public GlobalFileRef(string dir, string name, EProprietaryFileFormat format) 
            : base(dir, name, format) { }
        public GlobalFileRef(string dir, string name, EProprietaryFileFormat format, T file) 
            : base(dir, name, format, file) { }
        public GlobalFileRef(string dir, string name, EProprietaryFileFormat format, Func<T> createIfNotFound)
            : base(dir, name, format, createIfNotFound) { }

        internal static void AddGlobalFileInstance(string path, T file)
        {
            if (string.IsNullOrEmpty(path))
                return;

            //if (file.Domain == AppDomain.CurrentDomain && AppDomainHelper.IsPrimaryDomain)
            //    throw new Exception();

            Engine.DomainProxy?.AddGlobalFile(path, file);
        }
        internal static void RemoveGlobalFileInstance(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            Engine.DomainProxy?.RemoveGlobalFile(path);
        }

        protected override void RegisterInstance()
        {
            T file = IsLoaded ? File : null;
            if (Context != null)
            {
                if (string.IsNullOrEmpty(Path.Path) || IsLoaded)
                    return;

                Context.GlobalFileInstances.AddOrUpdate(Path.Path, file, (key, oldValue) => file);
            }
            else
                AddGlobalFileInstance(Path.Path, file);
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
                    AddGlobalFileInstance(Path.Path, default);
            }
        }

        /// <summary>
        /// This is the context used to determine if the file is loaded or not.
        /// If null, uses the engine itself as a global context.
        /// </summary>
        [Browsable(false)]
        public IGlobalFilesContext<T> Context { get; set; } = null;

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
                        if (Engine.DomainProxy.GetGlobalFile(absolutePath, out IFileObject file))
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
                            else
                                Engine.DomainProxy.RemoveGlobalFile(absolutePath);
                        }
                    }
                    else
                    {
                        if (Context.GlobalFileInstances.TryGetValue(absolutePath, out T file))
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
                            else
                                Context.GlobalFileInstances.TryRemove(absolutePath, out _);
                        }
                    }
                }

                bool allowLoad = !LoadAttempted;
                var (instance, _, loadAttempted) = await LoadNewInstanceAsync(progress, cancel, allowLoad);
                if (allowLoad)
                    LoadAttempted = loadAttempted;

                File = value = instance;

                if (absolutePath != null && instance != null)
                {
                    if (Context is null)
                    {
                        AddGlobalFileInstance(absolutePath, instance);
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
