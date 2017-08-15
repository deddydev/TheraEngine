using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using TheraEngine.Files;
using System.IO;
using System.Linq;
using System.Diagnostics;
using TheraEditor.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

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
        protected bool _discovered = false;

        public new ResourceTree TreeView => (ResourceTree)base.TreeView;
        public new BaseWrapper Parent => (BaseWrapper)base.Parent;
        public abstract string FilePath { get; set; }
        
        public BaseWrapper(ContextMenuStrip menu)
        {
            menu.Tag = this;
            ContextMenuStrip = menu;
        }

        protected static ResourceTree GetTree()
            => Editor.Instance.ContentTree;
        protected static T GetInstance<T>() where T : BaseWrapper
            => GetTree().SelectedNode as T;

        public void OpenInExplorer()
        {
            if (!string.IsNullOrEmpty(FilePath))
                Process.Start("explorer.exe", FilePath);
        }

        public void Cut() => SetClipboard(true);
        public void Copy() => SetClipboard(false);
        private void SetClipboard(bool cut)
        {
            string path = FilePath;
            bool? dir = path.IsDirectory();
            if (dir == null)
                return;
            string[] paths = Directory.GetFileSystemEntries(path, "*.*", System.IO.SearchOption.TopDirectoryOnly);
            ResourceTree.SetClipboard(paths, cut);
        }
        public void Paste() => ResourceTree.Paste(FilePath);
        
        internal protected virtual void OnExpand()
        {
            if (!_discovered)
            {
                if (Nodes.Count > 0 && Nodes[0].Text == "..." && Nodes[0].Tag == null)
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
                    w = Activator.CreateInstance(typeof(FileWrapper<FileObject>)) as BaseFileWrapper;
            }
            w.Text = Path.GetFileName(path);
            w.FilePath = w.Name = path;
            return w;
        }
        public static BaseFileWrapper Wrap(FileObject file)
        {
            if (file == null)
                return null;

            BaseFileWrapper w = null;

            Type type = file.GetType();
            if (type != null && NodeWrapperAttribute.Wrappers.ContainsKey(type))
                w = Activator.CreateInstance(NodeWrapperAttribute.Wrappers[type]) as BaseFileWrapper;
            else
                w = Activator.CreateInstance(typeof(FileWrapper<FileObject>)) as BaseFileWrapper;

            FileObject.GetDirNameFmt(file.FilePath, out string dir, out string name, out FileFormat fmt);
            w.Text = name + "." + file.FileHeader.GetProperExtension(fmt);
            w.Name = file.FilePath;
            w.FileObject = file;
            return w;
        }
    }
}
