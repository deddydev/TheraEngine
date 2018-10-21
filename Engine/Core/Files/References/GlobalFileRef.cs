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

    public interface IGlobalFilesContext<T> where T : class, IFileObject
    {
        ConcurrentDictionary<string, GlobalFileRef<T>> GlobalFileInstances { get; }
    }
    public interface IGlobalFilesContext
    {
        ConcurrentDictionary<string, IGlobalFileRef> GlobalFileInstances { get; }
    }

    /// <summary>
    /// Allows only one loaded instance of this file throughout the program.
    /// File can be loaded on-demand or preloaded.
    /// </summary>
    [FileExt("gref")]
    [FileDef("Global File Reference")]
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
                if (string.IsNullOrEmpty(ReferencePathAbsolute) || IsLoaded)
                    return false;

                Context.GlobalFileInstances.AddOrUpdate(ReferencePathAbsolute, this, (key, oldValue) => this);

                return true;
            }
            else
                return Engine.AddGlobalFileInstance(this);
        }

        protected override void OnAbsoluteRefPathChanged(string oldAbsRefPath)
        {
            base.OnAbsoluteRefPathChanged(oldAbsRefPath);
            if (IsLoaded)
            {
                if (Context != null)
                {
                    if (!string.IsNullOrEmpty(oldAbsRefPath))
                        Context.GlobalFileInstances.TryRemove(oldAbsRefPath, out GlobalFileRef<T> value);
                    if (!string.IsNullOrEmpty(ReferencePathAbsolute))
                        Context.GlobalFileInstances.AddOrUpdate(ReferencePathAbsolute, this, (key, oldValue) => this);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(oldAbsRefPath))
                    Engine.RemoveGlobalFileInstance(oldAbsRefPath);
                if (!string.IsNullOrEmpty(ReferencePathAbsolute))
                    Engine.AddGlobalFileInstance(this);
            }
        }

        [Browsable(false)]
        public IGlobalFilesContext<T> Context { get; set; } = null;

        public override async Task<T> GetInstanceAsync(IProgress<float> progress, CancellationToken cancel)
        {
            if (_file != null || LoadAttempted)
                return _file;

            LoadAttempted = true;
            string absolutePath = ReferencePathAbsolute;
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

            T value = await LoadNewInstanceAsync(false, null, progress, cancel);
            File = value;
            return value;
        }
        
        public static implicit operator GlobalFileRef<T>(T file) => file == null ? null : new GlobalFileRef<T>(file);
        public static implicit operator GlobalFileRef<T>(Type type) => new GlobalFileRef<T>(type);
        public static implicit operator GlobalFileRef<T>(string relativePath) => new GlobalFileRef<T>(relativePath);

        public override string ToString()
        {
            return $"Global File Ref [{(IsLoaded ? "Loaded" : "Unloaded")}]: {(string.IsNullOrWhiteSpace(ReferencePathAbsolute) ? "<null path>" : ReferencePathAbsolute)}";
        }
    }
}
