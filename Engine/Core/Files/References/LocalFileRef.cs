using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files
{
    public interface ILocalFileRef : IFileRef
    {

    }
    /// <summary>
    /// Allows only one loaded instance of the file for just this reference.
    /// Other local references with the same file path will load their own new instances of the file.
    /// File can be loaded on-demand or preloaded.
    /// </summary>
    [TFileExt("lref")]
    [TFileDef("Local File Reference")]
    public class LocalFileRef<T> : FileRef<T>, ILocalFileRef where T : class, IFileObject
    {
        public LocalFileRef()
            : base() { }
        public LocalFileRef(Type type)
            : base(type) { }
        public LocalFileRef(T file)
            : base(file) { }
        public LocalFileRef(string filePath)
            : base(filePath) { }
        public LocalFileRef(string filePath, Type type)
            : base(filePath, type) { }
        public LocalFileRef(string filePath, T file)
            : base(filePath, file) { }
        public LocalFileRef(string filePath, Func<T> createIfNotFound)
            : base(filePath, createIfNotFound) { }
        public LocalFileRef(string dir, string name, EProprietaryFileFormat format)
            : base(dir, name, format) { }
        public LocalFileRef(string dir, string name, EProprietaryFileFormat format, T file)
            : base(dir, name, format, file) { }
        public LocalFileRef(string dir, string name, EProprietaryFileFormat format, Func<T> createIfNotFound)
            : base(dir, name, format, createIfNotFound) { }

        protected override bool RegisterInstance()
        {
            //Engine.AddLocalFileInstance(path, file);
            return true;
        }

        public override async Task<T> GetInstanceAsync(IProgress<float> progress, CancellationToken cancel)
        {
            if (_file != null || LoadAttempted)
                return _file;

            LoadAttempted = true;
            T value = await LoadNewInstanceAsync(false, null, progress, cancel);
            File = value;
            return value;
        }
        
        public static implicit operator LocalFileRef<T>(T file) => file == null ? null : new LocalFileRef<T>(file);
        public static implicit operator LocalFileRef<T>(Type type) => new LocalFileRef<T>(type);
        public static implicit operator LocalFileRef<T>(string relativePath) => new LocalFileRef<T>(relativePath);

        public override string ToString()
        {
            return $"Local File Ref [{(IsLoaded ? "Loaded" : "Unloaded")}]: {(string.IsNullOrWhiteSpace(Path.Absolute) ? "<null path>" : Path.Absolute)}";
        }
    }
}
