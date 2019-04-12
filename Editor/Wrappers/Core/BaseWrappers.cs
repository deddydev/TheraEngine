using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Wrappers
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class NodeWrapperAttribute : Attribute
    {
        public NodeWrapperAttribute(string imageName, string selectedImageName)
        {
            ImageName = imageName;
            SelectedImageName = selectedImageName;
        }
        public NodeWrapperAttribute(string thirdPartyExtension)
        {
            ThirdPartyExtension = thirdPartyExtension;
        }

        //public Type FileType { get; }
        public string ImageName { get; }
        public string SelectedImageName { get; }
        public string ThirdPartyExtension { get; }

        /// <summary>
        /// Key is file type, Value is tree node wrapper type
        /// </summary>
        public static Dictionary<TypeProxy, TypeProxy> Wrappers { get; private set; }
        public static Dictionary<string, TypeProxy> ThirdPartyWrappers { get; private set; }

        static NodeWrapperAttribute()
        {
            //Load all nodewrapper types from editor assembly
            //Supports loading nodewrappers from multiple assemblies for the future possibility of file type plugins
            LoadWrappers(Assembly.GetExecutingAssembly());
        }
        public static void LoadWrappers(params Assembly[] assemblies)
        {
            Wrappers = new Dictionary<TypeProxy, TypeProxy>(new TypeProxy.EqualityComparer());
            ThirdPartyWrappers = new Dictionary<string, TypeProxy>();

            if (assemblies != null)
                foreach (Assembly asm in assemblies)
                    foreach (Type asmType in asm.GetTypes())
                        foreach (NodeWrapperAttribute attr in asmType.GetCustomAttributes(typeof(NodeWrapperAttribute), true))
                        {
                            if (asmType.BaseType == typeof(ThirdPartyFileWrapper))
                            {
                                if (!string.IsNullOrWhiteSpace(attr.ThirdPartyExtension))
                                    ThirdPartyWrappers[attr.ThirdPartyExtension] = asmType;
                            }
                            else if (asmType.BaseType.IsGenericType && asmType.BaseType.GetGenericTypeDefinition() == typeof(FileWrapper<>))
                            {
                                Type fileType = asmType.BaseType.GetGenericArguments()[0];
                                Wrappers[fileType] = asmType;
                            }
                            else
                            {
                                throw new InvalidOperationException($"{nameof(NodeWrapperAttribute)} must be an attribute on a class that inherits directly from {nameof(FileWrapper<IFileObject>)} or {nameof(ThirdPartyFileWrapper)}.");
                            }
                        }
        }
    }
    public abstract class BaseWrapper : TreeNode
    {
        protected bool _isPopulated = false;

        public new ResourceTree TreeView => (ResourceTree)base.TreeView;
        public new BaseWrapper Parent => base.Parent as BaseWrapper; //Parent may be null
        /// <summary>
        /// The path of this node.
        /// </summary>
        public virtual string FilePath
        {
            get => Name;
            set => Name = value;
        }
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
            bool? isDir = path.IsExistingDirectoryPath();
            if (isDir == null)
                return null;
            if (isDir.Value)
            {
                try
                {
                    w = new FolderWrapper(path);
                    if (Directory.GetFileSystemEntries(path).Length > 0)
                        w.Nodes.Add("...");
                }
                catch { }
            }
            else
            {
                string ext = Path.GetExtension(path);
                if (!string.IsNullOrWhiteSpace(ext))
                {
                    ext = ext.Substring(1).ToLowerInvariant();
                    if (NodeWrapperAttribute.ThirdPartyWrappers.ContainsKey(ext))
                    {
                        w = NodeWrapperAttribute.ThirdPartyWrappers[ext]?.CreateInstance() as BaseWrapper;

                        w.Text = Path.GetFileName(path);
                        w.FilePath = w.Name = path;
                        return w;
                    }
                }

                TypeProxy t = TFileObject.DetermineType(path, out _);
                w = TryWrapType(t);
                if (w != null)
                {
                    w.Text = Path.GetFileName(path);
                    w.FilePath = w.Name = path;
                }
                else
                    w = new GenericFileWrapper(path);
            }
            return w;
        }
        public static BaseFileWrapper Wrap(TFileObject file)
        {
            BaseFileWrapper w = TryWrapType(file?.GetType());
            if (w != null)
            {
                TFileObject.GetDirNameFmt(file.FilePath, out string dir, out string name, out EFileFormat fmt, out string thirdPartyExt);
                w.Text = name + "." + file.FileExtension.GetFullExtension((EProprietaryFileFormat)fmt);
                w.SingleInstance = file;
                //w.SelectedImageIndex = w.ImageIndex = 0;
            }
            return w;
        }
        public static BaseFileWrapper TryWrapType(TypeProxy t)
        {
            BaseFileWrapper w = null;
            if (t != null)
            {
                //Try to find wrapper for type or any inherited type, in order
                TypeProxy tempType = t;
                while (tempType != null && w == null)
                {
                    if (NodeWrapperAttribute.Wrappers.ContainsKey(tempType))
                        w = NodeWrapperAttribute.Wrappers[tempType]?.CreateInstance() as BaseFileWrapper;
                    else
                    {
                        TypeProxy[] interfaces = t.GetInterfaces();
                        var validInterfaces = interfaces.Where(interfaceType => NodeWrapperAttribute.Wrappers.Keys.Any(wrapperKeyType => wrapperKeyType == interfaceType)).ToArray();
                        if (validInterfaces.Length > 0)
                        {
                            TypeProxy match;

                            //TODO: find best interface to use if multiple matches?
                            if (validInterfaces.Length > 1)
                            {
                                var counts = validInterfaces.Select(inf => validInterfaces.Count(v => inf.IsAssignableFrom(v))).ToArray();
                                int min = counts.Min();
                                int[] mins = counts.FindAllMatchIndices(x => x == min);
                                string msg = "File of type " + t.GetFriendlyName() + " has multiple valid interface wrappers: " + validInterfaces.ToStringList(", ", " and ", x => x.GetFriendlyName());
                                msg += ". Narrowed down wrappers to " + mins.Select(x => validInterfaces[x]).ToArray().ToStringList(", ", " and ", x => x.GetFriendlyName());
                                Engine.PrintLine(msg);
                                match = validInterfaces[mins[0]];
                            }
                            else
                                match = validInterfaces[0];
                            
                            w = NodeWrapperAttribute.Wrappers[match]?.CreateInstance() as BaseFileWrapper;
                        }
                        else
                            tempType = tempType.BaseType;
                    }
                }

                if (w == null)
                {
                    //Make wrapper for whatever file type this is
                    TypeProxy genericFileWrapper = TypeProxy.Get(typeof(FileWrapper<>)).MakeGenericType(t);
                    w = genericFileWrapper.CreateInstance() as BaseFileWrapper;
                }
            }
            return w;
        }
        internal protected abstract void OnExpand();
        internal protected abstract void OnCollapse();
        internal protected abstract void FixPath(string parentFolderPath);

        internal void HandlePathDrop(string path, bool copy)
        {
            bool? isDir = path.IsExistingDirectoryPath();
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
                    newPath += Path.DirectorySeparatorChar + Path.GetFileName(path);
                    if (copy)
                        FileSystem.CopyFile(path, newPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                    else
                        FileSystem.MoveFile(path, newPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                }
            }
            catch { return; }

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
                catch { return; }
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
