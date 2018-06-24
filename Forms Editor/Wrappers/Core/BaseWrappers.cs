using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine.Files;

namespace TheraEditor.Wrappers
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class NodeWrapperAttribute : Attribute
    {
        public NodeWrapperAttribute(Type type, string imageName)
        {
            FileType = type;
            ImageName = SelectedImageName = imageName;
        }
        public NodeWrapperAttribute(Type type, string imageName, string selectedImageName)
        {
            FileType = type;
            ImageName = imageName;
            SelectedImageName = selectedImageName;
        }
        public NodeWrapperAttribute(string thirdPartyExtension)
        {
            ThirdPartyExtension = thirdPartyExtension;
        }

        public Type FileType { get; }
        public string ImageName { get; }
        public string SelectedImageName { get; }
        public string ThirdPartyExtension { get; }

        /// <summary>
        /// Key is file type, Value is tree node wrapper type
        /// </summary>
        public static Dictionary<Type, Type> Wrappers
        {
            get
            {
                if (_wrappers == null)
                {
                    LoadWrappers(Assembly.GetExecutingAssembly());
                }
                return _wrappers;
            }
        }
        public static Dictionary<string, Type> ThirdPartyWrappers
        {
            get
            {
                if (_thirdPartyWrappers == null)
                {
                    LoadWrappers(Assembly.GetExecutingAssembly());
                }
                return _thirdPartyWrappers;
            }
        }

        public static void LoadWrappers(Assembly assembly)
        {
            _wrappers = new Dictionary<Type, Type>();
            _thirdPartyWrappers = new Dictionary<string, Type>();
            if (assembly != null)
                foreach (Type asmType in assembly.GetTypes())
                    foreach (NodeWrapperAttribute attr in asmType.GetCustomAttributes(typeof(NodeWrapperAttribute), true))
                    {
                        if (!string.IsNullOrWhiteSpace(attr.ThirdPartyExtension))
                            _thirdPartyWrappers[attr.ThirdPartyExtension] = asmType;
                        else
                            _wrappers[attr.FileType] = asmType;
                    }
        }
        
        private static Dictionary<Type, Type> _wrappers;
        private static Dictionary<string, Type> _thirdPartyWrappers;
    }
    public abstract class BaseWrapper : TreeNode
    {
        protected bool _isPopulated = false;

        public new ResourceTree TreeView => (ResourceTree)base.TreeView;
        public new BaseWrapper Parent => base.Parent as BaseWrapper; //Parent may be null
        /// <summary>
        /// The path of this node.
        /// </summary>
        public virtual string FilePath { get => Name; set => Name = value; }
        public bool IsPopulated => _isPopulated;

        public BaseWrapper(ContextMenuStrip menu)
        {
            ContextMenuStrip = menu;
            ContextMenuStrip.RenderMode = ToolStripRenderMode.Professional;
            ContextMenuStrip.Renderer = new TheraForm.TheraToolstripRenderer();
        }
        
        protected static ResourceTree GetTree()
            => Editor.Instance.ContentTree;
        protected static T GetInstance<T>() where T : BaseWrapper
            => GetTree().SelectedNode as T;

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
        
        public static BaseWrapper Wrap(string path)
        {
            BaseWrapper w = null;
            bool? isDir = path.IsDirectoryPath();
            if (isDir == null)
                return null;
            if (isDir.Value)
            {
                w = new FolderWrapper(path);
                if (Directory.GetFileSystemEntries(path).Length > 0)
                    w.Nodes.Add("...");
            }
            else
            {
                string ext = Path.GetExtension(path).Substring(1).ToLowerInvariant();
                if (NodeWrapperAttribute.ThirdPartyWrappers.ContainsKey(ext))
                {
                    w = Activator.CreateInstance(NodeWrapperAttribute.ThirdPartyWrappers[ext]) as BaseWrapper;

                    w.Text = Path.GetFileName(path);
                    w.FilePath = w.Name = path;
                }
                else
                {
                    Type mainType = TFileObject.DetermineType(path);
                    if (mainType != null)
                    {
                        //Try to find wrapper for type or any inherited type, in order
                        Type tempType = mainType;
                        while (tempType != null && w == null)
                        {
                            if (NodeWrapperAttribute.Wrappers.ContainsKey(tempType))
                                w = Activator.CreateInstance(NodeWrapperAttribute.Wrappers[tempType]) as BaseWrapper;
                            else
                                tempType = tempType.BaseType;
                        }

                        if (w == null)
                        {
                            //Make wrapper for whatever file type this is
                            Type genericFileWrapper = typeof(FileWrapper<>).MakeGenericType(mainType);
                            w = Activator.CreateInstance(genericFileWrapper) as BaseFileWrapper;
                        }

                        w.Text = Path.GetFileName(path);
                        w.FilePath = w.Name = path;
                    }
                    else
                    {
                        w = new GenericFileWrapper(path);
                    }
                }
            }
            return w;
        }
        public static BaseFileWrapper Wrap(TFileObject file)
        {
            if (file == null)
                return null;

            BaseFileWrapper w = null;

            Type type = file.GetType();
            if (NodeWrapperAttribute.Wrappers.ContainsKey(type))
                w = Activator.CreateInstance(NodeWrapperAttribute.Wrappers[type]) as BaseFileWrapper;
            else
            {
                Type genericFileWrapper = typeof(FileWrapper<>).MakeGenericType(type);
                w = Activator.CreateInstance(genericFileWrapper) as BaseFileWrapper;
            }
            TFileObject.GetDirNameFmt(file.FilePath, out string dir, out string name, out FileFormat fmt, out string thirdPartyExt);
            w.Text = name + "." + file.FileExtension.GetProperExtension((ProprietaryFileFormat)fmt);
            w.SingleInstance = file;
            //w.SelectedImageIndex = w.ImageIndex = 0;
            return w;
        }

        internal protected abstract void OnExpand();
        internal protected abstract void OnCollapse();
        internal protected abstract void FixPath(string parentFolderPath);

        internal void HandlePathDrop(string path, bool copy)
        {
            bool? isDir = path.IsDirectoryPath();
            if (isDir == null)
                return;
            string newPath = this is BaseFileWrapper ? Path.GetDirectoryName(FilePath) : FilePath;
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
                    newPath += Path.GetFileName(path);
                    if (copy)
                        FileSystem.CopyFile(path, newPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                    else
                        FileSystem.MoveFile(path, newPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                }
            }
            catch (OperationCanceledException e)
            {
                return;
            }

            BaseWrapper child = Wrap(newPath);
            if (child != null)
                Nodes.Add(child);
            else
                throw new Exception();
        }
        internal void HandleNodeDrop(BaseWrapper node, bool copy)
        {
            bool isFileNode = this is BaseFileWrapper;
            string destPath = isFileNode ? Path.GetDirectoryName(FilePath) : FilePath;

            if (string.IsNullOrEmpty(destPath))
                return;

            if (!destPath.EndsWith("\\"))
                destPath += "\\";

            string fileName = Path.GetFileName(node.FilePath);
            destPath += fileName;

            if (string.Equals(node.FilePath, destPath, StringComparison.InvariantCulture))
                return;

            if (node is BaseFileWrapper fileNode)
            {
                try
                {
                    if (copy)
                        FileSystem.CopyFile(fileNode.FilePath, destPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                    else
                        FileSystem.MoveFile(fileNode.FilePath, destPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                }
                catch (OperationCanceledException e) { return; }
            }
            else if (node is FolderWrapper folderNode)
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
                catch (OperationCanceledException e) { return; }
            }

            if (!isFileNode && !IsPopulated && Nodes.Count == 0)
            {
                if (Directory.GetFileSystemEntries(FilePath).Length > 0)
                    Nodes.Add("...");
            }
        }
    }
}
