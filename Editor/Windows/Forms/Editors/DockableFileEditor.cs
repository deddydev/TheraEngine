using Extensions;
using System;
using System.Threading;
using System.Windows.Forms;
using TheraEngine.Core.Files;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public abstract class DockableFileEditor<T> : DockContent, IFileEditorControl where T : class, IFileObject
    {
        private T _file;
        public virtual T File
        {
            get => _file;
            set
            {
                if (!AllowFileClose() || _file == value)
                    return;

                _file = value;
            }
        }
        IFileObject IFileEditorControl.File => File;

        protected void BtnSave_Click(object sender, EventArgs e) => Save();
        protected void BtnSaveAs_Click(object sender, EventArgs e) => SaveAs();
        protected void BtnClose_Click(object sender, EventArgs e) => Close();
        
        public async void Save()
        {
            if (File == null)
                return;

            string path = File.FilePath;
            if (string.IsNullOrWhiteSpace(path))
                SaveAs();
            else
            {
                int op = Editor.Instance.BeginOperation($"Saving {path}...", $"Successfully saved {path}.", out Progress<float> progress, out CancellationTokenSource cancel);
                await File.RootFile.ExportAsync(ESerializeFlags.Default, progress, cancel.Token);
                Editor.Instance.EndOperation(op);
            }
        }
        public async void SaveAs()
        {
            if (File is null)
                return;

            string filter = File.GetFilter();
            bool ok;
            string filePath;

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = filter })
            {
                ok = sfd.ShowDialog(this) == DialogResult.OK;
                filePath = sfd.FileName;
            }

            if (ok)
                await File.ExportAsync(filePath, ESerializeFlags.Default);
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Editor.OpenEditors.Add(this);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            bool isAllowed = AllowFileClose();
            e.Cancel = !isAllowed;
            if (isAllowed)
                Editor.OpenEditors.Remove(this);
        }
        public bool AllowFileClose()
        {
            if (_file == null || !_file.HasEditorState || !_file.EditorState.IsDirty)
                return true;

            string path = 
                string.IsNullOrWhiteSpace(_file?.FilePath) || 
                string.IsNullOrWhiteSpace(Editor.Instance.Project?.DirectoryPath) ? 
                _file?.ToString() ?? "<null>" : 
                _file.FilePath.MakeAbsolutePathRelativeTo(Editor.Instance.Project.DirectoryPath);

            DialogResult result = MessageBox.Show(this, $"Do you want to save your work to {path} before closing?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button3);
            if (result == DialogResult.Cancel)
                return false;
            else if (result == DialogResult.Yes)
                Save();

            return true;
        }
    }
}
