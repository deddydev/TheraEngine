using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Files;

namespace TheraEditor.Wrappers
{
    public class FileWrapper<T> : BaseFileWrapper where T : TFileObject
    {
        public FileWrapper() : base() { }
        public FileWrapper(ContextMenuStrip menu) : base(menu) { }

        public override Type FileType => typeof(T);

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
            get => ResourceRef.ReferencePathAbsolute;
            set
            {
                ResourceRef.ReferencePathAbsolute = value;
                Name = value;
            }
        }
        public override TFileObject GetNewInstance() => _fileRef.LoadNewInstance();
        public override async Task<TFileObject> GetNewInstanceAsync() => await _fileRef.LoadNewInstanceAsync();
        public override TFileObject SingleInstance
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
        protected void DefaultSaveText(DockableTextEditor obj)
        {
            ITextSource source = Resource as ITextSource;
            if (source == null)
                return;
            source.Text = obj.GetText();
            Editor.Instance.ContentTree.WatchProjectDirectory = false;
            ResourceRef.ExportReference();
            Editor.Instance.ContentTree.WatchProjectDirectory = true;
        }
    }
}
