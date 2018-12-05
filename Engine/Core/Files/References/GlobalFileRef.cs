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
        ConcurrentDictionary<string, GlobalFileRef<T>> GlobalFileInstances { get; }
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

        protected override bool RegisterInstance()
        {
            if (Context != null)
            {
                if (string.IsNullOrEmpty(Path.Absolute) || IsLoaded)
                    return false;

                Context.GlobalFileInstances.AddOrUpdate(Path.Absolute, this, (key, oldValue) => this);

                return true;
            }
            else
                return Engine.AddGlobalFileInstance(this);
        }

        protected override void OnAbsoluteRefPathChanged(string oldPath, string newPath)
        {
            base.OnAbsoluteRefPathChanged(oldPath, newPath);
            if (IsLoaded)
            {
                if (Context != null)
                {
                    if (!string.IsNullOrEmpty(oldPath))
                        Context.GlobalFileInstances.TryRemove(oldPath, out GlobalFileRef<T> value);
                    if (!string.IsNullOrEmpty(newPath))
                        Context.GlobalFileInstances.AddOrUpdate(newPath, this, (key, oldValue) => this);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(oldPath))
                    Engine.RemoveGlobalFileInstance(oldPath);
                if (!string.IsNullOrEmpty(newPath))
                    Engine.AddGlobalFileInstance(this);
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
            if (_file != null || LoadAttempted)
                return _file;

            LoadAttempted = true;
            string absolutePath = Path.Absolute;
            if (absolutePath != null)
            {
                if (Context == null)
                {
                    if (Engine.GlobalFileInstances.TryGetValue(absolutePath, out IGlobalFileRef fileRef))
                    {
                        if (fileRef.File is T casted)
                        {
                            //casted.References.Add(this);
                            File = casted;
                        }
                        else
                            Engine.LogWarning(fileRef.File.GetType().GetFriendlyName() + " cannot be casted to " + typeof(T).GetFriendlyName());
                    }
                }
                else
                {
                    if (Context.GlobalFileInstances.TryGetValue(absolutePath, out GlobalFileRef<T> fileRef))
                    {
                        File = fileRef.File;
                    }
                }
            }

            T value = await LoadNewInstanceAsync(progress, cancel);
            File = value;
            return value;
        }
        
        public static implicit operator GlobalFileRef<T>(T file) => file == null ? null : new GlobalFileRef<T>(file);
        public static implicit operator GlobalFileRef<T>(Type type) => new GlobalFileRef<T>(type);
        public static implicit operator GlobalFileRef<T>(string relativePath) => new GlobalFileRef<T>(relativePath);

        public override string ToString()
        {
            return $"Global File Ref [{(IsLoaded ? "Loaded" : "Unloaded")}]: {(string.IsNullOrWhiteSpace(Path.Absolute) ? "<null path>" : Path.Absolute)}";
        }
    }
}
