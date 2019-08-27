using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine.Core.Files;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    public class FileEditorWrapperBase<TFile, TEditor> : FileWrapper<TFile> 
        where TFile : class, IFileObject 
        where TEditor : DockableFileEditor<TFile>, new()
    {
        public FileEditorWrapperBase() : base() { }
        
        public TEditor Editor { get; private set; }
        public override async void EditResource()
        {
            if (Editor is null)
            {
                Editor = new TEditor();
                Editor.FormClosed += Editor_FormClosed;
                Editor.Show(Windows.Forms.Editor.Instance.DockPanel, DockState.Document);
            }
            TFile file = await ResourceRef.GetInstanceAsync();
            Editor.File = file;
            Editor.Focus();
        }
        private void Editor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Editor.FormClosed -= Editor_FormClosed;
            Editor = null;
        }
    }
}
