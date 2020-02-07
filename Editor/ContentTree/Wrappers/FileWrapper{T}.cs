using System;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Files;

namespace TheraEditor.Wrappers
{
    public class FileWrapper<T> : BaseFileWrapper where T : class, IFileObject
    {
        protected LocalFileRef<T> _fileRef = new LocalFileRef<T>();

        public T File
        {
            get => FileRef.File;
            set => FileRef.File = value;
        }
        public LocalFileRef<T> FileRef
        {
            get => _fileRef;
            set => _fileRef = value ?? new LocalFileRef<T>();
        }

        public async Task<T> GetFileAsync()
            => await FileRef.GetInstanceAsync();
        public async Task<T> GetFileAsync(IProgress<float> progress, CancellationToken cancel)
            => await FileRef.GetInstanceAsync(progress, cancel);

        public override IFileRef FileRefGeneric => _fileRef;
    }
}
