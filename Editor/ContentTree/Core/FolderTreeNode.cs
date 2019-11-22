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
        public FolderTreeNode(string path) : base(path)
        {
            ImageIndex = 1;
            SelectedImageIndex = 1;
        }

        protected override void DestroyWrapper()
        {
            ((IFolderWrapper)Wrapper).NewFolderEvent -= NewFolder;
            base.DestroyWrapper();
        }
        protected override void DetermineWrapper()
        {
            base.DetermineWrapper();
            ((IFolderWrapper)Wrapper).NewFolderEvent += NewFolder;
        }

        protected override void Instance_DomainProxyPostSet(TheraEngine.Core.EngineDomainProxy proxy)
        {
            base.Instance_DomainProxyPostSet(proxy);
            proxy.ReloadTypeCaches += LoadFileTypes;
            LoadFileTypes(true);
        }
        protected override void Instance_DomainProxyPreUnset(TheraEngine.Core.EngineDomainProxy proxy)
        {
            base.Instance_DomainProxyPreUnset(proxy);
            proxy.ReloadTypeCaches -= LoadFileTypes;
            LoadFileTypes(false);
        }

        public void LoadFileTypes(bool now)
        {
            ((IFolderWrapper)Wrapper).LoadFileTypes(now);
            GenerateMenu();
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
