using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine.Core.Files;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    public class FileEditorWrapperBase<TFile, TEditor> : FileWrapper<TFile> 
        where TFile : class, IFileObject 
        where TEditor : class, IDockableFileEditorControl<TFile>, new()
    {
        public FileEditorWrapperBase() : base() { }
        
        public TEditor FileEditor { get; private set; }
        public override async void Edit()
        {
            if (FileEditor is null)
            {
                FileEditor = Editor.Instance.DockEditor<TEditor, TFile>(DockState.Document);
                FileEditor.FormClosed += Editor_FormClosed;
            }
            TFile file = await FileRef.GetInstanceAsync();
            FileEditor.File = file;
            //FileEditor.Focus();
        }
        private void Editor_FormClosed(object sender, FormClosedEventArgs e)
        {
            //FileEditor.FormClosed -= Editor_FormClosed;
            FileEditor = null;
        }
    }
}
