﻿using System.Windows.Forms;
using TheraEngine.Core.Files;

namespace TheraEditor.Wrappers
{
    public class FileWrapper : BaseProprietaryFileWrapper
    {
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
