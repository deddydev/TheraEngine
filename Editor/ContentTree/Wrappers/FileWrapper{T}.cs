using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Wrappers
{
    public class FileWrapper<T> : BaseProprietaryFileWrapper where T : class, IFileObject
    {
        public FileWrapper() : base() { }

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
