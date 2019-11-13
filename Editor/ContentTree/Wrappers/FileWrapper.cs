﻿using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Wrappers
{
    public class FileWrapper : BaseFileWrapper
    {
        public FileWrapper() : base() { }
        public FileWrapper(TypeProxy t) => SetFileType(t);

        private TypeProxy _fileType;
        public override TypeProxy FileType => _fileType;
        public void SetFileType(TypeProxy type)
        {
            _fileType = type;
            _fileRef.SubType = type;
        }

        protected LocalFileRef<IFileObject> _fileRef = new LocalFileRef<IFileObject>();

        public IFileObject Resource
        {
            get => ResourceRef.File;
            set => ResourceRef.File = value;
        }
        public LocalFileRef<IFileObject> ResourceRef
        {
            get => _fileRef;
            set => _fileRef = value ?? new LocalFileRef<IFileObject>();
        }

        public override bool IsLoaded => ResourceRef.IsLoaded;
        public override string FilePath
        {
            get => ResourceRef.Path.Path;
            set
            {
                ResourceRef.Path.Path = value;
                Name = value;
            }
        }
        public override IFileObject GetNewInstance() => _fileRef.LoadNewInstance();
        public override async Task<IFileObject> GetNewInstanceAsync() => await _fileRef.LoadNewInstanceAsync();
        public override IFileObject SingleInstance
        {
            get => ResourceRef.File;
            set
            {
                IFileObject obj = value as IFileObject;
                ResourceRef.File = obj;
                Name = FilePath;
            }
        }
        public override IFileRef SingleInstanceRef => ResourceRef;
        protected internal override void SetPath(string parentFolderPath)
        {
            string fileName = Text;
            if (parentFolderPath[parentFolderPath.Length - 1] != Path.DirectorySeparatorChar)
                parentFolderPath += Path.DirectorySeparatorChar;
            FilePath = parentFolderPath + fileName;
        }
        //protected async void DefaultSaveText(DockableTextEditor obj)
        //{
        //    if (!(Resource is ITextSource source))
        //        return;

        //    source.Text = obj.GetText();

        //    string path = ResourceRef.Path.Absolute;

        //    Editor.Instance.ContentTree.BeginFileSaveWithProgress(path, "Saving text...", out Progress<float> progress, out CancellationTokenSource cancel);
        //    await ResourceRef.File.ExportAsync(path, ESerializeFlags.Default, progress, cancel.Token);
        //    Editor.Instance.ContentTree.EndFileSave(path);
        //}
    }
}
