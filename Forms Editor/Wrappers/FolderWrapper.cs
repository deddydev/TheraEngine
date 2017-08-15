using System.Windows.Forms;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Files;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using TheraEditor.Windows.Forms;

namespace TheraEditor.Wrappers
{
    public class FolderWrapper : BaseWrapper
    {
        #region Menu
        private static ContextMenuStrip _menu;
        public FolderWrapper() : base(_menu)
        {
            ImageIndex = 1;
            SelectedImageIndex = 2;
        }
        static FolderWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.F2));                              //0
            _menu.Items.Add(new ToolStripMenuItem("&Open In Explorer", null, ExplorerAction, Keys.Control | Keys.E));   //1
            _menu.Items.Add(new ToolStripSeparator());                                                                  //2
            _menu.Items.Add(new ToolStripMenuItem("&Import File", null, null, Keys.Control | Keys.I));                  //3
            _menu.Items.Add(new ToolStripMenuItem("&New File", null, null, Keys.Control | Keys.N));                     //4
            _menu.Items.Add(new ToolStripMenuItem("New &Folder", null, FolderAction, Keys.Control | Keys.F));           //5
            _menu.Items.Add(new ToolStripSeparator());                                                                  //6
            _menu.Items.Add(new ToolStripMenuItem("Compile To &Archive", null, ArchiveAction, Keys.Control | Keys.A));  //7
            _menu.Items.Add(new ToolStripSeparator());                                                                  //8
            _menu.Items.Add(new ToolStripMenuItem("&Cut", null, CutAction, Keys.Control | Keys.X));                     //9
            _menu.Items.Add(new ToolStripMenuItem("&Copy", null, CopyAction, Keys.Control | Keys.C));                   //10
            _menu.Items.Add(new ToolStripMenuItem("&Paste", null, PasteAction, Keys.Control | Keys.V));                 //11
            _menu.Items.Add(new ToolStripMenuItem("&Delete", null, DeleteAction, Keys.Control | Keys.Delete));          //12
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;

            LoadFileTypes();
        }

        protected static void FolderAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().NewFolder();
        protected static void RenameAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().Rename();
        protected static void DeleteAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().Delete();
        protected static void CutAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().Cut();
        protected static void CopyAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().Copy();
        protected static void PasteAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().Paste();
        protected static void ArchiveAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().ToArchive();
        protected static void ExplorerAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().OpenInExplorer();

        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            //_menu.Items[7].Enabled = _menu.Items[12].Enabled = true;
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            FolderWrapper w = GetInstance<FolderWrapper>();
            _menu.Items[0].Enabled = !w.TreeView.LabelEdit;
            _menu.Items[1].Enabled = !string.IsNullOrEmpty(w.FilePath) && Directory.Exists(w.FilePath);

            bool paste = false;
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                MemoryStream stream = (MemoryStream)data.GetData("Preferred DropEffect", true);
                int flag = stream.ReadByte();
                paste = flag == 2 || flag == 5;
            }
            _menu.Items[11].Enabled = paste;
            _menu.Items[9].Enabled = _menu.Items[12].Enabled = w.Parent != null;
        }
        #endregion

        private string _path;

        public override string FilePath
        {
            get => _path;
            set => _path = value;
        }
        
        public void ToArchive()
        {

        }

        public void NewFolder()
        {
            string path = FilePath + "\\NewFolder";
            TreeView.WatchProjectDirectory = false;
            Directory.CreateDirectory(path);
            TreeView.WatchProjectDirectory = true;
            FolderWrapper b = Wrap(path) as FolderWrapper;
            Nodes.Add(b);
            TreeView.SelectedNode = b;
            b.EnsureVisible();
            b.Rename();
        }

        public void Delete()
        {
            if (Parent == null)
                return;

            ResourceTree tree = TreeView;
            try
            {
                tree.WatchProjectDirectory = false;
                if (Directory.GetDirectories(FilePath).Length > 0 || Directory.GetFiles(FilePath).Length > 0)
                    FileSystem.DeleteDirectory(FilePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
                else
                    Directory.Delete(FilePath);
                Remove();
                ContextMenuStrip.Close();
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

        #region File Type Loading
        private static void LoadFileTypes()
        {
            var fileObjectTypes =
                from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetExportedTypes()
                where assemblyType.IsSubclassOf(typeof(FileObject)) && !assemblyType.IsAbstract
                select assemblyType;
            Dictionary<string, NamespaceNode> nodeCache = new Dictionary<string, NamespaceNode>();
            foreach (Type t in fileObjectTypes)
            {
                string path = t.Namespace;
                int dotIndex = path.IndexOf(".");
                string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                if (nodeCache.ContainsKey(name))
                    nodeCache[name].Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, t);
                else
                {
                    NamespaceNode node = new NamespaceNode(name);
                    nodeCache.Add(name, node);
                    ((ToolStripDropDownItem)_menu.Items[3]).DropDownItems.Add(node.Button);
                }
            }
        }

        private class NamespaceNode
        {
            public NamespaceNode(string name)
            {
                _name = name;
                _children = new Dictionary<string, NamespaceNode>();
                Button = new ToolStripDropDownButton(_name) { ShowDropDownArrow = true };
            }

            string _name;
            Dictionary<string, NamespaceNode> _children;
            ToolStripDropDownButton _button;

            public string Name { get => _name; set => _name = value; }
            private Dictionary<string, NamespaceNode> Children { get => _children; set => _children = value; }
            public ToolStripDropDownButton Button { get => _button; set => _button = value; }

            public void Add(string path, Type t)
            {
                if (string.IsNullOrEmpty(path))
                {
                    ToolStripDropDownButton btn = new ToolStripDropDownButton(t.Name)
                    {
                        ShowDropDownArrow = false,
                        Tag = t,
                    };
                    btn.Click += OnNewClick;
                    _button.DropDownItems.Add(btn);
                    return;
                }
                int dotIndex = path.IndexOf(".");
                string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                if (_children.ContainsKey(name))
                    _children[name].Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, t);
                else
                {
                    NamespaceNode node = new NamespaceNode(name);
                    _children.Add(name, node);
                    Button.DropDownItems.Add(node.Button);
                }
            }
        }
        private static void OnNewClick(object sender, EventArgs e)
        {
            if (sender is ToolStripDropDownButton button)
            {
                Type fileType = button.Tag as Type;
                FileObject file = (FileObject)Activator.CreateInstance(fileType);
                ContextMenuStrip ctx = button.GetContextMenuStrip();
                FolderWrapper b = ctx.Tag as FolderWrapper;
                string dir = b.FilePath as string;
                b.TreeView.WatchProjectDirectory = false;
                file.Export(dir, file.Name, FileFormat.XML);
                b.TreeView.WatchProjectDirectory = true;
                b.Nodes.Add(Wrap(file) as BaseWrapper);
            }
        }
        #endregion
    }
}
