using System;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using TheraEngine.Files;
using Microsoft.VisualBasic.FileIO;

namespace TheraEditor.Wrappers
{
    public class FileWrapper : BaseWrapper
    {
        #region Menu
        private static ContextMenuStrip _menu;
        public FileWrapper() : base(_menu) { }
        static FileWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Re&name", null, RenameAction, Keys.Control | Keys.N));
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add(new ToolStripMenuItem("&Export", null, ExportAction, Keys.Control | Keys.E));
            _menu.Items.Add(new ToolStripMenuItem("&Replace", null, ReplaceAction, Keys.Control | Keys.R));
            _menu.Items.Add(new ToolStripMenuItem("Re&load", null, RestoreAction, Keys.Control | Keys.L));
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add(new ToolStripMenuItem("&Delete", null, DeleteAction, Keys.Control | Keys.Delete));
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;
        }
        protected static void ExportAction(object sender, EventArgs e) { GetInstance<FileWrapper>().Export(); }
        protected static void ReplaceAction(object sender, EventArgs e) { GetInstance<FileWrapper>().Replace(); }
        protected static void RestoreAction(object sender, EventArgs e) { GetInstance<FileWrapper>().Restore(); }
        protected static void DeleteAction(object sender, EventArgs e) { GetInstance<FileWrapper>().Delete(); }
        protected static void RenameAction(object sender, EventArgs e) { GetInstance<FileWrapper>().Rename(); }
        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            //_menu.Items[1].Enabled = _menu.Items[2].Enabled = _menu.Items[4].Enabled = _menu.Items[5].Enabled = _menu.Items[8].Enabled = true;
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            FileWrapper w = GetInstance<FileWrapper>();
            //_menu.Items[1].Enabled = _menu.Items[8].Enabled = w.Parent != null;
            //_menu.Items[2].Enabled = ((w._resource.IsDirty) || (w._resource.IsBranch));
            //_menu.Items[4].Enabled = w.PrevNode != null;
            //_menu.Items[5].Enabled = w.NextNode != null;
        }
        #endregion

        protected SingleFileRef<FileObject> _fileRef = new SingleFileRef<FileObject>();

        public override string FilePath
        {
            get => _fileRef.ReferencePath;
            internal set => _fileRef.ReferencePath = value;
        }

        public SingleFileRef<FileObject> Resource => _fileRef;
        public bool IsLoaded => _fileRef.IsLoaded;
        public bool AlwaysReload { get; internal set; } = false;
        public bool ExternallyModified { get; internal set; } = false;

        internal void Reload()
        {

        }

        public void EditResource()
        {

        }
        
        public virtual string Export()
        {
            string outPath = "";
            //int index = Program.SaveFile(ExportFilter, Text, out outPath);
            //if (index != 0)
            //{
            //    if (Parent == null)
            //        _resource.Merge(Control.ModifierKeys == (Keys.Control | Keys.Shift));
            //    OnExport(outPath, index);
            //}
            return outPath;
        }
        public virtual void OnExport(string outPath, int filterIndex)
        {
            //_resource.Export(outPath);
        }

        public virtual void Replace()
        {
            if (Parent == null)
                return;

            //string inPath;
            //int index = Program.OpenFile(ReplaceFilter, out inPath);
            //if (index != 0)
            //{
            //    OnReplace(inPath, index);
            //    this.Link(_resource);
            //}
        }

        public virtual void OnReplace(string inStream, int filterIndex)
        {
            //_resource.Replace(inStream);
        }

        public void Restore()
        {
            //_resource.Restore();
        }

        public void Delete()
        {
            if (Parent == null)
                return;
            
            ResourceTree tree = TreeView;
            try
            {
                tree.WatchProjectDirectory = false;
                FileSystem.DeleteFile(FilePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
                Remove();
            }
            catch (OperationCanceledException e)
            {

            }
            finally
            {
                tree.WatchProjectDirectory = true;
            }
        }

        public void Rename()
        {
            TreeView.LabelEdit = true;
            BeginEdit();
        }
    }
}
