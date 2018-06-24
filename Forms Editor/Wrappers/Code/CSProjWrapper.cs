﻿using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Files;
using TheraEngine.Rendering.Models;

namespace TheraEditor.Wrappers
{
    [NodeWrapper("csproj")]
    public class CSProjWrapper : ThirdPartyFileWrapper
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static CSProjWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.F2));                              //0
            _menu.Items.Add(new ToolStripMenuItem("&Open In Explorer", null, ExplorerAction, Keys.Control | Keys.O));   //1
            _menu.Items.Add(new ToolStripMenuItem("Co&mpile", null, CompileAction, Keys.Control | Keys.M));             //2
            _menu.Items.Add(new ToolStripMenuItem("Edit Raw", null, EditRawAction, Keys.F3));                           //3
            _menu.Items.Add(new ToolStripSeparator());                                                                  //4
            _menu.Items.Add(new ToolStripMenuItem("&Cut", null, CutAction, Keys.Control | Keys.X));                     //5
            _menu.Items.Add(new ToolStripMenuItem("&Copy", null, CopyAction, Keys.Control | Keys.C));                   //6
            _menu.Items.Add(new ToolStripMenuItem("&Paste", null, PasteAction, Keys.Control | Keys.V));                 //7
            _menu.Items.Add(new ToolStripMenuItem("&Delete", null, DeleteAction, Keys.Control | Keys.Delete));          //8
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;
        }
        
        protected static void CompileAction(object sender, EventArgs e)
            => GetInstance<CSProjWrapper>().Compile();
        
        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            CSProjWrapper w = GetInstance<CSProjWrapper>();
        }
        #endregion
        
        public CSProjWrapper() : base(_menu) { }

        public void Compile()
        {
            Editor.Instance.Compile(FilePath);
        }
    }
}