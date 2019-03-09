using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Core.Files;
using TheraEngine.GameModes;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public abstract class DockableFileEditor : DockContent, IEditorControl
    {
        protected abstract TFileObject File { get; }

        protected void btnSave_Click(object sender, EventArgs e) => Save();
        protected void btnSaveAs_Click(object sender, EventArgs e) => SaveAs();
        protected void btnClose_Click(object sender, EventArgs e) => Close();

        public async void Save()
        {
            if (File == null)
                return;
            if (string.IsNullOrWhiteSpace(File.FilePath))
                SaveAs();
            else
            {
                int op = Editor.Instance.BeginOperation("Saving file...", out Progress<float> progress, out CancellationTokenSource cancel);
                await File.RootFile.ExportAsync(ESerializeFlags.Default, progress, cancel.Token);
                Editor.Instance.EndOperation(op);
            }
        }
        public async void SaveAs()
        {
            if (File == null)
                return;
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = File.GetFilter() })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                    await File.ExportAsync(sfd.FileName, ESerializeFlags.Default);
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (File == null || !File.EditorState.IsDirty)
                return;

            DialogResult result = MessageBox.Show(this, "Do you want to save your work before closing?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button3);
            if (result == DialogResult.Cancel)
                e.Cancel = true;
            else if (result == DialogResult.Yes)
                Save();
        }
    }
}
