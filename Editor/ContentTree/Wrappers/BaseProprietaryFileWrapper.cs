using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Wrappers
{
    public abstract class BaseProprietaryFileWrapper : BaseFileWrapper
    {
        public void Reload()
        {
            bool wasLoaded = FileRefGeneric.IsLoaded;
            if (wasLoaded)
                FileRefGeneric.Unload();
            FileRefGeneric.IsLoaded = wasLoaded;
        }

        public bool IsLoaded => FileRefGeneric.IsLoaded;
        public bool AlwaysReload { get; set; } = false;
        public bool ExternallyModified { get; set; } = false;

        public IFileObject GetFileGeneric() 
            => FileRefGeneric.GetInstance();

        public async Task<IFileObject> GetFileGenericAsync() 
            => await FileRefGeneric.GetInstanceAsync();
        public async Task<IFileObject> GetFileGenericAsync(IProgress<float> progress, CancellationToken cancel)
            => await FileRefGeneric.GetInstanceAsync(progress, cancel);

        private TypeProxy _fileType;
        public TypeProxy FileType 
        {
            get => _fileType;
            set
            {
                _fileType = value;
                FileRefGeneric.SubType = _fileType;
            }
        }

        public override string FilePath
        {
            get => FileRefGeneric.Path.Path;
            set => FileRefGeneric.Path.Path = value;
        }

        public abstract IFileRef FileRefGeneric { get; }

        public override void Edit()
        {
            var file = GetFileGeneric();

            _fileType = file.GetTypeProxy();

            if (file is null)
                Engine.PrintLine($"Can't open file at {FilePath}.");
            else
            {
                Engine.PrintLine($"Editing file at {FilePath}.");

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
