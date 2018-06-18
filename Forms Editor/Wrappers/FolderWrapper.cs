using System.Windows.Forms;
using System.ComponentModel;
using System;
using TheraEngine.Files;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using TheraEditor.Windows.Forms;
using System.Drawing;

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
            catch (OperationCanceledException e)
            {

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
                        BaseWrapper node = Wrap(file);
                        if (node != null)
                        {
                            var tree = GetTree();
                            string ext = Path.GetExtension(file).Substring(1).ToLowerInvariant();
                            if (!tree.ImageList.Images.ContainsKey(ext))
                            {
                                Icon iconForFile = Icon.ExtractAssociatedIcon(file);
                                tree.ImageList.Images.Add(ext, iconForFile);
                            }
                            node.ImageKey = node.SelectedImageKey = node.StateImageKey = ext;
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
            return !t.IsAbstract && t.IsSubclassOf(typeof(TFileObject));
        }

        private static bool Is3rdPartyImportable(Type t)
        {
            if (t.IsAbstract || !t.IsSubclassOf(typeof(TFileObject)))
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
                        if (gs.ShowDialog() == DialogResult.OK)
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
                    DialogResult r = ofd.ShowDialog(Editor.Instance);
                    if (r == DialogResult.OK)
                    {
                        TFileObject file = await TFileObject.LoadAsync(fileType, ofd.FileName);

                        FolderWrapper folderNode = GetInstance<FolderWrapper>();
                        string dir = folderNode.FilePath as string;

                        //folderNode.TreeView.WatchProjectDirectory = false;
                        file.Export(dir, file.Name, FileFormat.XML);
                        //folderNode.TreeView.WatchProjectDirectory = true;

                        //folderNode.Nodes.Add(Wrap(file));
                    }
                }
            }
        }
        private static void OnNewClick(object sender, EventArgs e)
        {
            if (sender is ToolStripDropDownButton button)
            {
                Type fileType = button.Tag as Type;
                TFileObject file = Editor.UserCreateInstanceOf(fileType, true) as TFileObject;

                if (file == null)
                    return;

                FolderWrapper folderNode = GetInstance<FolderWrapper>();
                string dir = folderNode.FilePath as string;

                //folderNode.TreeView.WatchProjectDirectory = false;
                file.Export(dir, file.Name);
                //folderNode.TreeView.WatchProjectDirectory = true;

                //folderNode.Nodes.Add(Wrap(file) as BaseWrapper);
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
                if (FilePath != null && FilePath.IsDirectoryPath() == true)
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
