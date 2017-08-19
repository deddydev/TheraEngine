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
using System.Reflection;

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
            _menu.Items[0].Enabled = w.TreeView.LabelEdit;
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

        public override string FilePath
        {
            get => Name;
            set => Name = value;
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
                if (Directory.GetFileSystemEntries(FilePath).Length > 0)
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

        public void CheckNodes()
        {

        }

        internal protected override void OnExpand()
        {
            if (!_isPopulated)
            {
                if (Nodes.Count > 0 && Nodes[0].Text == "..." && Nodes[0].Tag == null)
                {
                    Nodes.Clear();

                    string path = FilePath.ToString();
                    string[] subDirs = Directory.GetDirectories(path);
                    foreach (string subDir in subDirs)
                    {
                        DirectoryInfo subDirInfo = new DirectoryInfo(subDir);
                        BaseWrapper node = Wrap(subDir);
                        if (node == null)
                            continue;
                        try
                        {
                            //If the directory has sub folders/files, add the placeholder
                            if (subDirInfo.GetFileSystemInfos().Length > 0)
                                node.Nodes.Add(null, "...", 0, 0);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            //display a locked folder icon
                            node.ImageIndex = 2;
                            node.SelectedImageIndex = 2;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "DirectoryReader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            Nodes.Add(node);
                        }
                    }

                    string[] files = Directory.GetFiles(path);
                    foreach (string file in files)
                    {
                        BaseWrapper node = Wrap(file);
                        if (node != null)
                            Nodes.Add(node);
                    }
                }
                _isPopulated = true;
            }
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
                    ((ToolStripDropDownItem)_menu.Items[4]).DropDownItems.Add(node.Button);
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
        public enum GenericVarianceFlag
        {
            None,
            CovariantOut,
            ContravariantIn,
        }
        public enum TypeConstraintFlag
        {
            None,
            Struct,             //struct
            Class,              //class
            NewClass,           //class, new()
            NewStructOrClass,   //new()
        }
        private static void ListGenericParameterAttributes(Type t, out GenericVarianceFlag gvf, out TypeConstraintFlag tcf)
        {
            GenericParameterAttributes gpa = t.GenericParameterAttributes;
            GenericParameterAttributes variance = gpa & GenericParameterAttributes.VarianceMask;
            GenericParameterAttributes constraints = gpa & GenericParameterAttributes.SpecialConstraintMask;

            gvf = GenericVarianceFlag.None;
            tcf = TypeConstraintFlag.None;

            if (variance != GenericParameterAttributes.None)
            {
                if ((variance & GenericParameterAttributes.Covariant) != 0)
                    gvf = GenericVarianceFlag.CovariantOut;
                else
                    gvf = GenericVarianceFlag.ContravariantIn;
            }
            
            if (constraints != GenericParameterAttributes.None)
            {
                if ((constraints & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
                    tcf = TypeConstraintFlag.Struct;
                else
                {
                    if ((constraints & GenericParameterAttributes.DefaultConstructorConstraint) != 0)
                        tcf = TypeConstraintFlag.NewStructOrClass;
                    if ((constraints & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
                    {
                        if (tcf == TypeConstraintFlag.NewStructOrClass)
                            tcf = TypeConstraintFlag.NewClass;
                        else
                            tcf = TypeConstraintFlag.Class;
                    }
                }
            }
        }
        private static void OnNewClick(object sender, EventArgs e)
        {
            if (sender is ToolStripDropDownButton button)
            {
                FileObject file;
                Type fileType = button.Tag as Type;
                if (fileType.ContainsGenericParameters)
                {
                    Type[] args = fileType.GetGenericArguments();
                    foreach (Type genArg in args)
                    {

                    }
                    Type genericFileWrapper = fileType.MakeGenericType(args);
                    file = Activator.CreateInstance(genericFileWrapper) as FileObject;
                }
                else
                    file = Activator.CreateInstance(fileType) as FileObject;

                ContextMenuStrip ctx = button.GetContextMenuStrip();
                FolderWrapper folderNode = GetInstance<FolderWrapper>();
                string dir = folderNode.FilePath as string;

                folderNode.TreeView.WatchProjectDirectory = false;
                file.Export(dir, file.Name, FileFormat.XML);
                folderNode.TreeView.WatchProjectDirectory = true;

                folderNode.Nodes.Add(Wrap(file) as BaseWrapper);
            }
        }
        #endregion

        internal protected override void HandlePathDrop(string path, bool copy)
        {
            bool? isDir = path.IsDirectory();
            if (isDir == null)
                return;
            TreeView.WatchProjectDirectory = false;
            string newPath;
            try
            {
                if (isDir.Value)
                {
                    newPath = FilePath;
                    if (copy)
                        FileSystem.CopyDirectory(path, newPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                    else
                        FileSystem.MoveDirectory(path, newPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                }
                else
                {
                    string name = Path.GetFileName(path);
                    newPath = FilePath + name;
                    if (copy)
                        FileSystem.CopyFile(path, newPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                    else
                        FileSystem.MoveFile(path, newPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                }
            }
            catch (OperationCanceledException e)
            {
                //finally block is called before returning, despite appearing after
                return;
            }
            finally
            {
                TreeView.WatchProjectDirectory = true;
            }
            
            BaseWrapper child = Wrap(newPath);
            if (child != null)
                Nodes.Add(child);
            else
                throw new Exception();
        }

        internal protected override void HandleNodeDrop(BaseWrapper node, bool copy)
        {
            if (node is BaseFileWrapper fileNode)
            {
                try
                {
                    string name = Path.GetFileName(fileNode.FilePath);
                    if (copy)
                        FileSystem.CopyFile(fileNode.FilePath, FilePath + name, UIOption.AllDialogs, UICancelOption.ThrowException);
                    else
                        FileSystem.MoveFile(fileNode.FilePath, FilePath + name, UIOption.AllDialogs, UICancelOption.ThrowException);
                }
                catch (OperationCanceledException e)
                {
                    return;
                }
            }
            else if (node is FolderWrapper folderNode)
            {
                try
                {
                    if (copy)
                        FileSystem.CopyDirectory(folderNode.FilePath, FilePath, UIOption.AllDialogs, UICancelOption.ThrowException);
                    else
                        FileSystem.MoveDirectory(folderNode.FilePath, FilePath, UIOption.AllDialogs, UICancelOption.ThrowException);
                }
                catch (OperationCanceledException e)
                {
                    return;
                }

            }
        }
    }
}
