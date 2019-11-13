using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Wrappers
{
    public class FileWrapper : BaseProprietaryFileWrapper
    {
        public FileWrapper() : base() { }
        public FileWrapper(TypeProxy t) : base(t) { }

        protected LocalFileRef<IFileObject> _fileRef = new LocalFileRef<IFileObject>();

        public IFileObject File
        {
            get => FileRef.File;
            set => FileRef.File = value;
        }
        public LocalFileRef<IFileObject> FileRef
        {
            get => _fileRef;
            set => _fileRef = value ?? new LocalFileRef<IFileObject>();
        }

        public override IFileRef FileRefGeneric => _fileRef;
    }
}
