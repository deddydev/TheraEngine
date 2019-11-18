using System.Windows.Forms;
using TheraEngine.Core.Files;

namespace TheraEditor.Wrappers
{
    public class FileWrapper : BaseProprietaryFileWrapper
    {
        public static TheraMenuOption RenameOption()
            => new TheraMenuOption("Rename", Rename, Keys.F2);
        public static TheraMenuOption ExplorerOption()
            => new TheraMenuOption("&Open In Explorer", Explorer, Keys.Control | Keys.O);
        public static TheraMenuOption EditOption()
            => new TheraMenuOption("Edit", Edit, Keys.F4);
        public static TheraMenuOption EditRawOption()
            => new TheraMenuOption("Edit Raw", EditRaw, Keys.F3);
        public static TheraMenuOption CutOption()
            => new TheraMenuOption("&Cut", Cut, Keys.Control | Keys.X);
        public static TheraMenuOption CopyOption()
            => new TheraMenuOption("&Copy", Copy, Keys.Control | Keys.C);
        public static TheraMenuOption PasteOption()
            => new TheraMenuOption("&Paste", Paste, Keys.Control | Keys.V);
        public static TheraMenuOption DeleteOption()
            => new TheraMenuOption("&Delete", Delete, Keys.Control | Keys.Delete);

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

        public void Rename() { }
        public void Explorer() { }
        public void Cut() { }
        public void Copy() { }
        public void Paste() { }
        public void Delete() { }
    }
}
