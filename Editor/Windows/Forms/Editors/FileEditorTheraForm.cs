using Extensions;
using System;
using System.Windows.Forms;
using TheraEngine.Core.Files;

namespace TheraEditor.Windows.Forms
{
    public abstract class FileEditorTheraForm<T> : TheraForm, IFileEditorControl where T : class, IFileObject
    {
        private T _file;
        public T File
        {
            get => _file;
            set => TrySetFile(value);
        }
        protected virtual bool TrySetFile(T file)
        {
            if (!AllowFileClose() || _file == file)
                return false;

            _file = file;
            return true;
        }
        IFileObject IFileEditorControl.File => File;

        protected void BtnSave_Click(object sender, EventArgs e) => Save();
        protected void BtnSaveAs_Click(object sender, EventArgs e) => SaveAs();
        protected void BtnClose_Click(object sender, EventArgs e) => Close();
        
        public async void Save()
        {
            if (File is null)
                return;

            string path = File.FilePath;
            if (string.IsNullOrWhiteSpace(path))
                SaveAs();
            else
            {
                await Editor.RunOperationAsync(
                    $"Saving {path}...", 
                    $"Successfully saved {path}.",
                    async (p, c) => await File.RootFile.ExportAsync(ESerializeFlags.Default, p, c.Token));
            }
        }
        public async void SaveAs()
        {
            if (File is null)
                return;

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = File.GetFilter() })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                    await File.ExportAsync(sfd.FileName, ESerializeFlags.Default);
            }
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Editor.OpenEditors.Add(this);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            bool allow = AllowFileClose();
            e.Cancel = !allow;
            if (allow)
                Editor.OpenEditors.Remove(this);
        }
        public bool AllowFileClose()
        {
            if (_file is null || !_file.HasEditorState || !_file.EditorState.IsDirty)
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
