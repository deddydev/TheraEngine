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
    public abstract class DockableRenderableFileEditor : DockableFileEditor, IEditorRenderableControl
    {
        public virtual ELocalPlayerIndex PlayerIndex { get; } = ELocalPlayerIndex.One;
        public BaseGameMode GameMode { get; protected set; }
        BaseRenderPanel IEditorRenderableControl.RenderPanel => RenderPanelGeneric as BaseRenderPanel;

        public abstract IPawn EditorPawn { get; }
        public abstract World World { get; }

        protected abstract IUIRenderPanel RenderPanelGeneric { get; }

        protected void RenderPanel_MouseEnter(object sender, EventArgs e) => Cursor.Hide();
        protected void RenderPanel_MouseLeave(object sender, EventArgs e) => Cursor.Show();
        protected void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            Editor.SetActiveEditorControl(this);
            if (File != null)
                Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = File;
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (Editor.ActiveRenderForm == this)
                Editor.SetActiveEditorControl(null);
            base.OnHandleDestroyed(e);
        }
        protected override void OnShown(EventArgs e)
        {
            RenderPanelGeneric.FormShown();
            base.OnShown(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            RenderPanelGeneric.FormClosed();
            base.OnClosing(e);
        }
    }
}
