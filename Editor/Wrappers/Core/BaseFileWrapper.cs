using AppDomainToolkit;
using Microsoft.VisualBasic.FileIO;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;
using WeifenLuo.WinFormsUI.Docking;

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
            strip.Items.Add(new ToolStripMenuItem("Edit", null, EditAction, Keys.F1));                                  //2
            strip.Items.Add(new ToolStripMenuItem("Edit Raw", null, EditRawAction, Keys.F3));                           //3
            strip.Items.Add(new ToolStripSeparator());                                                                  //4
            strip.Items.Add(new ToolStripMenuItem("&Export", null, ExportAction, Keys.Control | Keys.E));               //5
            strip.Items.Add(new ToolStripMenuItem("&Replace", null, ReplaceAction, Keys.Control | Keys.R));             //6
            strip.Items.Add(new ToolStripMenuItem("Re&load", null, RestoreAction, Keys.Control | Keys.L));              //7
            ToolStripMenuItem alwaysReload = new ToolStripMenuItem("Reload Automatically") { CheckOnClick = true };
            alwaysReload.CheckedChanged += AlwaysReload_CheckedChanged;
            strip.Items.Add(alwaysReload);                                                                              //8
            strip.Items.Add(new ToolStripSeparator());                                                                  //9
            strip.Items.Add(new ToolStripMenuItem("&Cut", null, CutAction, Keys.Control | Keys.X));                     //10
            strip.Items.Add(new ToolStripMenuItem("&Copy", null, CopyAction, Keys.Control | Keys.C));                   //11
            strip.Items.Add(new ToolStripMenuItem("&Paste", null, PasteAction, Keys.Control | Keys.V));                 //12
            strip.Items.Add(new ToolStripMenuItem("&Delete", null, DeleteAction, Keys.Control | Keys.Delete));          //13
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
        protected static void EditRawAction(object sender, EventArgs e) => GetInstance<BaseFileWrapper>().EditResourceRaw();
        
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
            //return;

            //var nodeWrappers = GetType().GetCustomAttributesExt<NodeWrapperAttribute>();
            //if (nodeWrappers.Length > 0)
            //{
            //    ImageIndex = ResourceTree.Images.Images.IndexOfKey(nodeWrappers[0].ImageName);
            //    SelectedImageIndex = ResourceTree.Images.Images.IndexOfKey(nodeWrappers[0].SelectedImageName);
            //}
            //else
            //    ImageIndex = SelectedImageIndex = 0;
        }

        public abstract TypeProxy FileType { get; }
        public abstract bool IsLoaded { get; }
        public bool AlwaysReload { get; set; } = false;
        public bool ExternallyModified { get; set; } = false;
        public abstract IFileObject SingleInstance { get; set; }
        public abstract IFileObject GetNewInstance();
        public abstract Task<IFileObject> GetNewInstanceAsync();
        public abstract IGlobalFileRef SingleInstanceRef { get; }
        
        public void Reload()
        {
            bool wasLoaded = SingleInstanceRef.IsLoaded;
            SingleInstanceRef.IsLoaded = false;
            if (wasLoaded)
                SingleInstanceRef.IsLoaded = true;
        }

        public virtual void EditResource()
        {
            IFileObject fileObj = RemoteFunc.Invoke(AppDomainHelper.GetGameAppDomain(), SingleInstanceRef, FileType, (gref, fileType) =>
            {
                Engine.PrintLine("Editing resource on AppDomain " + AppDomain.CurrentDomain.FriendlyName);
                if (gref == null)
                {
                    Engine.PrintLine("Cannot edit " + FilePath + ", reference is null.");
                    return null;
                }
                IFileObject f = gref.GetInstance();
                if (f == null)
                {
                    Engine.PrintLine("Cannot edit " + FilePath + ", instance is null.");
                    return null;
                }

                EngineDomainProxyEditor proxy = Engine.DomainProxy as EngineDomainProxyEditor;
                var full = proxy.FullEditorTypes;
                TypeProxy objType = typeof(object);

                while (!(fileType is null) && fileType != objType)
                {
                    if (full.ContainsKey(fileType))
                    {
                        var editorType = full[fileType];
                        Form form = editorType.CreateInstance<Form>(f);

                        if (form is DockContent dc && !(form is TheraForm))
                            dc.Show(Editor.Instance.DockPanel, DockState.Document);
                        else
                            form?.ShowDialog(Editor.Instance);

                        return null;
                    }

                    TypeProxy[] interfaces = fileType.GetInterfaces();
                    foreach (TypeProxy intfType in interfaces)
                    {
                        if (full.ContainsKey(intfType))
                        {
                            var editorType = full[intfType];
                            Form form = editorType.CreateInstance<Form>(f);

                            if (form is DockContent dc && !(form is TheraForm))
                                dc.Show(Editor.Instance.DockPanel, DockState.Document);
                            else
                                form?.ShowDialog(Editor.Instance);

                            return null;
                        }
                    }

                    fileType = fileType.BaseType;
                }
                return f;
            });

            Editor.SetPropertyGridObject(fileObj);
        }
        public virtual async void EditResourceRaw()
        {
            TextFile file = await TFileObject.LoadAsync<TextFile>(FilePath);
            if (file != null)
                DockableTextEditor.ShowNew(Editor.Instance.DockPanel, DockState.Document, file);
        }
        
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
            catch
            {

            }
            finally
            {
                tree.WatchProjectDirectory = true;
            }
        }
    }
}
