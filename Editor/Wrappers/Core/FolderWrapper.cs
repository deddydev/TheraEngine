using AppDomainToolkit;
using Extensions;
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

            Engine.Instance.DomainProxy.ReloadTypeCaches += LoadFileTypes;
            LoadFileTypes(true);
        }
        public static void LoadFileTypes(bool now)
        {
            if (_menu.InvokeRequired)
            {
                _menu.Invoke((Action<bool>)LoadFileTypes, now);
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

            Engine.PrintLine("Loading importable and creatable file types to folder menu.");
            Task import = Task.Run(() =>
            {
                Program.PopulateMenuDropDown(importDropdown, OnImportClickAsync, Is3rdPartyImportable);
            });
            Task create = Task.Run(() =>
            {
                Program.PopulateMenuDropDown(newDropdown, OnNewClick, IsFileObject);
            });
            Task.WhenAll(import, create).ContinueWith(t =>
            {
                Engine.PrintLine("Finished loading importable and creatable file types to folder menu.");
            });
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
        protected static async void ArchiveAction(object sender, EventArgs e) => await GetInstance<FolderWrapper>().ToArchive();
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

            try
            {
                IDataObject data = Clipboard.GetDataObject();
                if (data.GetDataPresent(DataFormats.FileDrop))
                {
                    MemoryStream stream = data.GetData("Preferred DropEffect", true) as MemoryStream;
                    int flag = stream?.ReadByte() ?? 0;
                    paste = flag == 2 || flag == 5;
                }
            }
            catch { }

            _menu.Items[11].Enabled = paste;
            _menu.Items[9].Enabled = _menu.Items[12].Enabled = w.Parent != null;
        }
        #endregion

        public override string FilePath
        {
            get => Name;
            set => Name = value;
        }

        public async Task ToArchive(
            ESerializeFlags flags = ESerializeFlags.Default,
            EProprietaryFileFormat format = EProprietaryFileFormat.Binary)
        {
            string path = FilePath;
            ArchiveFile file = ArchiveFile.FromDirectory(path, true);
            string parentDir = Path.GetDirectoryName(path);
            string dirName = Path.GetFileName(path);
            await file.ExportAsync(parentDir, dirName, flags, format);
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
            if (Parent is null)
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
            catch (DirectoryNotFoundException)
            {
                Engine.PrintLine($"Unable to find directory {FilePath}; removing from content tree.");
                Remove();
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
                        if (node is null)
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
                        if (node is null)
                            continue;

                        string key = Tree.GetOrAddIconFromPath(file);
                        node.ImageKey = node.SelectedImageKey = node.StateImageKey = key;
                                               
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

        private static bool IsFileObject(TypeProxy t) => 
            !t.Assembly.EqualTo(Assembly.GetExecutingAssembly()) &&
            !t.IsAbstract && !t.IsInterface && t.GetConstructors().Any(x => x.IsPublic) &&
            t.IsSubclassOf(typeof(TFileObject)) &&
            t.HasCustomAttribute<TFileExt>();
        
        private static bool Is3rdPartyImportable(TypeProxy t)
            => IsFileObject(t) && (t?.GetCustomAttribute<TFileExt>().HasAnyImportableExtensions ?? false);
        
        private static void OnImportClickAsync(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem button) || !(button.Tag is TypeProxy fileType))
                return;

            Editor.DomainProxy.Import(fileType, GetInstance<FolderWrapper>()?.FilePath);
        }
        public static string GetFolderPath() => GetInstance<FolderWrapper>().FilePath;
        private static void OnNewClick(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem button))
                return;

            TypeProxy fileType = button.Tag as TypeProxy;

            object o = Editor.UserCreateInstanceOf(fileType, true, button.Owner);
            if (!(o is IFileObject file))
                return;

            string dir = GetFolderPath();
            Engine.DomainProxy.ExportFile(file, dir, EProprietaryFileFormat.XML);

            //if (Serializer.PreExport(file, dir, file.Name, EProprietaryFileFormat.XML, null, out string path))
            //{
            //    int op = Editor.Instance.BeginOperation($"Exporting {path}...", $"Export to {path} completed.", out Progress<float> progress, out CancellationTokenSource cancel);
            //    string name = file.Name;
            //    name = name.Replace("<", "[");
            //    name = name.Replace(">", "]");
            //    await Serializer.ExportXMLAsync(file, dir, name, ESerializeFlags.Default, progress, cancel.Token);
            //    Editor.Instance.EndOperation(op);
            //}
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
            await Editor.RunOperationAsync(
                $"Saving script to {path}...", $"Script saved to {path} successfully.", async (p, c) =>
                await code.Export3rdPartyAsync(dir, "NewCodeFile", "cs", p, c.Token));
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
