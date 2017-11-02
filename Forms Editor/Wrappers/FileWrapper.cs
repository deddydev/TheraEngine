using System;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using TheraEngine.Files;
using Microsoft.VisualBasic.FileIO;
using TheraEditor.Windows.Forms;
using System.Diagnostics;

namespace TheraEditor.Wrappers
{
    public abstract class BaseFileWrapper : BaseWrapper
    {
        #region Menu
        private static ContextMenuStrip _menu;
        public BaseFileWrapper() : base(_menu)
        {
            ImageIndex = 0;
            SelectedImageIndex = 0;
        }
        static BaseFileWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.F2));                              //0
            _menu.Items.Add(new ToolStripMenuItem("&Open In Explorer", null, ExplorerAction, Keys.Control | Keys.O));   //1
            _menu.Items.Add(new ToolStripMenuItem("Edit File", null, EditAction, Keys.F1));                             //2
            _menu.Items.Add(new ToolStripSeparator());                                                                  //3
            _menu.Items.Add(new ToolStripMenuItem("&Export", null, ExportAction, Keys.Control | Keys.E));               //4
            _menu.Items.Add(new ToolStripMenuItem("&Replace", null, ReplaceAction, Keys.Control | Keys.R));             //5
            _menu.Items.Add(new ToolStripMenuItem("Re&load", null, RestoreAction, Keys.Control | Keys.L));              //6
            ToolStripMenuItem alwaysReload = new ToolStripMenuItem("Reload Automatically") { CheckOnClick = true };
            alwaysReload.CheckedChanged += AlwaysReload_CheckedChanged;
            _menu.Items.Add(alwaysReload); //7
            _menu.Items.Add(new ToolStripSeparator());                                                                  //8
            _menu.Items.Add(new ToolStripMenuItem("&Cut", null, CutAction, Keys.Control | Keys.X));                     //9
            _menu.Items.Add(new ToolStripMenuItem("&Copy", null, CopyAction, Keys.Control | Keys.C));                   //10
            _menu.Items.Add(new ToolStripMenuItem("&Paste", null, PasteAction, Keys.Control | Keys.V));                 //11
            _menu.Items.Add(new ToolStripMenuItem("&Delete", null, DeleteAction, Keys.Control | Keys.Delete));          //12
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;
        }

        private static void AlwaysReload_CheckedChanged(object sender, EventArgs e)
        {
            GetInstance<BaseFileWrapper>().AlwaysReload = ((ToolStripMenuItem)sender).Checked;
        }

        protected static void DeleteAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Delete();
        protected static void RenameAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Rename();
        protected static void CutAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Cut();
        protected static void CopyAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Copy();
        protected static void PasteAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Paste();

        protected static void ExportAction(object sender, EventArgs e) { GetInstance<BaseFileWrapper>().Export(); }
        protected static void ReplaceAction(object sender, EventArgs e) { GetInstance<BaseFileWrapper>().Replace(); }
        protected static void RestoreAction(object sender, EventArgs e) { GetInstance<BaseFileWrapper>().Restore(); }
        protected static void EditExternalAction(object sender, EventArgs e) { GetInstance<BaseFileWrapper>().OpenInExplorer(true); }
        protected static void ExplorerAction(object sender, EventArgs e) => GetInstance<BaseFileWrapper>().OpenInExplorer(false);
        protected static void EditAction(object sender, EventArgs e) => GetInstance<BaseFileWrapper>().EditResource();

        public void Export() { }
        public void Replace() { }
        public void Restore() { }

        public void OpenInExplorer(bool editFileExternally)
        {
            string path = FilePath;
            if (string.IsNullOrEmpty(path))
                return;
            if (editFileExternally)
                Process.Start("explorer.exe", path);
            else
            {
                string dir = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(dir))
                    return;
                Process.Start("explorer.exe", dir);
            }
        }

        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            //_menu.Items[1].Enabled = _menu.Items[2].Enabled = _menu.Items[4].Enabled = _menu.Items[5].Enabled = _menu.Items[8].Enabled = true;
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            BaseFileWrapper w = GetInstance<BaseFileWrapper>();
            //_menu.Items[0].Enabled = w.TreeView.LabelEdit;
            _menu.Items[1].Enabled = !string.IsNullOrEmpty(w.FilePath) && File.Exists(w.FilePath);
            _menu.Items[5].Enabled = _menu.Items[8].Enabled = w.Parent != null;
            _menu.Items[6].Enabled = w.IsLoaded && w.SingleInstance.EditorState.HasChanges;
            ((ToolStripMenuItem)_menu.Items[7]).Checked = w.AlwaysReload;
            //_menu.Items[2].Enabled = ((w._resource.IsDirty) || (w._resource.IsBranch));
            //_menu.Items[4].Enabled = w.PrevNode != null;
            //_menu.Items[5].Enabled = w.NextNode != null;
        }
        #endregion

        public abstract Type FileType { get; }
        public abstract bool IsLoaded { get; }
        public bool AlwaysReload { get; set; } = false;
        public bool ExternallyModified { get; set; } = false;
        public abstract FileObject SingleInstance { get; set; }
        public abstract FileObject GetNewInstance();
        
        public void Reload()
        {

        }

        public void EditResource()
        {
            Editor.Instance.PropertyGridForm.theraPropertyGrid1.TargetObject = SingleInstance;
        }

        //public BaseFileWrapper(ContextMenuStrip menu) : base(menu)
        //{

        //}

        internal protected override void OnExpand()
        {

        }
        protected internal override void OnCollapse()
        {

        }
        public override void Delete()
        {
            if (Parent == null)
                return;

            ResourceTree tree = TreeView;
            try
            {
                tree.WatchProjectDirectory = false;
                if (new FileInfo(FilePath).Length > 0)
                    FileSystem.DeleteFile(FilePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
                else
                    File.Delete(FilePath);
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
    }
    public class UnidentifiedFileWrapper : BaseFileWrapper
    {
        public UnidentifiedFileWrapper() : base()
        {

        }
        
        public override bool IsLoaded => false;

        public override FileObject SingleInstance
        {
            get => null;
            set
            {

            }
        }
        public override string FilePath
        {
            get => Name;
            set => Name = value;
        }

        public override Type FileType => null;

        public override FileObject GetNewInstance()
        {
            return null;
        }

        protected internal override void FixPath(string parentFolderPath)
        {
        }
    }
    public class FileWrapper<T> : BaseFileWrapper where T : FileObject
    {
        public override Type FileType => typeof(T);

        protected SingleFileRef<T> _fileRef = new SingleFileRef<T>();

        public SingleFileRef<T> Resource => _fileRef;
        public override bool IsLoaded => Resource.IsLoaded;
        public override string FilePath
        {
            get => Resource.ReferencePath;
            set
            {
                Resource.ReferencePath = value;
                Name = value;
            }
        }
        public override FileObject GetNewInstance()
            => _fileRef.LoadNewInstance();
        public override FileObject SingleInstance
        {
            get => Resource.File;
            set
            {
                T obj = value as T;
                Resource.File = obj;
                Name = FilePath;
            }
        }

        protected internal override void FixPath(string parentFolderPath)
        {
            string fileName = Text;
            if (!parentFolderPath.EndsWith("\\"))
                parentFolderPath += "\\";
            FilePath = parentFolderPath + fileName;
        }
    }
}
