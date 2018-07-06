using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace TheraEngine.Files
{
    public interface ILocalFileRef : IFileRef
    {

    }
    /// <summary>
    /// Allows only one loaded instance of the file for just this reference.
    /// Other local references with the same file path will load their own new instances of the file.
    /// File can be loaded on-demand or preloaded.
    /// </summary>
    [FileExt("lref")]
    [FileDef("Local File Reference")]
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
        public LocalFileRef(string filePath, T file, bool exportNow)
            : base(filePath, file, exportNow) { }
        public LocalFileRef(string filePath, Func<T> createIfNotFound)
            : base(filePath, createIfNotFound) { }
        public LocalFileRef(string dir, string name, EProprietaryFileFormat format)
            : base(dir, name, format) { }
        public LocalFileRef(string dir, string name, EProprietaryFileFormat format, T file, bool exportNow)
            : base(dir, name, format, file, exportNow) { }
        public LocalFileRef(string dir, string name, EProprietaryFileFormat format, Func<T> createIfNotFound)
            : base(dir, name, format, createIfNotFound) { }

        protected override bool RegisterFile(string path, T file)
            => Engine.AddLocalFileInstance(path, file);

        public override async Task<T> GetInstanceAsync(IProgress<float> progress, CancellationToken cancel)
        {
            if (_file != null || LoadAttempted)
                return _file;
            
            T value = await LoadNewInstanceAsync(false, null, progress, cancel);
            File = value;
            LoadAttempted = true;
            return value;
        }
        
        public static implicit operator LocalFileRef<T>(T file) => file == null ? null : new LocalFileRef<T>(file);
        public static implicit operator LocalFileRef<T>(Type type) => new LocalFileRef<T>(type);
        public static implicit operator LocalFileRef<T>(string relativePath) => new LocalFileRef<T>(relativePath);
    }
}
