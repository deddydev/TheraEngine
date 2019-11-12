using Extensions;
using Microsoft.VisualBasic.FileIO;
using System;
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
        protected bool _isPopulated = false;

        public new ResourceTree TreeView => (ResourceTree)base.TreeView;
        public new ContentTreeNode Parent => base.Parent as ContentTreeNode; //Parent may be null
        /// <summary>
        /// The path of this node.
        /// </summary>
        public virtual string FilePath
        {
            get => Name;
            set => Name = value;
        }
        public bool IsPopulated => _isPopulated;

        public ContentTreeNode(ContextMenuStrip menu)
        {
            ContextMenuStrip = menu;
            ContextMenuStrip.RenderMode = ToolStripRenderMode.Professional;
            ContextMenuStrip.Renderer = new TheraForm.TheraToolstripRenderer();
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
            ContentTreeNode wrapper = null;
            bool? isDir = path.IsExistingDirectoryPath();
            if (isDir is null)
                return null;
            if (isDir.Value)
            {
                try
                {
                    wrapper = new FolderTreeNode(path);
                    if (Directory.GetFileSystemEntries(path).Length > 0)
                        wrapper.Nodes.Add("...");
                }
                catch { }
            }
            else
            {
                string ext = Path.GetExtension(path);
                if (!string.IsNullOrWhiteSpace(ext))
                {
                    ext = ext.Substring(1).ToLowerInvariant();
                    if (Editor.DomainProxy.ThirdPartyWrappers.TryGetValue(ext, out TypeProxy value))
                    {
                        Type type = (Type)value;
                        wrapper = Activator.CreateInstance(type) as ContentTreeNode;

                        wrapper.Text = Path.GetFileName(path);
                        wrapper.FilePath = wrapper.Name = path;

                        return wrapper;
                    }
                }

                TypeProxy t = TFileObject.DetermineType(path, out _);
                //Engine.PrintLine(t.Domain.FriendlyName);
                wrapper = TryWrapType(t);
                if (wrapper != null)
                {
                    wrapper.Text = Path.GetFileName(path);
                    wrapper.FilePath = wrapper.Name = path;
                }
                else
                    wrapper = new DefaultFileWrapper(path);
            }
            return wrapper;
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
        public static FileTreeNode TryWrapType(TypeProxy type)
        {
            if (type is null)
                return null;

            FileTreeNode wrapper = null;

            //Try to find wrapper for type or any inherited type, in order
            var wrappers = Editor.DomainProxy.Wrappers;
            TypeProxy currentType = type;
            while (!(currentType is null) && wrapper is null)
            {
                if (wrappers.TryGetValue(currentType, out TypeProxy matchType))
                {
                    Type wrapperType = (Type)wrappers[currentType];
                    wrapper = Activator.CreateInstance(wrapperType) as FileTreeNode;
                }
                else
                {
                    TypeProxy[] interfaces = type.GetInterfaces();
                    var validInterfaces = interfaces.Where(interfaceType => wrappers.Keys.Any(wrapperKeyType => wrapperKeyType == interfaceType)).ToArray();
                    if (validInterfaces.Length > 0)
                    {
                        TypeProxy interfaceType;

                        //TODO: find best interface to use if multiple matches?
                        if (validInterfaces.Length > 1)
                        {
                            var counts = validInterfaces.Select(inf => validInterfaces.Count(v => inf.IsAssignableFrom(v))).ToArray();
                            int min = counts.Min();
                            int[] mins = counts.FindAllMatchIndices(x => x == min);
                            string msg = "File of type " + type.GetFriendlyName() + " has multiple valid interface wrappers: " + validInterfaces.ToStringList(", ", " and ", x => x.GetFriendlyName());
                            msg += ". Narrowed down wrappers to " + mins.Select(x => validInterfaces[x]).ToArray().ToStringList(", ", " and ", x => x.GetFriendlyName());
                            Engine.PrintLine(msg);
                            interfaceType = validInterfaces[mins[0]];
                        }
                        else
                            interfaceType = validInterfaces[0];

                        if (wrappers.TryGetValue(interfaceType, out matchType))
                        {
                            Type wrapperType = (Type)matchType;
                            wrapper = Activator.CreateInstance(wrapperType) as FileTreeNode;
                        }
                    }
                }

                currentType = currentType.BaseType;
            }

            if (wrapper is null)
            {
                //Make wrapper for whatever file type this is
                wrapper = new FileWrapper(type);
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
