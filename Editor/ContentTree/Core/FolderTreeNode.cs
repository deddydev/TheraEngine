﻿using Extensions;
using Microsoft.VisualBasic.FileIO;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;
using TheraEngine.Scripting;

namespace TheraEditor.Wrappers
{
    public class FolderTreeNode : ContentTreeNode
    {
        private TMenuOption ImportFileOption { get; set; }
        private TMenuOption NewFileOption { get; set; }

        public FolderTreeNode(string path) : base()
        {
            FilePath = path;
            ImageIndex = 1;
            SelectedImageIndex = 1;

            //Generate menu in main domain for folders
            //Populate new and import child items from game domain

            Menu.RemoveAt(3);
            Menu.RemoveAt(3);

            Menu.Insert(3, new TMenuOption("Compile To Archive", ToArchive, Keys.Control | Keys.A));
            Menu.Insert(3, TMenuDivider.Instance);
            Menu.Insert(3, new TMenuOption("New Folder", null, Keys.Control | Keys.F));
            Menu.Insert(3, NewFileOption = new TMenuOption("New File", null, Keys.Control | Keys.N));
            Menu.Insert(3, ImportFileOption = new TMenuOption("Import File", null, Keys.Control | Keys.I));
        }

        protected override void Instance_DomainProxySet(TheraEngine.Core.EngineDomainProxy proxy)
        {
            proxy.ReloadTypeCaches += LoadFileTypes;
            LoadFileTypes(true);
        }
        protected override void Instance_DomainProxyUnset(TheraEngine.Core.EngineDomainProxy proxy)
        {
            proxy.ReloadTypeCaches -= LoadFileTypes;
            LoadFileTypes(false);
        }

        public static void LoadFileTypes(bool now)
        {
            //if (_menu.InvokeRequired)
            //{
            //    _menu.Invoke((Action<bool>)LoadFileTypes, now);
            //    return;
            //}

            //ToolStripDropDownItem importDropdown = (ToolStripDropDownItem)_menu.Items[3];
            //ToolStripDropDownItem newDropdown = (ToolStripDropDownItem)_menu.Items[4];

            //importDropdown.DropDownItems.Clear();
            //newDropdown.DropDownItems.Clear();

            //ToolStripMenuItem newCodeItem = new ToolStripMenuItem("C#");
            //newCodeItem.DropDownItems.Add(new ToolStripMenuItem("Class", null, NewClassAction));
            //newCodeItem.DropDownItems.Add(new ToolStripMenuItem("Struct", null, NewStructAction));
            //newCodeItem.DropDownItems.Add(new ToolStripMenuItem("Interface", null, NewInterfaceAction));
            //newCodeItem.DropDownItems.Add(new ToolStripMenuItem("Enum", null, NewEnumAction));
            //newDropdown.DropDownItems.Add(newCodeItem);

            //if (now)
            //{
            //    Engine.PrintLine("Loading importable and creatable file types to folder menu.");
            //    //Task import = Task.Run(() =>
            //    //{
            //    Program.GenerateTypeTree(Is3rdPartyImportable);
            //    //});
            //    //Task create = Task.Run(() =>
            //    //{
            //    Program.GenerateTypeTree(IsFileObject);
            //    //});
            //    //Task.WhenAll(import, create).ContinueWith(t =>
            //    //{
            //    //    Engine.PrintLine("Finished loading importable and creatable file types to folder menu.");
            //    //});
            //}
        }

        private enum ECodeFileType
        {
            Class,
            Struct,
            Interface,
            Enum,
        }

        public static bool IsClipboardPasteReady()
        {
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

            return paste;
        }

        public async void ToArchive()
            => await ToArchiveAsync();

        public async Task ToArchiveAsync(
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
            FolderTreeNode b = Wrap(path) as FolderTreeNode;
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

                        ContentTreeNode node = Wrap(subDir);
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
                        ContentTreeNode node = Wrap(file);
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

            Editor.DomainProxy.Import(fileType, GetInstance<FolderTreeNode>()?.FilePath);
        }
        public static string GetFolderPath() => GetInstance<FolderTreeNode>().FilePath;
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

            TextFile file = Engine.Files.Script(type.ToString() + "_Template.cs");
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

                if (!_isPopulated) 
                    return;
                
                foreach (ContentTreeNode b in Nodes)
                    b.SetPath(FilePath + Path.DirectorySeparatorChar);
            }
        }
        protected internal override void SetPath(string parentFolderPath)
        {
            string folderName = Text;

            if (!string.IsNullOrEmpty(parentFolderPath) && 
                parentFolderPath[parentFolderPath.Length - 1] != Path.DirectorySeparatorChar)
                parentFolderPath += Path.DirectorySeparatorChar;

            FilePath = parentFolderPath + folderName;

            if (!_isPopulated) 
                return;

            foreach (ContentTreeNode b in Nodes)
                b.SetPath(FilePath + Path.DirectorySeparatorChar);
        }
    }
}
