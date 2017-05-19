using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using CustomEngine;
using CustomEngine.Files;
using System.IO;
using System.Drawing;

namespace TheraEditor.Wrappers
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class NodeWrapperAttribute : Attribute
    {
        private Type _type;
        private SystemImages _image;
        
        public NodeWrapperAttribute(Type type, SystemImages image)
        {
            _type = type;
            _image = image;
        }

        public Type WrappedType => _type;

        private static Dictionary<Type, Type> _wrappers;
        public static Dictionary<Type, Type> Wrappers
        {
            get
            {
                if (_wrappers == null)
                {
                    _wrappers = new Dictionary<Type, Type>();
                    foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
                        foreach (NodeWrapperAttribute attr in t.GetCustomAttributes(typeof(NodeWrapperAttribute), true))
                            _wrappers[attr._type] = t;
                }
                return _wrappers;
            }
        }
    }
    public abstract class BaseWrapper : TreeNode
    {
        protected string _filePath;
        protected FileObject _file;
        protected bool _discovered = false;

        public string FilePath => _filePath;
        public FileObject File => _file;

        protected BaseWrapper() { }

        protected static T GetInstance<T>() where T : BaseWrapper
        {
            return null;
            //return Editor.Instance.FileTree.SelectedNode as T;
        }

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
        //internal protected virtual void OnExpand()
        //{
        //    if (!_discovered)
        //    {
        //        Nodes.Clear();

        //        if (_resource._isPopulating)
        //            while (_resource._isPopulating) { Application.DoEvents(); }

        //        foreach (ResourceNode n in _resource.Children)
        //            Nodes.Add(Wrap(_owner, n));
        //        _discovered = true;
        //    }
        //}
        //internal protected virtual void OnDoubleClick() { }
        //public static BaseWrapper Wrap(string path)
        //{
        //    BaseWrapper w;
        //    ResourceType? type = FileManager.GetResourceTypeWithExtension(Path.GetExtension(path));
        //    if (type.HasValue && NodeWrapperAttribute.Wrappers.ContainsKey(type.Value))
        //        w = Activator.CreateInstance(NodeWrapperAttribute.Wrappers[type.Value], path) as BaseWrapper;
        //    else
        //        w = new GenericWrapper(path);
        //    return w;
        //}
        //public static bool CanWrap(string path)
        //{
        //    return FileManager.GetResourceTypeWithExtension(Path.GetExtension(path)).HasValue;
        //}
    }
}
