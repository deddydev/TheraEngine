using Extensions;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Windows.Forms;
using TheraEditor.ContentTree.Core;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Wrappers
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class TreeFileTypeAttribute : Attribute
    {
        public TreeFileTypeAttribute()
        {

        }
        public TreeFileTypeAttribute(string imageName, string selectedImageName)
        {
            ImageName = imageName;
            SelectedImageName = selectedImageName;
        }
        public TreeFileTypeAttribute(string thirdPartyExtension)
        {
            ThirdPartyExtension = thirdPartyExtension;
        }

        //public Type FileType { get; }
        public string ImageName { get; }
        public string SelectedImageName { get; }
        public string ThirdPartyExtension { get; }
    }
    //TODO: make property of one tree node class
    //Determine what wrapper to use by extension after game domain load
    public abstract class ContentTreeNode : TreeNode
    {
        public ContentTreeNode(string path)
        {
            FilePath = path;
            Text = Path.GetFileName(path);

            Engine.Instance.DomainProxyCreated += Instance_DomainProxyPostSet;
            Engine.Instance.DomainProxyDestroying += Instance_DomainProxyPreUnset;
            Instance_DomainProxyPostSet(Engine.DomainProxy);
        }

        protected virtual void Instance_DomainProxyPostSet(TheraEngine.Core.EngineDomainProxy proxy)
        {
            DetermineWrapper();
        }
        protected virtual void Instance_DomainProxyPreUnset(TheraEngine.Core.EngineDomainProxy proxy)
        {
            DestroyWrapper();
        }

        public virtual void DestroyWrapper()
        {
            if (Wrapper != null)
            {
                Wrapper.RenameEvent -= Rename;
                Wrapper.CopyEvent -= Copy;
                Wrapper.CutEvent -= Cut;
                Wrapper.PasteEvent -= Paste;
                Wrapper.DeleteEvent -= Delete;

                Menu = null;

                AppDomainHelper.ReleaseSponsor(Wrapper);

                Wrapper = null;
            }
        }
        public virtual void DetermineWrapper()
        {
            try
            {
                IBasePathWrapper wrapper = TryWrapPath(FilePath);

                if (wrapper != null)
                {
                    wrapper.RenameEvent += Rename;
                    wrapper.CopyEvent += Copy;
                    wrapper.CutEvent += Cut;
                    wrapper.PasteEvent += Paste;
                    wrapper.DeleteEvent += Delete;

                    Menu = wrapper.Menu;
                }

                Wrapper = wrapper;
            }
            catch (IOException)
            {
                Trace.WriteLine($"Failed to determine wrapper for {Path.GetFileName(FilePath)}.");
            }
        }

        public IBasePathWrapper Wrapper 
        {
            get => _wrapper;
            protected set
            {
                if (value != null && !RemotingServices.IsTransparentProxy(value) && AppDomainHelper.AppDomains.Length > 1)
                    throw new InvalidOperationException("Content tree wrappers must exist in the other domain.");

                _wrapper = value;
                AppDomainHelper.Sponsor(_wrapper);
            }
        }

        public virtual string FilePath
        {
            get => Name;
            set
            {
                Name = value;
                Text = Path.GetFileName(value);

                if (Wrapper != null)
                    Wrapper.FilePath = value;
            }
        }

        protected bool _isPopulated = false;
        private ITMenu _menu;
        private IBasePathWrapper _wrapper;

        public new ResourceTree TreeView => (ResourceTree)base.TreeView;
        public new ContentTreeNode Parent => base.Parent as ContentTreeNode; //Parent may be null

        public bool IsPopulated => _isPopulated;

        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                if (base.ContextMenuStrip is null)
                    GenerateMenu();
                return base.ContextMenuStrip;
            }
            set => base.ContextMenuStrip = value;
        }

        public ITMenu Menu
        {
            get => _menu;
            set
            {
                if (ContextMenuStrip != null)
                {
                    ContextMenuStrip.Opening -= Strip_Opening;
                    ContextMenuStrip.Closing -= Strip_Closing;
                    ContextMenuStrip.Dispose();
                    ContextMenuStrip = null;
                }

                _menu = value;

                AppDomainHelper.Sponsor(_menu);
            }
        }

        //TODO: use previous method of making one context menu per file type instead of per node
        protected void GenerateMenu() => GenerateMenu(_menu);
        protected void GenerateMenu(ITMenu menu)
        {
            ContextMenuStrip strip = new ContextMenuStrip
            {
                RenderMode = ToolStripRenderMode.Professional,
                Renderer = new TheraForm.TheraToolStripRenderer()
            };

            AddMenuToCollection(menu, strip.Items);

            strip.Tag = menu;
            AppDomainHelper.Sponsor(menu);

            if (base.ContextMenuStrip != null)
            {
                ContextMenuStrip.Opening -= Strip_Opening;
                ContextMenuStrip.Closing -= Strip_Closing;
            }

            ContextMenuStrip = strip;

            ContextMenuStrip.Opening += Strip_Opening;
            ContextMenuStrip.Closing += Strip_Closing;
        }

        protected virtual void Strip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
            => ((ITMenu)ContextMenuStrip.Tag).OnOpening();
        protected virtual void Strip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
            => ((ITMenu)ContextMenuStrip.Tag).OnClosing();

        private static void AddMenuToCollection(ITMenu menu, ToolStripItemCollection coll)
        {
            if (menu is null)
                return;

            try
            {
                foreach (ITMenuItem item in menu)
                    AddItemToCollection(item, coll);
            }
            catch { }
        }

        private static void AddItemToCollection(ITMenuItem item, ToolStripItemCollection coll)
        {
            if (Editor.Instance.InvokeRequired)
            {
                Editor.Instance.BeginInvoke((Action<ITMenuItem, ToolStripItemCollection>)AddItemToCollection, item, coll);
                return;
            }
            if (item is ITMenuDivider)
                coll.Add(new ToolStripSeparator());
            else if (item is ITMenuOption op)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(op.Text, null, MenuItemClicked, op.HotKeys);
                menuItem.Tag = new ItemWrapper(menuItem, op);
                AppDomainHelper.Sponsor(op);
                AddMenuToCollection(op, menuItem.DropDownItems);
                coll.Add(menuItem);
            }
        }

        private class ItemWrapper : TObjectSlim
        {
            public ITMenuOption Option { get; set; }
            public ToolStripMenuItem Item { get; set; }

            public ItemWrapper(ToolStripMenuItem item, ITMenuOption option)
            {
                Item = item;
                Option = option;

                option.ChildAdded += Option_ChildAdded;
                option.ChildrenCleared += Option_ChildrenCleared;
            }

            private void Option_ChildrenCleared() => Item.DropDownItems.Clear();
            private void Option_ChildAdded(ITMenuItem item) => AddItemToCollection(item, Item.DropDownItems);
        }

        private static void MenuItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ItemWrapper op = item?.Tag as ItemWrapper;
            op?.Option?.ExecuteAction();
        }

        protected static ResourceTree Tree
            => Editor.Instance.ContentTree;

        protected static T GetInstance<T>() where T : ContentTreeNode
            => Tree.SelectedNode as T;

        public abstract void Delete();
        public void Rename()
        {
            if (!IsEditing)
            {
                TreeView.LabelEdit = true;
                BeginEdit();
            }
        }

        public void Paste() => TreeView.Paste(FilePath);
        public void Cut() => SetClipboard(true);
        public void Copy() => SetClipboard(false);
        private void SetClipboard(bool cut)
        {
            string[] paths = new string[] { FilePath };//Directory.GetFileSystemEntries(path, "*.*", System.IO.SearchOption.TopDirectoryOnly);
            ResourceTree.SetClipboard(paths, cut);
        }

        public static ContentTreeNode Wrap(string path)
        {
            ContentTreeNode treeNode = null;

            bool? isDir = path.IsExistingDirectoryPath();
            if (isDir != null)
            {
                bool dir = isDir.Value;
                if (dir)
                {
                    //try
                    //{
                    treeNode = new FolderTreeNode(path);
                    if (Directory.GetFileSystemEntries(path).Length > 0)
                        treeNode.Nodes.Add("...");
                    //}
                    //catch { }
                }
                else
                {
                    treeNode = new FileTreeNode(path);
                }
            }

            return treeNode;
        }
        //public static BaseFileWrapper Wrap(TFileObject file)
        //{
        //    BaseFileWrapper w = TryWrapType(file?.GetType());
        //    if (w != null)
        //    {
        //        TFileObject.GetDirNameFmt(file.FilePath, out string dir, out string name, out EFileFormat fmt, out string thirdPartyExt);
        //        w.Text = name + "." + file.FileExtension.GetFullExtension((EProprietaryFileFormat)fmt);
        //        w.SingleInstance = file;
        //        //w.SelectedImageIndex = w.ImageIndex = 0;
        //    }
        //    return w;
        //}

        public static IBasePathWrapper TryWrapPath(string path) 
            => Editor.DomainProxy.TryWrapPath(path);

        internal protected abstract void OnExpand();
        internal protected abstract void OnCollapse();
        internal protected abstract void SetPath(string parentFolderPath);

        internal void HandlePathDrop(string path, bool copy)
        {
            bool? isDir = path.IsExistingDirectoryPath();
            if (isDir is null)
                return;
            string newPath = this is FileTreeNode ? Path.GetDirectoryName(FilePath) : FilePath;
            try
            {
                if (isDir.Value)
                {
                    if (copy)
                        FileSystem.CopyDirectory(path, newPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                    else
                        FileSystem.MoveDirectory(path, newPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                }
                else
                {
                    newPath += Path.DirectorySeparatorChar + Path.GetFileName(path);
                    if (copy)
                        FileSystem.CopyFile(path, newPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                    else
                        FileSystem.MoveFile(path, newPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                }
            }
            catch { return; }

            ContentTreeNode child = Wrap(newPath);
            if (child != null)
                Nodes.Add(child);
            else
                throw new Exception();
        }
        internal void HandleNodeDrop(ContentTreeNode node, bool copy)
        {
            bool isFileNode = this is FileTreeNode;
            string destPath = isFileNode ? Path.GetDirectoryName(FilePath) : FilePath;

            if (string.IsNullOrEmpty(destPath))
                return;

            if (!destPath.EndsWith("\\"))
                destPath += "\\";

            string fileName = Path.GetFileName(node.FilePath);
            destPath += fileName;

            if (string.Equals(node.FilePath, destPath, StringComparison.InvariantCulture))
                return;

            if (node is FileTreeNode fileNode)
            {
                try
                {
                    if (copy)
                        FileSystem.CopyFile(fileNode.FilePath, destPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                    else
                        FileSystem.MoveFile(fileNode.FilePath, destPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                }
                catch { return; }
            }
            else if (node is FolderTreeNode folderNode)
            {
                try
                {
                    if (!Directory.Exists(destPath))
                        Directory.CreateDirectory(destPath);
                    if (copy)
                        FileSystem.CopyDirectory(folderNode.FilePath, destPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                    else
                        FileSystem.MoveDirectory(folderNode.FilePath, destPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                }
                catch { return; }
            }

            if (!isFileNode && !IsPopulated && Nodes.Count == 0)
            {
                if (Directory.GetFileSystemEntries(FilePath).Length > 0)
                    Nodes.Add("...");
            }
        }
    }
}
