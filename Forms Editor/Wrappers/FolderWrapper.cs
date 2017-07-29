using System.Windows.Forms;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Files;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;

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
            _menu.Items.Add(new ToolStripMenuItem("Re&name", null, RenameAction, Keys.Control | Keys.N)); //0
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add(new ToolStripMenuItem("&Import File", null, null, Keys.Control | Keys.I)); //2
            _menu.Items.Add(new ToolStripMenuItem("New &File", null, null, Keys.Control | Keys.F)); //3
            _menu.Items.Add(new ToolStripMenuItem("New F&older", null, FolderAction, Keys.Control | Keys.O)); //4
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add(new ToolStripMenuItem("Compile To &Archive", null, ArchiveAction, Keys.Control | Keys.A));  //6
            _menu.Items.Add(new ToolStripMenuItem("Open In &Explorer", null, ExplorerAction, Keys.Control | Keys.E)); //7
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add(new ToolStripMenuItem("&Delete", null, DeleteAction, Keys.Control | Keys.Delete)); //9
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;

            LoadFileTypes();
        }

        protected static void FolderAction(object sender, EventArgs e)
            => GetInstance<FolderWrapper>().NewFolder();
        protected static void RenameAction(object sender, EventArgs e)
            => GetInstance<FolderWrapper>().Rename();
        protected static void DeleteAction(object sender, EventArgs e)
            => GetInstance<FolderWrapper>().Delete();
        protected static void ArchiveAction(object sender, EventArgs e)
            => GetInstance<FolderWrapper>().ToArchive();
        protected static void ExplorerAction(object sender, EventArgs e)
            => GetInstance<FolderWrapper>().OpenInExplorer();

        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            _menu.Items[7].Enabled = _menu.Items[9].Enabled = true;
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            FolderWrapper w = GetInstance<FolderWrapper>();
            _menu.Items[7].Enabled = !string.IsNullOrEmpty(w.FilePath) && Directory.Exists(w.FilePath);
            _menu.Items[9].Enabled = w.Parent != null;
        }
        #endregion

        private string _path;

        public override string FilePath
        {
            get => _path;
            internal set => _path = value;
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
                FileSystem.DeleteDirectory(FilePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
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
            Dictionary<string, NamespaceNode> nodes = new Dictionary<string, NamespaceNode>();
            foreach (Type t in fileObjectTypes)
            {
                string path = t.Namespace;
                int dotIndex = path.IndexOf(".");
                string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                if (nodes.ContainsKey(name))
                    nodes[name].Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, t);
                else
                {
                    NamespaceNode node = new NamespaceNode(name);
                    nodes.Add(name, node);
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
                b.Nodes.Add(Wrap(file));
            }
        }
        #endregion
    }
}
