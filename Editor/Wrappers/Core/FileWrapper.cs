using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Wrappers
{
    public class FileWrapper<T> : BaseFileWrapper where T : class, IFileObject
    {
        public FileWrapper() : base() { }
        public FileWrapper(ContextMenuStrip menu) : base(menu) { }

        public override TypeProxy FileType => TypeProxy.TypeOf<T>();

        protected GlobalFileRef<T> _fileRef = new GlobalFileRef<T>();

        public T Resource
        {
            get => ResourceRef.File;
            set => ResourceRef.File = value;
        }
        public GlobalFileRef<T> ResourceRef
        {
            get => _fileRef;
            set => _fileRef = value ?? new GlobalFileRef<T>();
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

        public override IFileObject GetNewInstance() 
            => _fileRef.LoadNewInstance();
        public override async Task<IFileObject> GetNewInstanceAsync()
            => await _fileRef.LoadNewInstanceAsync();

        public override IFileObject SingleInstance
        {
            get => ResourceRef.File;
            set
            {
                T obj = value as T;
                ResourceRef.File = obj;
                Name = FilePath;
            }
        }
        public override IGlobalFileRef SingleInstanceRef => ResourceRef;
        protected internal override void FixPath(string parentFolderPath)
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
