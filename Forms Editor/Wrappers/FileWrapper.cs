using System;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using TheraEngine.Files;
using Microsoft.VisualBasic.FileIO;
using TheraEditor.Windows.Forms;
using System.Diagnostics;
using TheraEngine;

namespace TheraEditor.Wrappers
{
    public abstract class BaseFileWrapper : BaseWrapper
    {
        #region Menu

        static BaseFileWrapper()
        {
            _defaultMenu = new ContextMenuStrip();
            FillContextMenuDefaults(_defaultMenu);
        }

        private static ContextMenuStrip _defaultMenu;
        public static int FillContextMenuDefaults(ContextMenuStrip strip)
        {
            strip.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.F2));                              //0
            strip.Items.Add(new ToolStripMenuItem("&Open In Explorer", null, ExplorerAction, Keys.Control | Keys.O));   //1
            strip.Items.Add(new ToolStripMenuItem("Edit File", null, EditAction, Keys.F1));                             //2
            strip.Items.Add(new ToolStripSeparator());                                                                  //3
            strip.Items.Add(new ToolStripMenuItem("&Export", null, ExportAction, Keys.Control | Keys.E));               //4
            strip.Items.Add(new ToolStripMenuItem("&Replace", null, ReplaceAction, Keys.Control | Keys.R));             //5
            strip.Items.Add(new ToolStripMenuItem("Re&load", null, RestoreAction, Keys.Control | Keys.L));              //6
            ToolStripMenuItem alwaysReload = new ToolStripMenuItem("Reload Automatically") { CheckOnClick = true };
            alwaysReload.CheckedChanged += AlwaysReload_CheckedChanged;
            strip.Items.Add(alwaysReload); //7
            strip.Items.Add(new ToolStripSeparator());                                                                  //8
            strip.Items.Add(new ToolStripMenuItem("&Cut", null, CutAction, Keys.Control | Keys.X));                     //9
            strip.Items.Add(new ToolStripMenuItem("&Copy", null, CopyAction, Keys.Control | Keys.C));                   //10
            strip.Items.Add(new ToolStripMenuItem("&Paste", null, PasteAction, Keys.Control | Keys.V));                 //11
            strip.Items.Add(new ToolStripMenuItem("&Delete", null, DeleteAction, Keys.Control | Keys.Delete));          //12
            strip.Opening += MenuOpening;
            strip.Closing += MenuClosing;
            return strip.Items.Count;
        }
        private static void AlwaysReload_CheckedChanged(object sender, EventArgs e)
        {
            GetInstance<BaseFileWrapper>().AlwaysReload = ((ToolStripMenuItem)sender).Checked;
        }

        protected static void DeleteAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Delete();
        protected static void RenameAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Rename();
        protected static void CutAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Cut();
        protected static void CopyAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Copy();
        protected static void PasteAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Paste();

        protected static void ExportAction(object sender, EventArgs e) { GetInstance<BaseFileWrapper>().Export(); }
        protected static void ReplaceAction(object sender, EventArgs e) { GetInstance<BaseFileWrapper>().Replace(); }
        protected static void RestoreAction(object sender, EventArgs e) { GetInstance<BaseFileWrapper>().Restore(); }
        protected static void EditExternalAction(object sender, EventArgs e) { GetInstance<BaseFileWrapper>().OpenInExplorer(true); }
        protected static void ExplorerAction(object sender, EventArgs e) => GetInstance<BaseFileWrapper>().OpenInExplorer(false);
        protected static void EditAction(object sender, EventArgs e) => GetInstance<BaseFileWrapper>().EditResource();

        public void Export() { }
        public void Replace() { }
        public void Restore() { }

        public void OpenInExplorer(bool editFileExternally)
        {
            string path = FilePath;
            if (string.IsNullOrEmpty(path))
                return;
            if (editFileExternally)
                Process.Start(path);
            else
            {
                string dir = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(dir))
                    return;
                Process.Start("explorer.exe", dir);
            }
        }

        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            //_menu.Items[1].Enabled = _menu.Items[2].Enabled = _menu.Items[4].Enabled = _menu.Items[5].Enabled = _menu.Items[8].Enabled = true;
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            BaseFileWrapper w = GetInstance<BaseFileWrapper>();
            //_menu.Items[0].Enabled = w.TreeView.LabelEdit;
            _defaultMenu.Items[1].Enabled = !string.IsNullOrEmpty(w.FilePath) && File.Exists(w.FilePath);
            _defaultMenu.Items[5].Enabled = _defaultMenu.Items[8].Enabled = w.Parent != null;
            _defaultMenu.Items[6].Enabled = w.IsLoaded && w.SingleInstance.EditorState.HasChanges;
            ((ToolStripMenuItem)_defaultMenu.Items[7]).Checked = w.AlwaysReload;
            //_menu.Items[2].Enabled = ((w._resource.IsDirty) || (w._resource.IsBranch));
            //_menu.Items[4].Enabled = w.PrevNode != null;
            //_menu.Items[5].Enabled = w.NextNode != null;
        }
        #endregion

        public BaseFileWrapper() : this(_defaultMenu) { }
        public BaseFileWrapper(ContextMenuStrip menu) : base(menu)
        {
            var nodeWrappers = GetType().GetCustomAttributesExt<NodeWrapperAttribute>();
            if (nodeWrappers.Length > 0)
            {
                ImageIndex = ResourceTree.Images.Images.IndexOfKey(nodeWrappers[0].ImageName);
                SelectedImageIndex = ResourceTree.Images.Images.IndexOfKey(nodeWrappers[0].SelectedImageName);
            }
            else
                ImageIndex = SelectedImageIndex = 0;
        }

        public abstract Type FileType { get; }
        public abstract bool IsLoaded { get; }
        public bool AlwaysReload { get; set; } = false;
        public bool ExternallyModified { get; set; } = false;
        public abstract TFileObject SingleInstance { get; set; }
        public abstract TFileObject GetNewInstance();
        public abstract IGlobalFileRef SingleInstanceRef { get; }
        
        public void Reload()
        {

        }

        public virtual void EditResource()
            => Editor.SetPropertyGridObject(SingleInstance);
        
        internal protected override void OnExpand()
        {

        }
        protected internal override void OnCollapse()
        {

        }
        public override void Delete()
        {
            if (Parent == null)
                return;

            ResourceTree tree = TreeView;
            try
            {
                tree.WatchProjectDirectory = false;
                if (new FileInfo(FilePath).Length > 0)
                    FileSystem.DeleteFile(FilePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
                else
                    File.Delete(FilePath);
                Remove();
            }
            catch (OperationCanceledException e)
            {

            }
            finally
            {
                tree.WatchProjectDirectory = true;
            }
        }
    }
    public class GenericFileWrapper : BaseFileWrapper
    {
        public GenericFileWrapper(string path) : base()
        {
            Text = Path.GetFileName(path);
            FilePath = Name = path;
            Engine.PrintLine("File type not identified: " + path);
        }
        public override TFileObject SingleInstance { get => null; set { } }
        public override bool IsLoaded => false;
        public override Type FileType => null;
        public override IGlobalFileRef SingleInstanceRef => throw new NotImplementedException();
        public override TFileObject GetNewInstance() { return null; }
        protected internal override void FixPath(string parentFolderPath) { }

        //Let Windows decide how the file should be edited
        public override void EditResource() => Process.Start(FilePath);
    }
    public class FileWrapper<T> : BaseFileWrapper where T : TFileObject
    {
        public FileWrapper() : base() { }
        public FileWrapper(ContextMenuStrip menu) : base(menu) { }

        public override Type FileType => typeof(T);

        protected GlobalFileRef<T> _fileRef = new GlobalFileRef<T>();

        public T Resource
        {
            get => ResourceRef.File;
            set => ResourceRef.File = value;
        }
        public GlobalFileRef<T> ResourceRef
        {
            get => _fileRef;
            set => _fileRef = value ?? new GlobalFileRef<T>();
        }

        public override bool IsLoaded => ResourceRef.IsLoaded;
        public override string FilePath
        {
            get => ResourceRef.ReferencePathAbsolute;
            set
            {
                ResourceRef.ReferencePathAbsolute = value;
                Name = value;
            }
        }
        public override TFileObject GetNewInstance()
            => _fileRef.LoadNewInstance(true, null, null);
        public override TFileObject SingleInstance
        {
            get => ResourceRef.File;
            set
            {
                T obj = value as T;
                ResourceRef.File = obj;
                Name = FilePath;
            }
        }
        public override IGlobalFileRef SingleInstanceRef => ResourceRef;
        protected internal override void FixPath(string parentFolderPath)
        {
            string fileName = Text;
            if (parentFolderPath[parentFolderPath.Length - 1] != Path.DirectorySeparatorChar)
                parentFolderPath += Path.DirectorySeparatorChar;
            FilePath = parentFolderPath + fileName;
        }
        protected void DefaultSaveText(DockableTextEditor obj)
        {
            ITextSource source = Resource as ITextSource;
            if (source == null)
                return;
            source.Text = obj.GetText();
            Editor.Instance.ContentTree.WatchProjectDirectory = false;
            ResourceRef.ExportReference();
            Editor.Instance.ContentTree.WatchProjectDirectory = true;
        }
    }
}
