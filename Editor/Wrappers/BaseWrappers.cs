using System;
using System.Windows.Controls;
using CustomEngine;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Windows;

namespace Editor.Wrappers
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class NodeWrapperAttribute : Attribute
    {
        ResourceType _type;
        public NodeWrapperAttribute(ResourceType type) { _type = type; }
        public ResourceType WrappedType { get { return _type; } }

        private static Dictionary<ResourceType, Type> _wrappers;
        public static Dictionary<ResourceType, Type> Wrappers
        {
            get
            {
                if (_wrappers == null)
                {
                    _wrappers = new Dictionary<ResourceType, Type>();
                    foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
                        foreach (NodeWrapperAttribute attr in t.GetCustomAttributes(typeof(NodeWrapperAttribute), true))
                            _wrappers[attr._type] = t;
                }
                return _wrappers;
            }
        }
    }
    [Serializable]
    public abstract class BaseWrapper : TreeViewItem
    {
        protected static readonly ContextMenu _emptyMenu = new ContextMenu();

        protected bool _discovered = false;

        protected ObjectBase _resource;
        public ObjectBase ResourceNode
        {
            get { return _resource; }
            //set { Link(value); }
        }

        protected BaseWrapper() { }
        //protected BaseWrapper(ResourceNode resourceNode) { Link(resourceNode); }

        protected static void SetMenuEnabled(ContextMenu m, bool enabled, params int[] items)
        {
            foreach (int i in items)
            {
                MenuItem mi = m.Items[i] as MenuItem;
                if (mi != null)
                    mi.IsEnabled = enabled;
            }
        }
        protected static T GetInstance<T>() where T : BaseWrapper
        {
            //return MainWindow.Instance.ResourceTree.SelectedNode as T;
            return null;
        }

        public void Link(ObjectBase res)
        {
            Unlink();
            if (res != null)
            {
                Header = res.Name;
                ItemCollection nodes = Items;

                //Should we continue down the tree?
                if (IsExpanded && (res.HasChildren))
                {
                    //Add/link each resource node
                    foreach (ObjectBase n in res.Children)
                    {
                        bool found = false;
                        foreach (BaseWrapper tn in nodes)
                            if ((string)tn.Header == n.Name)
                            {
                                tn.Link(n);
                                found = true;
                                // Move node to bottom, to ensure that nodes are shown and saved in the same order as in the original data
                                nodes.Remove(tn);
                                nodes.Add(tn);
                                break;
                            }
                        if (!found)
                            nodes.Add(Wrap(_owner, n));
                    }

                    //Remove empty nodes
                    for (int i = 0; i < nodes.Count;)
                    {
                        BaseWrapper n = nodes[i] as BaseWrapper;
                        if (n._resource == null)
                            n.Remove();
                        else
                            i++;
                    }

                    _discovered = true;
                }
                else
                {
                    //Node will be reset and undiscovered
                    nodes.Clear();
                    //Collapse();
                    if (res.HasChildren)
                    {
                        nodes.Add(new GenericWrapper());
                        _discovered = false;
                    }
                    else
                        _discovered = true;
                }

                //SelectedImageIndex = ImageIndex = (int)res.ResourceType & 0xFF;
                res.Renamed += OnRenamed;
                res.PropertyChanged += OnPropertyChanged;
                res.UpdateProperties += OnUpdateProperties;
                res.UpdateEditor += OnUpdateCurrentControl;
            }
            _resource = res;
        }
        public void Unlink()
        {
            if (_resource != null)
            {
                _resource.Renamed -= OnRenamed;
                _resource.PropertyChanged -= OnPropertyChanged;
                _resource.UpdateProperties -= OnUpdateProperties;
                _resource.UpdateEditor -= OnUpdateCurrentControl;
                _resource = null;
            }

            foreach (BaseWrapper n in Items)
                n.Unlink();
        }
        private void Remove()
        {
            throw new NotImplementedException();
        }
        internal void EnsureVisible()
        {
            throw new NotImplementedException();
        }
        internal protected virtual void OnSelectChild(int index)
        {
            //if (!(Items == null || index < 0 || index >= Items.Count))
            //    TreeView.SelectedNode = Items[index];
        }
        internal protected virtual void OnUpdateProperties(ObjectBase obj)
        {
            //MainWindow.Instance.PropertyGrid.Refresh();
        }
        internal protected virtual void OnUpdateCurrentControl(ObjectBase obj)
        {
            MainWindow form = MainWindow.Instance;
            //var g = form.propertyGrid1.SelectedGridItem;
            //form._currentControl = null;
            //form.resourceTree_SelectionChanged(this, null);
        }
        internal protected virtual void OnChildAdded(ObjectBase parent, ObjectBase child)
        {
            Items.Add(Wrap(_owner, child));
        }
        internal protected virtual void OnChildInserted(int index, ObjectBase parent, ObjectBase child)
        {
            Items.Insert(index, Wrap(_owner, child));
        }
        internal protected virtual void OnChildRemoved(ObjectBase parent, ObjectBase child)
        {
            foreach (BaseWrapper w in Items)
                if (w != null)
                    if (w._resource == child)
                    {
                        w.Unlink();
                        w.Remove();
                    }
        }
        internal protected void RefreshView(ObjectBase node)
        {
            Link(node);

            //if ((TreeView != null) && (TreeView.SelectedNode == this))
            //{
            //    ((ResourceTree)TreeView).SelectedNode = null;
            //    TreeView.SelectedNode = this;
            //}
        }
        internal protected virtual void OnRestored(ObjectBase node) { RefreshView(node); }
        internal protected virtual void OnReplaced(ObjectBase node) { RefreshView(node); }
        internal protected virtual void OnRenamed(ObjectBase node, string oldName) { Header = node.Name; }

        internal protected virtual void OnMovedUp(ObjectBase node, bool select)
        {
            GenericWrapper res = this.FindResource(node, false) as GenericWrapper;
            res.MoveUp(select);
            res.EnsureVisible();
            //res.TreeView.SelectedNode = res;
        }
        internal protected virtual void OnMovedDown(ObjectBase node, bool select)
        {
            GenericWrapper res = this.FindResource(node, false) as GenericWrapper;
            res.MoveDown(select);
            res.EnsureVisible();
            //res.TreeView.SelectedNode = res;
        }
        internal protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e) { }
        
        internal protected virtual void OnExpand()
        {
            //if (!_discovered)
            //{
            //    Items.Clear();

            //    //if (_resource._isPopulating)
            //    //    while (_resource._isPopulating) { System.Windows.Forms.Application.DoEvents(); }

            //    foreach (ObjectBase n in _resource.Children)
            //        Items.Add(Wrap(_owner, n));
            //    _discovered = true;
            //}
        }
        internal protected virtual void OnDoubleClick() { }

        internal BaseWrapper FindResource(ObjectBase n, bool searchChildren)
        {
            BaseWrapper node;
            if (_resource == n)
                return this;
            else
            {
                OnExpand();
                foreach (BaseWrapper c in Items)
                    if (c._resource == n)
                        return c;
                    else if ((searchChildren) && ((node = c.FindResource(n, true)) != null))
                        return node;
            }
            return null;
        }

        public static Control _owner;
        public static BaseWrapper Wrap(ObjectBase node) { return Wrap(null, node); }
        public static BaseWrapper Wrap(Control owner, ObjectBase node)
        {
            _owner = owner;
            BaseWrapper w;
            if (!NodeWrapperAttribute.Wrappers.ContainsKey(node.ResourceType))
                w = new GenericWrapper();
            else
                w = Activator.CreateInstance(NodeWrapperAttribute.Wrappers[node.ResourceType]) as BaseWrapper;
            w.Link(node);
            return w;
        }
    }
}
