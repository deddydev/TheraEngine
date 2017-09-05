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
        public abstract bool IsLoaded { get; }
        public bool AlwaysReload { get; set; } = false;
        public bool ExternallyModified { get; set; } = false;
        public abstract FileObject FileObject { get; set; }
        public abstract FileObject GetNewInstance();
        
        public void Reload()
        {

        }

        public void EditResource()
        {
            Editor.Instance.PropForm.PropertyGrid.SelectedObject = GetNewInstance();
        }

        public BaseFileWrapper(ContextMenuStrip menu) : base(menu)
        {

        }

        internal protected override void OnExpand()
        {

        }
    }
    public class FileWrapper<T> : BaseFileWrapper where T : FileObject
    {
        #region Menu
        private static ContextMenuStrip _menu;
        public FileWrapper() : base(_menu)
        {
            ImageIndex = 0;
            SelectedImageIndex = 0;
        }
        static FileWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.F2));                              //0
            _menu.Items.Add(new ToolStripMenuItem("&Open In Explorer", null, ExplorerAction, Keys.Control | Keys.O));   //1
            _menu.Items.Add(new ToolStripMenuItem("Edit File", null, EditAction, Keys.F1));                             //2
            _menu.Items.Add(new ToolStripSeparator());                                                                  //3
            _menu.Items.Add(new ToolStripMenuItem("&Export", null, ExportAction, Keys.Control | Keys.E));               //4
            _menu.Items.Add(new ToolStripMenuItem("&Replace", null, ReplaceAction, Keys.Control | Keys.R));             //5
            _menu.Items.Add(new ToolStripMenuItem("Re&load", null, RestoreAction, Keys.Control | Keys.L));              //6
            _menu.Items.Add(new ToolStripSeparator());                                                                  //7
            _menu.Items.Add(new ToolStripMenuItem("&Cut", null, CutAction, Keys.Control | Keys.X));                     //8
            _menu.Items.Add(new ToolStripMenuItem("&Copy", null, CopyAction, Keys.Control | Keys.C));                   //9
            _menu.Items.Add(new ToolStripMenuItem("&Paste", null, PasteAction, Keys.Control | Keys.V));                 //10
            _menu.Items.Add(new ToolStripMenuItem("&Delete", null, DeleteAction, Keys.Control | Keys.Delete));          //11
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;
        }
        protected static void ExportAction(object sender, EventArgs e) { GetInstance<FileWrapper<T>>().Export(); }
        protected static void ReplaceAction(object sender, EventArgs e) { GetInstance<FileWrapper<T>>().Replace(); }
        protected static void RestoreAction(object sender, EventArgs e) { GetInstance<FileWrapper<T>>().Restore(); }
        protected static void DeleteAction(object sender, EventArgs e) { GetInstance<FileWrapper<T>>().Delete(); }
        protected static void RenameAction(object sender, EventArgs e) { GetInstance<FileWrapper<T>>().Rename(); }
        protected static void EditExternalAction(object sender, EventArgs e) { GetInstance<FileWrapper<T>>().OpenInExplorer(true); }
        protected static void ExplorerAction(object sender, EventArgs e) => GetInstance<FileWrapper<T>>().OpenInExplorer(false);
        protected static void EditAction(object sender, EventArgs e) => GetInstance<FileWrapper<T>>().EditResource();
        protected static void CutAction(object sender, EventArgs e) => GetInstance<FileWrapper<T>>().Cut();
        protected static void CopyAction(object sender, EventArgs e) => GetInstance<FileWrapper<T>>().Copy();
        protected static void PasteAction(object sender, EventArgs e) => GetInstance<FileWrapper<T>>().Paste();
        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            //_menu.Items[1].Enabled = _menu.Items[2].Enabled = _menu.Items[4].Enabled = _menu.Items[5].Enabled = _menu.Items[8].Enabled = true;
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            FileWrapper<T> w = GetInstance<FileWrapper<T>>();
            _menu.Items[0].Enabled = w.TreeView.LabelEdit;
            _menu.Items[1].Enabled = !string.IsNullOrEmpty(w.FilePath) && File.Exists(w.FilePath);
            _menu.Items[5].Enabled = _menu.Items[8].Enabled = w.Parent != null;
            _menu.Items[6].Enabled = w.Resource.IsLoaded && w.Resource.File.EditorState.HasChanges;
            //_menu.Items[2].Enabled = ((w._resource.IsDirty) || (w._resource.IsBranch));
            //_menu.Items[4].Enabled = w.PrevNode != null;
            //_menu.Items[5].Enabled = w.NextNode != null;
        }
        #endregion

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
        {
            return _fileRef.LoadNewInstance();
        }
        public override FileObject FileObject
        {
            get => Resource.File;
            set
            {
                T obj = value as T;
                Resource.File = obj;
                Name = FilePath;
            }
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
        protected internal override void FixPath(string parentFolderPath)
        {
            string fileName = Text;
            if (!parentFolderPath.EndsWith("\\"))
                parentFolderPath += "\\";
            FilePath = parentFolderPath + fileName;
        }
    }
}
