using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using TheraEngine.Files;
using System.IO;

namespace TheraEditor.Wrappers
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class NodeWrapperAttribute : Attribute
    {
        private Type _fileType;
        private SystemImages _image;

        public NodeWrapperAttribute(Type type, SystemImages image)
        {
            _fileType = type;
            _image = image;
        }

        public Type FileType => _fileType;

        /// <summary>
        /// Key is file type, Value is tree node wrapper type
        /// </summary>
        public static Dictionary<Type, Type> Wrappers
        {
            get
            {
                if (_wrappers == null)
                {
                    _wrappers = new Dictionary<Type, Type>();
                    LoadWrappers(Assembly.GetExecutingAssembly());
                }
                return _wrappers;
            }
        }

        public static void LoadWrappers(Assembly assembly)
        {
            if (assembly != null)
                foreach (Type asmType in assembly.GetTypes())
                    foreach (NodeWrapperAttribute attr in asmType.GetCustomAttributes(typeof(NodeWrapperAttribute), true))
                        _wrappers[attr.FileType] = asmType;
        }

        private static Dictionary<Type, Type> _wrappers;
    }
    public abstract class BaseWrapper : TreeNode
    {
        protected string _filePath;
        protected FileObject _file;
        protected bool _discovered = false;

        public string FilePath
        {
            get => _filePath;
            internal set => _filePath = value;
        }

        public FileObject Resource => _file;

        protected BaseWrapper() { }

        protected static T GetInstance<T>() where T : BaseWrapper
            => Editor.Instance.ContentTree.SelectedNode as T;
        
        public void Link(string path)
        {
            //Unlink();
            _filePath = path;

            //if (res != null)
            //{
            //    Text = res.Name;
            //    TreeNodeCollection nodes = Nodes;

            //    //Should we continue down the tree?
            //    if (IsExpanded && res.HasChildren)
            //    {
            //        //Add/link each resource node
            //        foreach (ResourceNode n in res.Children)
            //        {
            //            bool found = false;
            //            foreach (BaseWrapper tn in nodes)
            //                if (tn.Text == n.Name)
            //                {
            //                    tn.Link(n);
            //                    found = true;
            //                    // Move node to bottom, to ensure that nodes are shown and saved in the same order as in the original data
            //                    nodes.Remove(tn);
            //                    nodes.Add(tn);
            //                    break;
            //                }

            //            if (!found)
            //                nodes.Add(Wrap(_owner, n));
            //        }

            //        //Remove empty nodes
            //        for (int i = 0; i < nodes.Count;)
            //        {
            //            BaseWrapper n = nodes[i] as BaseWrapper;
            //            if (n._resource == null)
            //                n.Remove();
            //            else
            //                i++;
            //        }

            //        _discovered = true;
            //    }
            //    else
            //    {
            //        //Node will be reset and undiscovered
            //        nodes.Clear();
            //        //Collapse();
            //        if (res.HasChildren)
            //        {
            //            nodes.Add(new GenericWrapper());
            //            _discovered = false;
            //        }
            //        else
            //            _discovered = true;
            //    }

            //    SelectedImageIndex = ImageIndex = (int)res.ResourceType & 0xFF;

            //    //res.SelectChild += OnSelectChild;
            //    //res.ChildAdded += OnChildAdded;
            //    //res.ChildRemoved += OnChildRemoved;
            //    //res.ChildInserted += OnChildInserted;
            //    //res.Replaced += OnReplaced;
            //    //res.Restored += OnRestored;
            //    //res.Renamed += OnRenamed;
            //    //res.MovedUp += OnMovedUp;
            //    //res.MovedDown += OnMovedDown;
            //    //res.PropertyChanged += OnPropertyChanged;
            //    //res.UpdateProps += OnUpdateProperties;
            //    //res.UpdateControl += OnUpdateCurrentControl;
            //}
        }

        public void Unlink()
        {
            _filePath = null;
            _file = null;

            //if (_file != null)
            //{
            //    _file.SelectChild -= OnSelectChild;
            //    _file.ChildAdded -= OnChildAdded;
            //    _file.ChildRemoved -= OnChildRemoved;
            //    _file.ChildInserted -= OnChildInserted;
            //    _file.Replaced -= OnReplaced;
            //    _file.Restored -= OnRestored;
            //    _file.Renamed -= OnRenamed;
            //    _file.MovedUp -= OnMovedUp;
            //    _file.MovedDown -= OnMovedDown;
            //    _file.PropertyChanged -= OnPropertyChanged;
            //    _file.UpdateProps -= OnUpdateProperties;
            //    _file.UpdateControl -= OnUpdateCurrentControl;
            //    _file = null;
            //}

            //foreach (BaseWrapper n in Nodes)
            //    n.Unlink();
        }
        
        //internal protected virtual void OnSelectChild(int index)
        //{
        //    if (!(Nodes == null || index < 0 || index >= Nodes.Count))
        //        TreeView.SelectedNode = Nodes[index];
        //}
        //internal protected virtual void OnUpdateProperties(object sender, EventArgs e)
        //{
        //    Editor.Instance.PropertyGrid.Refresh();
        //}
        //internal protected virtual void OnUpdateCurrentControl(object sender, EventArgs e)
        //{
        //    MainForm form = MainForm.Instance;
        //    //var g = form.propertyGrid1.SelectedGridItem;
        //    form._currentControl = null;
        //    form.resourceTree_SelectionChanged(this, null);
        //}
        //internal protected virtual void OnChildAdded(ResourceNode parent, ResourceNode child)
        //{
        //    Nodes.Add(Wrap(_owner, child));
        //}
        //internal protected virtual void OnChildInserted(int index, ResourceNode parent, ResourceNode child)
        //{
        //    Nodes.Insert(index, Wrap(_owner, child));
        //}
        //internal protected virtual void OnChildRemoved(ResourceNode parent, ResourceNode child)
        //{
        //    foreach (BaseWrapper w in Nodes)
        //        if (w != null)
        //            if (w._resource == child)
        //            {
        //                w.Unlink();
        //                w.Remove();
        //            }
        //}
        //internal protected void RefreshView(ResourceNode node)
        //{
        //    Link(node);

        //    if ((TreeView != null) && (TreeView.SelectedNode == this))
        //    {
        //        ((ResourceTree)TreeView).SelectedNode = null;
        //        TreeView.SelectedNode = this;
        //    }
        //}
        //internal protected virtual void OnRestored(ResourceNode node)
        //{
        //    RefreshView(node);
        //}
        //internal protected virtual void OnReplaced(ResourceNode node) { RefreshView(node); }
        //internal protected virtual void OnRenamed(ResourceNode node) { Text = node.Name; }
        //internal protected virtual void OnMovedUp(ResourceNode node, bool select)
        //{
        //    GenericWrapper res = this.FindResource(node, false) as GenericWrapper;
        //    res.MoveUp(select);
        //    res.EnsureVisible();
        //    //res.TreeView.SelectedNode = res;
        //}
        //internal protected virtual void OnMovedDown(ResourceNode node, bool select)
        //{
        //    GenericWrapper res = this.FindResource(node, false) as GenericWrapper;
        //    res.MoveDown(select);
        //    res.EnsureVisible();
        //    //res.TreeView.SelectedNode = res;
        //}
        //internal protected virtual void OnPropertyChanged(ResourceNode node) { }
        internal protected virtual void OnExpand()
        {
            if (!_discovered)
            {
                if (Nodes.Count > 0 &&
                    Nodes[0].Text == "..." &&
                    Nodes[0].Tag == null)
                {
                    Nodes.Clear();

                    string path = FilePath.ToString();
                    string[] dirs = Directory.GetDirectories(path);
                    foreach (string dir in dirs)
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        BaseWrapper node = Wrap(dir);
                        try
                        {
                            //If the directory has sub directories, add the placeholder
                            if (di.GetDirectories().Length > 0)
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
                        FileInfo fi = new FileInfo(file);
                        BaseWrapper node = Wrap(file);
                        Nodes.Add(node);
                    }
                }
                _discovered = true;
            }
        }
        public new void Remove()
        {
            base.Remove();
        }
        //internal protected virtual void OnDoubleClick() { }
        public static BaseWrapper Wrap(string path)
        {
            BaseWrapper w = null;
            FileAttributes attr = File.GetAttributes(path);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                w = new FolderWrapper();
                if (Directory.GetFiles(path).Length > 0 ||
                    Directory.GetDirectories(path).Length > 0)
                {
                    w.Nodes.Add("...");
                }
            }
            else
            {
                Type type = FileObject.DetermineType(path);
                if (type != null && NodeWrapperAttribute.Wrappers.ContainsKey(type))
                    w = Activator.CreateInstance(NodeWrapperAttribute.Wrappers[type]) as BaseWrapper;
                else
                    w = new FileWrapper();
            }
            w.Text = Path.GetFileNameWithoutExtension(path);
            w.FilePath = w.Name = path;
            return w;
        }
    }
}
