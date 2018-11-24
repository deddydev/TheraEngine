using System.Windows.Forms;
using System.ComponentModel;
using System;
using TheraEngine.Core.Files;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using TheraEditor.Windows.Forms;
using System.Drawing;
using System.Threading;
using TheraEngine;
using TheraEngine.Core.Files.Serialization;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using TheraEditor.Core;

namespace TheraEditor.Wrappers
{
    public class FolderWrapper : BaseWrapper
    {
        #region Menu
        private static ContextMenuStrip _menu;
        public FolderWrapper(string path) : base(_menu)
        {
            Text = Path.GetFileName(path);
            FilePath = Name = path;
            ImageIndex = 1;
            SelectedImageIndex = 1;
        }
        static FolderWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.F2));                              //0
            _menu.Items.Add(new ToolStripMenuItem("&Open In Explorer", null, ExplorerAction, Keys.Control | Keys.E));   //1
            _menu.Items.Add(new ToolStripSeparator());                                                                  //2
            _menu.Items.Add(new ToolStripMenuItem("&Import File", null, null, Keys.Control | Keys.I));                  //3
            _menu.Items.Add(new ToolStripMenuItem("&New File", null, null, Keys.Control | Keys.N));                     //4
            _menu.Items.Add(new ToolStripMenuItem("New &Folder", null, FolderAction, Keys.Control | Keys.F));           //5
            _menu.Items.Add(new ToolStripSeparator());                                                                  //6
            _menu.Items.Add(new ToolStripMenuItem("Compile To &Archive", null, ArchiveAction, Keys.Control | Keys.A));  //7
            _menu.Items.Add(new ToolStripSeparator());                                                                  //8
            _menu.Items.Add(new ToolStripMenuItem("&Cut", null, CutAction, Keys.Control | Keys.X));                     //9
            _menu.Items.Add(new ToolStripMenuItem("&Copy", null, CopyAction, Keys.Control | Keys.C));                   //10
            _menu.Items.Add(new ToolStripMenuItem("&Paste", null, PasteAction, Keys.Control | Keys.V));                 //11
            _menu.Items.Add(new ToolStripMenuItem("&Delete", null, DeleteAction, Keys.Control | Keys.Delete));          //12
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;

            LoadFileTypes();
        }

        protected static void RenameAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Rename();
        protected static void DeleteAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Delete();
        protected static void CutAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Cut();
        protected static void CopyAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Copy();
        protected static void PasteAction(object sender, EventArgs e) => GetInstance<BaseWrapper>().Paste();

        protected static void FolderAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().NewFolder();
        protected static void ArchiveAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().ToArchive();
        protected static void ExplorerAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().OpenInExplorer();

        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            //_menu.Items[7].Enabled = _menu.Items[12].Enabled = true;
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            FolderWrapper w = GetInstance<FolderWrapper>();
            //_menu.Items[0].Enabled = w.TreeView.LabelEdit;
            _menu.Items[1].Enabled = !string.IsNullOrEmpty(w.FilePath) && Directory.Exists(w.FilePath);

            bool paste = false;
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                MemoryStream stream = (MemoryStream)data.GetData("Preferred DropEffect", true);
                int flag = stream.ReadByte();
                paste = flag == 2 || flag == 5;
            }
            _menu.Items[11].Enabled = paste;
            _menu.Items[9].Enabled = _menu.Items[12].Enabled = w.Parent != null;
        }
        #endregion

        public override string FilePath
        {
            get => Name;
            set => Name = value;
        }

        public void ToArchive()
        {

        }

        public void NewFolder()
        {
            string path = FilePath + "\\NewFolder";
            TreeView.WatchProjectDirectory = false;
            Directory.CreateDirectory(path);
            TreeView.WatchProjectDirectory = true;
            FolderWrapper b = Wrap(path) as FolderWrapper;
            Nodes.Add(b);
            TreeView.SelectedNode = b;
            b.EnsureVisible();
            b.Rename();
        }

        public override void Delete()
        {
            if (Parent == null)
                return;

            ResourceTree tree = TreeView;
            try
            {
                tree.WatchProjectDirectory = false;
                if (Directory.GetFileSystemEntries(FilePath).Length > 0)
                    FileSystem.DeleteDirectory(FilePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
                else
                    Directory.Delete(FilePath);
                Remove();
                ContextMenuStrip.Close();
            }
            catch (OperationCanceledException) { }
            catch (UnauthorizedAccessException)
            {
                Engine.PrintLine($"Can't delete {FilePath}. Unable to access.");
            }
            finally
            {
                tree.WatchProjectDirectory = true;
            }
        }

        public void CheckNodes()
        {

        }

        internal protected override void OnExpand()
        {
            if (!_isPopulated)
            {
                if (Nodes.Count > 0 && Nodes[0].Text == "...")
                {
                    Nodes.Clear();

                    string path = FilePath.ToString();
                    string[] subDirs = Directory.GetDirectories(path);
                    foreach (string subDir in subDirs)
                    {
                        DirectoryInfo subDirInfo = new DirectoryInfo(subDir);

                        BaseWrapper node = Wrap(subDir);
                        if (node == null)
                            continue;

                        try
                        {
                            //If the directory has sub folders/files, add the placeholder
                            if (subDirInfo.GetFileSystemInfos().Length > 0)
                                node.Nodes.Add(null, "...", 0, 0);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            //display a locked folder icon
                            node.ImageIndex = node.SelectedImageIndex = 3;
                        }
                        catch (Exception ex)
                        {
                            Engine.LogException(ex);
                        }
                        finally
                        {
                            Nodes.Add(node);
                        }
                    }

                    string[] files = Directory.GetFiles(path);
                    foreach (string file in files)
                    {
                        BaseWrapper node = Wrap(file);
                        if (node != null)
                        {
                            var tree = GetTree();
                            string key = tree.GetOrAddIcon(file);
                            node.ImageKey = node.SelectedImageKey = node.StateImageKey = key;
                            Nodes.Add(node);
                        }
                    }
                }
                _isPopulated = true;
            }

            if (Nodes.Count > 0)
            {
                ImageIndex = SelectedImageIndex = 2; //Open folder
            }
        }

        protected internal override void OnCollapse()
        {
            if (Nodes.Count > 0)
            {
                ImageIndex = SelectedImageIndex = 1; //Closed folder
            }
        }

        #region File Type Loading

        private static bool IsFileObject(Type t)
        {
            return 
                !t.IsAbstract && 
                !t.IsInterface &&
                t.IsSubclassOf(typeof(TFileObject)) &&
                t.GetCustomAttributeExt<TFileExt>() != null;
        }

        private static bool Is3rdPartyImportable(Type t)
        {
            if (!IsFileObject(t))
                return false;

            string[] ext = TFileObject.GetFile3rdPartyExtensions(t)?.ImportableExtensions;
            return ext != null && ext.Length > 0;
        }

        private static void LoadFileTypes()
        {
            Program.PopulateMenuDropDown((ToolStripDropDownItem)_menu.Items[3], OnImportClickAsync, Is3rdPartyImportable);
            Program.PopulateMenuDropDown((ToolStripDropDownItem)_menu.Items[4], OnNewClick, IsFileObject);
        }
        private static async void OnImportClickAsync(object sender, EventArgs e)
        {
            if (sender is ToolStripDropDownButton button)
            {
                Type fileType = button.Tag as Type;
                if (fileType.ContainsGenericParameters)
                {
                    using (GenericsSelector gs = new GenericsSelector(fileType))
                    {
                        if (gs.ShowDialog(button.Owner) == DialogResult.OK)
                            fileType = gs.FinalClassType;
                        else
                            return;
                    }
                }
                using (OpenFileDialog ofd = new OpenFileDialog()
                {
                    Filter = TFileObject.GetFilter(fileType, true, true, false),
                    Title = "Import File"
                })
                {
                    DialogResult r = ofd.ShowDialog(button.Owner);
                    if (r == DialogResult.OK)
                    {
                        string name = Path.GetFileNameWithoutExtension(ofd.FileName);
                        ResourceTree tree = Editor.Instance.ContentTree;
                        string path = ofd.FileName;
                        
                        int op = Editor.Instance.BeginOperation($"Importing '{path}'...", out Progress<float> progress, out CancellationTokenSource cancel);
                        object file = await TFileObject.LoadAsync(fileType, path, progress, cancel.Token);
                        Editor.Instance.EndOperation(op);

                        if (file == null)
                            return;
                        
                        FolderWrapper folderNode = GetInstance<FolderWrapper>();
                        string dir = folderNode.FilePath as string;
                        
                        if (TSerializer.PreExport(file, dir, name, EProprietaryFileFormat.XML, null, out string filePath))
                        {
                            TSerializer serializer = new TSerializer();

                            tree.BeginFileSaveWithProgress(filePath, $"Saving...", out progress, out cancel);
                            await serializer.SerializeXMLAsync(file, filePath, ESerializeFlags.Default, progress, cancel.Token);
                            tree.EndFileSave(filePath);
                        }
                    }
                }
            }
        }
        private static async void OnNewClick(object sender, EventArgs e)
        {
            if (!(sender is ToolStripDropDownButton button))
                return;
            
            Type fileType = button.Tag as Type;

            if (!(Editor.UserCreateInstanceOf(fileType, true, button.Owner) is TFileObject file))
                return;

            FolderWrapper folderNode = GetInstance<FolderWrapper>();
            string dir = folderNode.FilePath as string;

            //Node will automatically be added to the file tree
            if (TSerializer.PreExport(file, dir, file.Name, EProprietaryFileFormat.XML, null, out string path))
            {
                Editor.Instance.ContentTree.BeginFileSaveWithProgress(path, "Exporting...", 
                    out Progress<float> progress, out CancellationTokenSource cancel);
                await TSerializer.ExportXMLAsync(file, dir, file.Name, ESerializeFlags.Default, progress, cancel.Token);
                Editor.Instance.ContentTree.EndFileSave(path);
            }
        }
        #endregion
        
        public void OpenInExplorer()
        {
            string path = FilePath;
            if (!string.IsNullOrEmpty(path))
                Process.Start("explorer.exe", path);
        }
        
        public new string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                if (FilePath != null && FilePath.IsExistingDirectoryPath() == true)
                    FilePath = Path.GetDirectoryName(FilePath) + Path.DirectorySeparatorChar + value;
                else
                    FilePath = Path.DirectorySeparatorChar + value;
                if (_isPopulated)
                    foreach (BaseWrapper b in Nodes)
                        b.FixPath(FilePath + Path.DirectorySeparatorChar);
            }
        }
        protected internal override void FixPath(string parentFolderPath)
        {
            string folderName = Text;
            if (parentFolderPath != null && 
                parentFolderPath.Length > 0 && 
                parentFolderPath[parentFolderPath.Length - 1] != Path.DirectorySeparatorChar)
                parentFolderPath += Path.DirectorySeparatorChar;
            FilePath = parentFolderPath + folderName;
            if (_isPopulated)
                foreach (BaseWrapper b in Nodes)
                    b.FixPath(FilePath + Path.DirectorySeparatorChar);
        }
    }
}
