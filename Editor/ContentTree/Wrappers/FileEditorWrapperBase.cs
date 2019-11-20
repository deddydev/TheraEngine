﻿using System.Windows.Forms;
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
        
        public TEditor FileEditor { get; private set; }
        public override async void Edit()
        {
            if (FileEditor is null)
            {
                FileEditor = new TEditor();
                FileEditor.FormClosed += Editor_FormClosed;
                FileEditor.Show(Editor.Instance.DockPanel, DockState.Document);
            }
            TFile file = await FileRef.GetInstanceAsync();
            FileEditor.File = file;
            FileEditor.Focus();
        }
        private void Editor_FormClosed(object sender, FormClosedEventArgs e)
        {
            FileEditor.FormClosed -= Editor_FormClosed;
            FileEditor = null;
        }
    }
}
