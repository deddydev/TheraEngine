﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core;

namespace TheraEditor.Wrappers
{
    public class FileTreeNode : ContentTreeNode
    {
        public FileTreeNode(string path) : base()
        {
            FilePath = path;
            Text = Path.GetFileName(path);

            Engine.Instance.DomainProxyPostSet += Instance_DomainProxyPostSet;
            Engine.Instance.DomainProxyPreUnset += Instance_DomainProxyPreUnset;
        }

        private void Instance_DomainProxyPreUnset(EngineDomainProxy obj)
        {

        }
        private void Instance_DomainProxyPostSet(EngineDomainProxy obj)
        {

        }

        public override void Delete()
        {
            if (Parent is null)
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

        protected internal override void SetPath(string parentFolderPath)
        {
            string fileName = Text;
            if (parentFolderPath[parentFolderPath.Length - 1] != Path.DirectorySeparatorChar)
                parentFolderPath += Path.DirectorySeparatorChar;
            FilePath = parentFolderPath + fileName;
        }

        internal protected override void OnExpand() { }
        internal protected override void OnCollapse() { }
    }
}
