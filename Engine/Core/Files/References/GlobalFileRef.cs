using System;
using System.ComponentModel;

namespace TheraEngine.Files
{
    public interface IGlobalFileRef : IFileRef
    {

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
        public GlobalFileRef(string filePath, T file, bool exportNow) 
            : base(filePath, file, exportNow) { }
        public GlobalFileRef(string filePath, Func<T> createIfNotFound)
            : base(filePath, createIfNotFound) { }
        public GlobalFileRef(string dir, string name, ProprietaryFileFormat format) 
            : base(dir, name, format) { }
        public GlobalFileRef(string dir, string name, ProprietaryFileFormat format, T file, bool exportNow) 
            : base(dir, name, format, file, exportNow) { }
        public GlobalFileRef(string dir, string name, ProprietaryFileFormat format, Func<T> createIfNotFound)
            : base(dir, name, format, createIfNotFound) { }

        protected override bool RegisterFile(string path, T file)
            => Engine.AddGlobalFileInstance(path, file);

        public override T GetInstance()
        {
            if (_file != null)
                return _file;

            string absolutePath = ReferencePathAbsolute;
            if (absolutePath != null && Engine.GlobalFileInstances.TryGetValue(absolutePath, out IFileObject file))
            {
                //lock (file)
                //{
                if (file is T casted)
                {
                    casted.References.Add(this);
                    File = casted;
                }
                else
                    throw new InvalidOperationException(file.GetType().GetFriendlyName() + " cannot be casted to " + typeof(T).GetFriendlyName());
                //}
            }

            return File = LoadNewInstance(false, null, null);
        }
        
        public static implicit operator GlobalFileRef<T>(T file) => file == null ? null : new GlobalFileRef<T>(file);
        public static implicit operator GlobalFileRef<T>(Type type) => new GlobalFileRef<T>(type);
        public static implicit operator GlobalFileRef<T>(string relativePath) => new GlobalFileRef<T>(relativePath);
    }
}
