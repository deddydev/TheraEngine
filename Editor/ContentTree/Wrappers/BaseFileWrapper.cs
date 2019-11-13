using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    public interface IBaseProprietaryFileWrapper : IBaseFileWrapper
    {
        TypeProxy FileType { get; }
        bool IsLoaded { get; }

        IFileObject GetFile();
        Task<IFileObject> GetFileAsync();
    }
    public interface IBaseFileWrapper : IObjectSlim
    {
        string FilePath { get; set; }
        ITheraMenu Menu { get; set; }

        void Edit();
        void EditRaw();
    }
    public abstract class BaseFileWrapper : TObjectSlim, IBaseFileWrapper
    {
        protected BaseFileWrapper() : this(null) { }
        protected BaseFileWrapper(TheraMenu menu) => Menu = menu;

        public event Action FilePathChanged;
        protected void OnFilePathChanged() => FilePathChanged?.Invoke();

        public ITheraMenu Menu { get; set; }
        public virtual string FilePath { get; set; }

        public abstract void Edit();
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
    }
}
