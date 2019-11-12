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

        public override TypeProxy FileType => TypeProxy.TypeOf<T>();

        protected LocalFileRef<T> _fileRef = new LocalFileRef<T>();

        public T Resource
        {
            get => ResourceRef.File;
            set => ResourceRef.File = value;
        }
        public LocalFileRef<T> ResourceRef
        {
            get => _fileRef;
            set => _fileRef = value ?? new LocalFileRef<T>();
        }

        public override bool IsLoaded => ResourceRef.IsLoaded;
        public override string FilePath
        {
            get => ResourceRef.Path.Path;
            set
            {
                ResourceRef.Path.Path = value;
            }
        }

        public T GetNewInstance()
            => _fileRef.LoadNewInstance();
        public async Task<T> GetNewInstanceAsync()
            => await _fileRef.LoadNewInstanceAsync();

        public IFileObject SingleInstance
        {
            get => ResourceRef.File;
            set
            {
                T obj = value as T;
                ResourceRef.File = obj;
            }
        }

        public override async void EditResource()
        {
            var file = await SingleInstanceRef.GetInstanceAsync();
            if (file is null)
                Engine.PrintLine($"Can't open file at {FilePath}.");
            else
            {
                Engine.PrintLine($"Editing file at {FilePath}.");

                AppDomainHelper.Sponsor(file);

                //TODO: pre-resolve editor type
                TypeProxy editorType = ResolveEditorType(FileType);
                if (editorType != null)
                    ShowEditor(editorType, file);
                else
                    Editor.SetPropertyGridObject(file);
            }
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
