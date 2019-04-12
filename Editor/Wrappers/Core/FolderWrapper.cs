using Microsoft.VisualBasic.FileIO;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection;
using TheraEngine.Scripting;

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
        public static void LoadFileTypes()
        {
            if (_menu.InvokeRequired)
            {
                _menu.Invoke((Action)LoadFileTypes);
                return;
            }

            ToolStripDropDownItem importDropdown = (ToolStripDropDownItem)_menu.Items[3];
            ToolStripDropDownItem newDropdown = (ToolStripDropDownItem)_menu.Items[4];

            importDropdown.DropDownItems.Clear();
            newDropdown.DropDownItems.Clear();

            ToolStripMenuItem newCodeItem = new ToolStripMenuItem("C#");
            newCodeItem.DropDownItems.Add(new ToolStripMenuItem("Class", null, NewClassAction));
            newCodeItem.DropDownItems.Add(new ToolStripMenuItem("Struct", null, NewStructAction));
            newCodeItem.DropDownItems.Add(new ToolStripMenuItem("Interface", null, NewInterfaceAction));
            newCodeItem.DropDownItems.Add(new ToolStripMenuItem("Enum", null, NewEnumAction));
            newDropdown.DropDownItems.Add(newCodeItem);

            Program.PopulateMenuDropDown(importDropdown, OnImportClickAsync, Is3rdPartyImportable);
            Program.PopulateMenuDropDown(newDropdown, OnNewClick, IsFileObject);
        }
        
        private enum ECodeFileType
        {
            Class,
            Struct,
            Interface,
            Enum,
        }
        protected static void NewClassAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().NewCodeFile(ECodeFileType.Class);
        protected static void NewStructAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().NewCodeFile(ECodeFileType.Struct);
        protected static void NewInterfaceAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().NewCodeFile(ECodeFileType.Interface);
        protected static void NewEnumAction(object sender, EventArgs e) => GetInstance<FolderWrapper>().NewCodeFile(ECodeFileType.Enum);

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

        protected internal override void OnExpand()
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
                        if (node == null)
                            continue;

                        ResourceTree tree = GetTree();
                        //Task.Run(() => 
                        //{
                            string key = tree.GetOrAddIcon(file);
                            node.ImageKey = node.SelectedImageKey = node.StateImageKey = key;
                        //})/*.ContinueWith(t => Nodes.Add(node))*/;
                       
                        Nodes.Add(node);
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

        private static bool IsFileObject(TypeProxy t)
          => t.Assembly != Assembly.GetExecutingAssembly() &&
              !t.IsAbstract && !t.IsInterface &&
              t.GetConstructors().Any(x => x.IsPublic) &&
              t.IsSubclassOf(typeof(TFileObject)) &&
              t.GetCustomAttribute<TFileExt>() != null;

        private static bool Is3rdPartyImportable(TypeProxy t)
        {
            if (!IsFileObject(t))
                return false;

            TFileExt attrib = t.GetCustomAttribute<TFileExt>();
            return attrib.HasAnyImportableExtensions;
        }

        private static async void OnImportClickAsync(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem button))
                return;

            TypeProxy fileType = button.Tag as TypeProxy;
            if (fileType == null)
                return;

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
                Filter = TFileObject.GetFilter(fileType, true, true, true, false),
                Title = "Import File"
            })
            {
                DialogResult r = ofd.ShowDialog(button.Owner);
                if (r != DialogResult.OK)
                    return;

                string name = Path.GetFileNameWithoutExtension(ofd.FileName);
                //ResourceTree tree = Editor.Instance.ContentTree;
                string path = ofd.FileName;
                        
                int op = Editor.Instance.BeginOperation($"Importing '{path}'...", "Import completed.", out Progress<float> progress, out CancellationTokenSource cancel);
                object file = await TFileObject.LoadAsync(fileType, path, progress, cancel.Token);
                Editor.Instance.EndOperation(op);

                if (file == null)
                    return;
                
                FolderWrapper folderNode = GetInstance<FolderWrapper>();
                string dir = folderNode.FilePath;

                if (!Serializer.PreExport(file, dir, name, EProprietaryFileFormat.XML, null, out string filePath))
                    return;

                Serializer serializer = new Serializer();
                op = Editor.Instance.BeginOperation($"Saving to '{filePath}'...", "Saved successfully.", out progress, out cancel);
                await serializer.SerializeXMLAsync(file, filePath, ESerializeFlags.Default, progress, cancel.Token);
                Editor.Instance.EndOperation(op);
            }
        }
        public static string GetFolderPath() => GetInstance<FolderWrapper>().FilePath;
        private static async void OnNewClick(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem button))
                return;
            
            Type fileType = button.Tag as Type;

            if (!(Editor.UserCreateInstanceOf(fileType, true, button.Owner) is TFileObject file))
                return;
            
            string dir = GetFolderPath();

            //Node will automatically be added to the file tree
            if (Serializer.PreExport(file, dir, file.Name, EProprietaryFileFormat.XML, null, out string path))
            {
                int op = Editor.Instance.BeginOperation("Exporting...", "Export completed.", out Progress<float> progress, out CancellationTokenSource cancel);
                string name = file.Name;
                name = name.Replace("<", "[");
                name = name.Replace(">", "]");
                await Serializer.ExportXMLAsync(file, dir, file.Name, ESerializeFlags.Default, progress, cancel.Token);
                Editor.Instance.EndOperation(op);
            }
        }
        private async void NewCodeFile(ECodeFileType type)
        {
            string ns = Editor.Instance.Project.RootNamespace;
            string ClassName = "NewClass";

            TextFile file = Engine.Files.LoadEngineScript(type.ToString() + "_Template.cs");
            string text = string.Format(file.Text, ns, ClassName, ": " + nameof(TObject));
            text = text.Replace("@", "{").Replace("#", "}");
            CSharpScript code = CSharpScript.FromText(text);
            string dir = GetFolderPath();

            string name = "NewCodeFile";
            string path = Path.Combine(dir, name + ".cs");
            int op = Editor.Instance.BeginOperation("Saving script...", "Script saved successfully.", out Progress<float> progress, out CancellationTokenSource cancel);
            await code.Export3rdPartyAsync(dir, "NewCodeFile", "cs", progress, cancel.Token);
            Editor.Instance.EndOperation(op);
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
                if (!_isPopulated) return;
                foreach (BaseWrapper b in Nodes)
                    b.FixPath(FilePath + Path.DirectorySeparatorChar);
            }
        }
        protected internal override void FixPath(string parentFolderPath)
        {
            string folderName = Text;
            if (!string.IsNullOrEmpty(parentFolderPath) && 
                parentFolderPath[parentFolderPath.Length - 1] != Path.DirectorySeparatorChar)
                parentFolderPath += Path.DirectorySeparatorChar;
            FilePath = parentFolderPath + folderName;
            if (!_isPopulated) return;
            foreach (BaseWrapper b in Nodes)
                b.FixPath(FilePath + Path.DirectorySeparatorChar);
        }
    }
}
