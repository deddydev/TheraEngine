using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    public interface IBaseFileWrapper : IBasePathWrapper
    {
        TypeProxy FileType { get; }
        bool IsLoaded { get; }
        bool AlwaysReload { get; set; }
        bool ExternallyModified { get; set; }

        IFileObject GetFileGeneric();
        Task<IFileObject> GetFileGenericAsync();
        Task<IFileObject> GetFileGenericAsync(IProgress<float> progress, CancellationToken cancel);

        void Reload();
        void Edit();
        void EditRaw();
    }
    public abstract class BaseFileWrapper : BasePathWrapper, IBaseFileWrapper
    {
        public void Reload()
        {
            var fref = FileRefGeneric;
            if (fref is null)
                return;

            bool wasLoaded = fref.IsLoaded;
            if (wasLoaded)
                fref.Unload();
            fref.IsLoaded = wasLoaded;
        }

        public bool IsLoaded => FileRefGeneric?.IsLoaded ?? false;
        public bool AlwaysReload { get; set; } = false;
        public bool ExternallyModified { get; set; } = false;

        public IFileObject GetFileGeneric() 
            => FileRefGeneric?.GetInstance();
        public async Task<IFileObject> GetFileGenericAsync()
        {
            if (FileRefGeneric is null)
                return null;

            return await FileRefGeneric.GetInstanceAsync();
        }
        public async Task<IFileObject> GetFileGenericAsync(IProgress<float> progress, CancellationToken cancel)
        {
            if (FileRefGeneric is null)
                return null;

            return await FileRefGeneric.GetInstanceAsync(progress, cancel);
        }

        private TypeProxy _fileType;
        public TypeProxy FileType 
        {
            get => _fileType;
            set
            {
                _fileType = value;

                if (FileRefGeneric != null)
                    FileRefGeneric.SubType = _fileType;
            }
        }

        private string _filePath;
        public override string FilePath
        {
            get => FileRefGeneric?.Path?.Path ?? _filePath;
            set
            {
                _filePath = value;

                if (FileRefGeneric?.Path != null)
                    FileRefGeneric.Path.Path = value;
            }
        }

        public abstract IFileRef FileRefGeneric { get; }

        public virtual void Edit()
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

        public virtual async void EditRaw()
        {
            var file = await TFileObject.LoadAsync<TextFile>(FilePath);
            if (file != null)
                DockableTextEditor.ShowNew(Editor.Instance.DockPanel, DockState.Document, file);
        }

        protected static void ShowEditor(TypeProxy editorType, IFileObject file)
        {
            //TODO: Casting TypeProxy to Type: type cannot be for a user-created form.
            //Try to fix this? Probably need to create the form in the game domain,
            //but I'm not sure if that form can be hosted in the DockPanel on the UI domain
            Type type = (Type)editorType;
            Form form = Activator.CreateInstance(type, file) as Form;

            if (form is DockContent dc && !(form is TheraForm))
                dc.Show(Editor.Instance.DockPanel, DockState.Document);
            else
                form?.ShowDialog(Editor.Instance);
        }

        public static TypeProxy ResolveEditorType(TypeProxy fileType)
        {
            if (fileType is null)
                return null;

            EngineDomainProxyEditor proxy = Engine.DomainProxy as EngineDomainProxyEditor;
            var editorTypes = proxy.FullEditorTypes;

            if (TypeProxy.AnyBaseTypeMatches(fileType, type => editorTypes.ContainsKey(type), out TypeProxy match, true))
                return editorTypes[match];

            return null;
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
