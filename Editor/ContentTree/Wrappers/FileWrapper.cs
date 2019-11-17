using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Wrappers
{
    public class FileWrapper : BaseProprietaryFileWrapper
    {
        public static TheraMenuOption RenameOption()
            => new TheraMenuOption("Rename", RenameAction, Keys.F2);
        public static TheraMenuOption ExplorerOption()
            => new TheraMenuOption("&Open In Explorer", ExplorerAction, Keys.Control | Keys.O);
        public static TheraMenuOption EditOption()
            => new TheraMenuOption("Edit", EditAction, Keys.F4);
        public static TheraMenuOption EditRawOption()
            => new TheraMenuOption("Edit Raw", EditRawAction, Keys.F3);
        public static TheraMenuOption CutOption()
            => new TheraMenuOption("&Cut", CutAction, Keys.Control | Keys.X);
        public static TheraMenuOption CopyOption()
            => new TheraMenuOption("&Copy", CopyAction, Keys.Control | Keys.C);
        public static TheraMenuOption PasteOption()
            => new TheraMenuOption("&Paste", PasteAction, Keys.Control | Keys.V);
        public static TheraMenuOption DeleteOption()
            => new TheraMenuOption("&Delete", DeleteAction, Keys.Control | Keys.Delete);

        public FileWrapper() : base() { }
        public FileWrapper(TheraMenu menu) : base(menu) { }

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
