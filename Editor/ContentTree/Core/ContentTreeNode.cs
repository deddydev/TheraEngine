using Extensions;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
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

        protected virtual void DestroyWrapper()
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
        protected virtual void DetermineWrapper()
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

        public IBasePathWrapper Wrapper 
        {
            get => _wrapper;
            protected set
            {
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
        private ITheraMenu _menu;
        private IBasePathWrapper _wrapper;

        public new ResourceTree TreeView => (ResourceTree)base.TreeView;
        public new ContentTreeNode Parent => base.Parent as ContentTreeNode; //Parent may be null

        public bool IsPopulated => _isPopulated;

        public ITheraMenu Menu
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

                GenerateMenu();
            }
        }

        protected void GenerateMenu() => GenerateMenu(_menu);
        protected void GenerateMenu(ITheraMenu menu)
        {
            ContextMenuStrip strip = new ContextMenuStrip
            {
                RenderMode = ToolStripRenderMode.Professional,
                Renderer = new TheraForm.TheraToolStripRenderer()
            };

            AddMenuToCollection(menu, strip.Items);

            strip.Tag = menu;
            AppDomainHelper.Sponsor(menu);

            ContextMenuStrip = strip;
            ContextMenuStrip.Opening += Strip_Opening;
            ContextMenuStrip.Closing += Strip_Closing;
        }

        protected virtual void Strip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
            => ((ITheraMenu)ContextMenuStrip.Tag).OnOpening();
        protected virtual void Strip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
            => ((ITheraMenu)ContextMenuStrip.Tag).OnClosing();

        private static void AddMenuToCollection(ITheraMenu menu, ToolStripItemCollection coll)
        {
            if (menu is null)
                return;

            foreach (ITheraMenuItem item in menu)
                AddItemToCollection(item, coll);
        }

        private static void AddItemToCollection(ITheraMenuItem item, ToolStripItemCollection coll)
        {
            if (item is ITheraMenuDivider)
                coll.Add(new ToolStripSeparator());
            else if (item is ITheraMenuOption op)
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
            public ITheraMenuOption Option { get; set; }
            public ToolStripMenuItem Item { get; set; }

            public ItemWrapper(ToolStripMenuItem item, ITheraMenuOption option)
            {
                Item = item;
                Option = option;

                option.ChildAdded += Option_ChildAdded;
                option.ChildrenCleared += Option_ChildrenCleared;
            }

            private void Option_ChildrenCleared() => Item.DropDownItems.Clear();
            private void Option_ChildAdded(ITheraMenuItem item) => AddItemToCollection(item, Item.DropDownItems);
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
        {
            IBasePathWrapper typeWrapper;
            if (path.IsExistingDirectoryPath() == true)
                typeWrapper = new FolderWrapper();
            else
            {
                string ext = Path.GetExtension(path);

                if (ext.Length > 0)
                    ext = ext.Substring(1).ToLowerInvariant();

                typeWrapper = TryWrap3rdPartyExt(ext);
                if (typeWrapper is null)
                {
                    TypeProxy type = !string.IsNullOrWhiteSpace(path) && File.Exists(path) ? TFileObject.DetermineType(path, out _) : null;
                    typeWrapper = TryWrapProprietaryType(type);
                    if (typeWrapper is null)
                        typeWrapper = new UnknownFileWrapper();
                }
            }
            typeWrapper.FilePath = path;
            return typeWrapper;
        }
        public static IBasePathWrapper TryWrap3rdPartyExt(string ext)
        {
            var wrappers = Editor.DomainProxy.ThirdPartyWrappers;

            if (wrappers.TryGetValue(ext, out TypeProxy wrapperType))
                return wrapperType.CreateInstance() as IBasePathWrapper;

            return null;
        }
        public static IBasePathWrapper TryWrapProprietaryType(TypeProxy type)
        {
            if (type is null)
                return null;

            IBasePathWrapper wrapper = null;

            //Try to find wrapper for type or any inherited type, in order
            var wrappers = Editor.DomainProxy.Wrappers;
            TypeProxy currentType = type;
            while (!(currentType is null) && wrapper is null)
            {
                if (wrappers.TryGetValue(currentType, out TypeProxy wrapperType))
                    wrapper = wrapperType.CreateInstance() as IBasePathWrapper;
                else
                {
                    TypeProxy[] interfaces = currentType.GetInterfaces();
                    var validInterfaces = interfaces.Where(interfaceType => wrappers.Keys.Any(wrapperKeyType => wrapperKeyType == interfaceType)).ToArray();
                    if (validInterfaces.Length > 0)
                    {
                        TypeProxy interfaceType;

                        //TODO: find best interface to use if multiple matches?
                        if (validInterfaces.Length > 1)
                        {
                            int[] numAssignableTo = validInterfaces.Select(match => validInterfaces.Count(other => other != match && other.IsAssignableTo(match))).ToArray();
                            int min = numAssignableTo.Min();
                            int[] mins = numAssignableTo.FindAllMatchIndices(x => x == min);
                            string msg = "File of type " + type.GetFriendlyName() + " has multiple valid interface wrappers: " + validInterfaces.ToStringList(", ", " and ", x => x.GetFriendlyName());
                            msg += ". Narrowed down wrappers to " + mins.Select(x => validInterfaces[x]).ToArray().ToStringList(", ", " and ", x => x.GetFriendlyName());
                            Engine.PrintLine(msg);
                            interfaceType = validInterfaces[mins[0]];
                        }
                        else
                            interfaceType = validInterfaces[0];

                        if (wrappers.TryGetValue(interfaceType, out wrapperType))
                            wrapper = wrapperType.CreateInstance() as IBasePathWrapper;
                    }
                }

                currentType = currentType.BaseType;
            }

            if (wrapper is null)
            {
                //Make wrapper for whatever file type this is
                wrapper = new FileWrapper() { FileType = type };
                //TypeProxy genericFileWrapper = TypeProxy.Get(typeof(FileWrapper<>)).MakeGenericType(t);
                //w = Activator.CreateInstance((Type)genericFileWrapper) as BaseFileWrapper;
            }

            return wrapper;
        }
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
