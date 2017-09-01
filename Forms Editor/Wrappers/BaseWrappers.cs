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
using System.ComponentModel;

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
        protected bool _isPopulated = false;

        public new ResourceTree TreeView => (ResourceTree)base.TreeView;
        public new BaseWrapper Parent => base.Parent as BaseWrapper; //Parent may be null
        public abstract string FilePath { get; set; }
        public bool IsPopulated => _isPopulated;

        public BaseWrapper(ContextMenuStrip menu)
        {
            ContextMenuStrip = menu;
        }

        protected static ResourceTree GetTree()
            => Editor.Instance.ContentTree;
        protected static T GetInstance<T>() where T : BaseWrapper
            => GetTree().SelectedNode as T;

        public void Rename()
        {
            if (!IsEditing && TreeView.LabelEdit)
                BeginEdit();
        }

        public void Cut() => SetClipboard(true);
        public void Copy() => SetClipboard(false);
        private void SetClipboard(bool cut)
        {
            string[] paths = new string[] { FilePath };//Directory.GetFileSystemEntries(path, "*.*", System.IO.SearchOption.TopDirectoryOnly);
            ResourceTree.SetClipboard(paths, cut);
        }
        public void Paste() => TreeView.Paste(FilePath);

        //public new void Remove()
        //{
        //    base.Remove();
        //}

        public static BaseWrapper Wrap(string path)
        {
            BaseWrapper w = null;
            bool? isDir = path.IsDirectory();
            if (isDir == null)
                return null;
            if (isDir.Value)
            {
                w = new FolderWrapper();
                if (Directory.GetFileSystemEntries(path).Length > 0)
                    w.Nodes.Add("...");
            }
            else
            {
                Type type = FileObject.DetermineType(path);
                if (type != null && NodeWrapperAttribute.Wrappers.ContainsKey(type))
                    w = Activator.CreateInstance(NodeWrapperAttribute.Wrappers[type]) as BaseWrapper;
                else //Make wrapper for whatever file type this is
                {
                    Type genericFileWrapper = typeof(FileWrapper<>).MakeGenericType(type);
                    w = Activator.CreateInstance(genericFileWrapper) as BaseFileWrapper;
                }
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
            if (NodeWrapperAttribute.Wrappers.ContainsKey(type))
                w = Activator.CreateInstance(NodeWrapperAttribute.Wrappers[type]) as BaseFileWrapper;
            else
            {
                Type genericFileWrapper = typeof(FileWrapper<>).MakeGenericType(type);
                w = Activator.CreateInstance(genericFileWrapper) as BaseFileWrapper;
            }

            FileObject.GetDirNameFmt(file.FilePath, out string dir, out string name, out FileFormat fmt);
            w.Text = name + "." + file.FileHeader.GetProperExtension(fmt);
            w.FileObject = file;
            return w;
        }

        internal protected abstract void OnExpand();
        internal protected abstract void HandlePathDrop(string path, bool copy);
        internal protected abstract void HandleNodeDrop(BaseWrapper node, bool copy);
    }
}
