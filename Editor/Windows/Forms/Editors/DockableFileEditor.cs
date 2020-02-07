using Extensions;
using System;
using System.Runtime.Remoting;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    //public interface IDockableFileEditorControl<T> : IFileEditorControl<T> where T : class, IFileObject
    //{
    //    event FormClosedEventHandler FormClosed;

    //    bool Focus();

    //    bool CloseButton { get; set; }
    //    bool CloseButtonVisible { get; set; }
    //    //DockPanel DockPanel { get; set; }
    //    DockState DockState { get; set; }
    //    //DockPane Pane { get; set; }
    //    bool IsHidden { get; set; }
    //    DockState VisibleState { get; set; }
    //    bool IsFloat { get; set; }
    //    //DockPane PanelPane { get; set; }
    //    //DockPane FloatPane { get; set; }
    //    bool HideOnClose { get; set; }
    //    DockState ShowHint { get; set; }
    //    bool IsActivated { get; }
    //    //ContextMenu TabPageContextMenu { get; set; }
    //    //ContextMenuStrip TabPageContextMenuStrip { get; set; }
    //    string TabText { get; set; }
    //    double AutoHidePortion { get; set; }
    //    DockAreas DockAreas { get; set; }
    //    bool AllowEndUserDocking { get; set; }
    //    //DockContentHandler DockHandler { get; }
    //    string ToolTipText { get; set; }

    //    event EventHandler DockStateChanged;

    //    void Activate();
    //    void DockTo(DockPanel panel, DockStyle dockStyle);
    //    void DockTo(DockPane paneTo, DockStyle dockStyle, int contentIndex);
    //    void FloatAt(Rectangle floatWindowBounds);
    //    void Hide();
    //    bool IsDockStateValid(DockState dockState);
    //    void Show(DockPane previousPane, DockAlignment alignment, double proportion);
    //    void Show(DockPane pane, IDockContent beforeContent);
    //    void Show(DockPanel dockPanel, DockState dockState);
    //    void Show(DockPanel dockPanel);
    //    void Show();
    //    void Show(DockPanel dockPanel, Rectangle floatWindowBounds);
    //}
    public abstract class DockableFileEditor<T> : 
        DockContent, IDockableFileEditorControl<T>, IFileEditorControl
        where T : class, IFileObject
    {
        public event Action FormClosedEvent;

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

            if (RemotingServices.IsTransparentProxy(_file))
            {
                Engine.Instance.DomainProxyDestroying -= Instance_DomainProxyPreUnset;
            }
            if (RemotingServices.IsTransparentProxy(file))
            {
                Engine.Instance.DomainProxyDestroying += Instance_DomainProxyPreUnset;
            }

            _file = file;
            return true;
        }

        private void Instance_DomainProxyPreUnset(TheraEngine.Core.EngineDomainProxy obj)
        {
            _file = null;
            Engine.Instance.DomainProxyDestroying -= Instance_DomainProxyPreUnset;
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

            string filter = File.CreateFilter();
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
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            FormClosedEvent?.Invoke();
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
        public void InvokeFocus()
        {
            Invoke((Func<bool>)Focus);
        }
    }
}
