using Microsoft.VisualBasic.FileIO;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    public class FileTreeNode : ContentTreeNode
    {
        public IBaseFileWrapper Wrapper { get; set; }

        #region Menu

        static FileTreeNode()
        {
            _defaultMenu = new ContextMenuStrip();
            FillContextMenuDefaults(_defaultMenu);
        }

        private static ContextMenuStrip _defaultMenu;
        public static int FillContextMenuDefaults(ContextMenuStrip strip)
        {
            strip.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.F2));                              //0
            strip.Items.Add(new ToolStripMenuItem("&Open In Explorer", null, ExplorerAction, Keys.Control | Keys.O));   //1
            strip.Items.Add(new ToolStripMenuItem("Edit", null, EditAction, Keys.F1));                                  //2
            strip.Items.Add(new ToolStripMenuItem("Edit Raw", null, EditRawAction, Keys.F3));                           //3
            strip.Items.Add(new ToolStripSeparator());                                                                  //4
            strip.Items.Add(new ToolStripMenuItem("&Export", null, ExportAction, Keys.Control | Keys.E));               //5
            strip.Items.Add(new ToolStripMenuItem("&Replace", null, ReplaceAction, Keys.Control | Keys.R));             //6
            strip.Items.Add(new ToolStripMenuItem("Re&load", null, RestoreAction, Keys.Control | Keys.L));              //7
            ToolStripMenuItem alwaysReload = new ToolStripMenuItem("Reload Automatically") { CheckOnClick = true };
            alwaysReload.CheckedChanged += AlwaysReload_CheckedChanged;
            strip.Items.Add(alwaysReload);                                                                              //8
            strip.Items.Add(new ToolStripSeparator());                                                                  //9
            strip.Items.Add(new ToolStripMenuItem("&Cut", null, CutAction, Keys.Control | Keys.X));                     //10
            strip.Items.Add(new ToolStripMenuItem("&Copy", null, CopyAction, Keys.Control | Keys.C));                   //11
            strip.Items.Add(new ToolStripMenuItem("&Paste", null, PasteAction, Keys.Control | Keys.V));                 //12
            strip.Items.Add(new ToolStripMenuItem("&Delete", null, DeleteAction, Keys.Control | Keys.Delete));          //13
            strip.Opening += MenuOpening;
            strip.Closing += MenuClosing;
            return strip.Items.Count;
        }
        private static void AlwaysReload_CheckedChanged(object sender, EventArgs e)
        {
            //GetInstance<FileTreeNode>().AlwaysReload = ((ToolStripMenuItem)sender).Checked;
        }

        protected static void DeleteAction(object sender, EventArgs e) => GetInstance<ContentTreeNode>().Delete();
        protected static void RenameAction(object sender, EventArgs e) => GetInstance<ContentTreeNode>().Rename();
        protected static void CutAction(object sender, EventArgs e) => GetInstance<ContentTreeNode>().Cut();
        protected static void CopyAction(object sender, EventArgs e) => GetInstance<ContentTreeNode>().Copy();
        protected static void PasteAction(object sender, EventArgs e) => GetInstance<ContentTreeNode>().Paste();

        protected static void ExportAction(object sender, EventArgs e) { GetInstance<FileTreeNode>().Export(); }
        protected static void ReplaceAction(object sender, EventArgs e) { GetInstance<FileTreeNode>().Replace(); }
        protected static void RestoreAction(object sender, EventArgs e) { GetInstance<FileTreeNode>().Restore(); }
        protected static void EditExternalAction(object sender, EventArgs e) { GetInstance<FileTreeNode>().OpenInExplorer(true); }
        protected static void ExplorerAction(object sender, EventArgs e) => GetInstance<FileTreeNode>().OpenInExplorer(false);
        protected static void EditAction(object sender, EventArgs e) => GetInstance<FileTreeNode>().Edit();
        protected static void EditRawAction(object sender, EventArgs e) => GetInstance<FileTreeNode>().EditRaw();
        
        public void Export() { }
        public void Replace() { }
        public void Restore() { }
        public void Edit() { }
        public void EditRaw() { }

        public void OpenInExplorer(bool editFileExternally)
        {
            string path = FilePath;
            if (string.IsNullOrEmpty(path))
                return;
            if (editFileExternally)
                Process.Start(path);
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
            FileTreeNode w = GetInstance<FileTreeNode>();
            //_menu.Items[0].Enabled = w.TreeView.LabelEdit;
            _defaultMenu.Items[1].Enabled = !string.IsNullOrEmpty(w.FilePath) && File.Exists(w.FilePath);
            _defaultMenu.Items[5].Enabled = _defaultMenu.Items[8].Enabled = w.Parent != null;

            //_defaultMenu.Items[6].Enabled = w.IsLoaded && w.SingleInstance.EditorState.HasChanges;
            //((ToolStripMenuItem)_defaultMenu.Items[7]).Checked = w.AlwaysReload;

            //_menu.Items[2].Enabled = ((w._resource.IsDirty) || (w._resource.IsBranch));
            //_menu.Items[4].Enabled = w.PrevNode != null;
            //_menu.Items[5].Enabled = w.NextNode != null;
        }
        #endregion

        public FileTreeNode() : this(_defaultMenu) { }
        public FileTreeNode(ContextMenuStrip menu) : base(menu)
        {
            //var nodeWrappers = GetType().GetCustomAttributesExt<NodeWrapperAttribute>();
            //if (nodeWrappers.Length > 0)
            //{
            //    ImageIndex = ResourceTree.Images.Images.IndexOfKey(nodeWrappers[0].ImageName);
            //    SelectedImageIndex = ResourceTree.Images.Images.IndexOfKey(nodeWrappers[0].SelectedImageName);
            //}
            //else
            //    ImageIndex = SelectedImageIndex = 0;

            Engine.Instance.DomainProxyPostSet += Instance_DomainProxyPostSet;
            Engine.Instance.DomainProxyPreUnset += Instance_DomainProxyPreUnset;
        }
        public FileTreeNode(IBaseFileWrapper wrapper) : base(wrapper.Menu)
        {
            Wrapper = wrapper;

            Engine.Instance.DomainProxyPostSet += Instance_DomainProxyPostSet;
            Engine.Instance.DomainProxyPreUnset += Instance_DomainProxyPreUnset;
        }
        public FileTreeNode(string path) : base(_defaultMenu)
        {
            FilePath = path;

            Engine.Instance.DomainProxyPostSet += Instance_DomainProxyPostSet;
            Engine.Instance.DomainProxyPreUnset += Instance_DomainProxyPreUnset;
        }

        public override string FilePath
        {
            get => base.FilePath;
            set
            {
                base.FilePath = value;
                if (Wrapper != null)
                    Wrapper.FilePath = value;
            }
        }

        private void Instance_DomainProxyPreUnset(EngineDomainProxy obj)
        {

        }
        private void Instance_DomainProxyPostSet(EngineDomainProxy obj)
        {

        }

        public override void Delete()
        {
            if (Parent is null)
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
            catch
            {

            }
            finally
            {
                tree.WatchProjectDirectory = true;
            }
        }

        protected internal override void SetPath(string parentFolderPath)
        {
            string fileName = Text;
            if (parentFolderPath[parentFolderPath.Length - 1] != Path.DirectorySeparatorChar)
                parentFolderPath += Path.DirectorySeparatorChar;
            FilePath = parentFolderPath + fileName;
        }

        internal protected override void OnExpand() { }
        internal protected override void OnCollapse() { }
    }
}
