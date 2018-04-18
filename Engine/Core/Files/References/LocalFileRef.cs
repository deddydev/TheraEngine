using System;
using System.ComponentModel;

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
    public class LocalFileRef<T> : FileRef<T>, ILocalFileRef where T : TFileObject
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
        public LocalFileRef(string dir, string name, ProprietaryFileFormat format)
            : base(dir, name, format) { }
        public LocalFileRef(string dir, string name, ProprietaryFileFormat format, T file, bool exportNow)
            : base(dir, name, format, file, exportNow) { }
        public LocalFileRef(string dir, string name, ProprietaryFileFormat format, Func<T> createIfNotFound)
            : base(dir, name, format, createIfNotFound) { }

        protected override bool RegisterFile(string path, T file)
            => Engine.AddLocalFileInstance(path, file);

        public override T GetInstance()
        {
            if (_file != null)
                return _file;
            
            return File = LoadNewInstance(true, null, null);
        }
        
        public static implicit operator LocalFileRef<T>(T file) => file == null ? null : new LocalFileRef<T>(file);
        public static implicit operator LocalFileRef<T>(Type type) => new LocalFileRef<T>(type);
        public static implicit operator LocalFileRef<T>(string relativePath) => new LocalFileRef<T>(relativePath);
    }
}
