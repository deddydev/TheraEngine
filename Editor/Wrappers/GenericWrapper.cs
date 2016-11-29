using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.IO;
using CustomEngine.Files;

namespace Editor.Wrappers
{
    //Contains generic members inherited by all sub-classed nodes
    class GenericWrapper : BaseWrapper
    {
        public GenericWrapper() { _owner = null; ContextMenu = _menu; }

        #region Menu
        private static ContextMenu _menu;
        static GenericWrapper()
        {
            _menu = new ContextMenu();
            //_menu.Items.Add(new CustomMenuItem("Export", ExportAction, Key.LeftCtrl, Key.E));
            //_menu.Items.Add(new CustomMenuItem("Replace", ReplaceAction, Key.LeftCtrl, Key.R));
            //_menu.Items.Add(new CustomMenuItem("Restore", RestoreAction, Key.LeftCtrl, Key.T));
            //_menu.Items.Add(new Separator());
            //_menu.Items.Add(new CustomMenuItem("Move Up", MoveUpAction, Key.LeftCtrl, Key.Up));
            //_menu.Items.Add(new CustomMenuItem("Move Down", MoveDownAction, Key.LeftCtrl, Key.Down));
            //_menu.Items.Add(new CustomMenuItem("Rename", RenameAction, Key.LeftCtrl, Key.N));
            //_menu.Items.Add(new Separator());
            //_menu.Items.Add(new CustomMenuItem("Delete", DeleteAction, Key.LeftCtrl, Key.Delete));
            _menu.Opened += MenuOpened;
            _menu.Closed += MenuClosed;
        }
        private static void MenuClosed(object sender, RoutedEventArgs e)
        {
            SetMenuEnabled(_menu, true, 1, 2, 4, 5, 8);
        }
        private static void MenuOpened(object sender, RoutedEventArgs e)
        {
            GenericWrapper w = GetInstance<GenericWrapper>();
            SetMenuEnabled(_menu, w.Parent != null, 1, 8);
            SetMenuEnabled(_menu, w._resource.IsDirty, 2);
            //SetMenuEnabled(_menu, w.PrevItem != null, 4);
            //SetMenuEnabled(_menu, w.NextItem != null, 5);
        }
        #endregion
        
        public void MoveUp() { MoveUp(true); }
        public virtual void MoveUp(bool select)
        {
            //if (PrevVisibleNode == null)
            //    return;

            //if (_resource.CanMoveUp)
            //{
            //    int index = Index - 1;
            //    TreeNode parent = Parent;
            //    TreeView.BeginUpdate();
            //    Remove();
            //    parent.Nodes.Insert(index, this);
            //    _resource.OnMoved();
            //    if (select)
            //        TreeView.SelectedNode = this;
            //    TreeView.EndUpdate();
            //}
        }

        public void MoveDown() { MoveDown(true); }
        public virtual void MoveDown(bool select)
        {
            //if (NextVisibleNode == null)
            //    return;

            //if (_resource.CanMoveDown)
            //{
            //    int index = Index + 1;
            //    TreeNode parent = Parent;
            //    TreeView.BeginUpdate();
            //    Remove();
            //    parent.Nodes.Insert(index, this);
            //    _resource.OnMoved();
            //    if (select)
            //        TreeView.SelectedNode = this;
            //    TreeView.EndUpdate();
        }

        public virtual string ExportFilter { get { return FileManager.GetCompleteFilter(_resource.GetType()); } }
        public virtual string ImportFilter { get { return ExportFilter; } }
        public virtual string ReplaceFilter { get { return ImportFilter; } }

        public static int CategorizeFilter(string path, string filter)
        {
            string ext = "*" + Path.GetExtension(path);

            string[] split = filter.Split('|');
            for (int i = 3; i < split.Length; i += 2)
                foreach (string s in split[i].Split(';'))
                    if (s.Equals(ext, StringComparison.OrdinalIgnoreCase))
                        return (i + 1) / 2;
            return 1;
        }

        //public virtual string Export()
        //{
        //    string outPath;
        //    int index = Program.SaveFile(ExportFilter, Text, out outPath);
        //    if (index != 0)
        //    {
        //        if (Parent == null)
        //            _resource.Merge(Control.ModifierKeys == (Keys.Control | Keys.Shift));
        //        OnExport(outPath, index);
        //    }
        //    return outPath;
        //}
        //public virtual void OnExport(string outPath, int filterIndex) { _resource.Export(outPath); }

        //public virtual void Replace()
        //{
        //    if (Parent == null)
        //        return;

        //    string inPath;
        //    int index = Program.OpenFile(ReplaceFilter, out inPath);
        //    if (index != 0)
        //    {
        //        OnReplace(inPath, index);
        //        this.Link(_resource);
        //    }
        //}

        //public virtual void OnReplace(string inStream, int filterIndex) { _resource.Replace(inStream); }

        //public void Restore()
        //{
        //    _resource.Restore();
        //}

        //public void Delete()
        //{
        //    if (Parent == null)
        //        return;

        //    _resource.Dispose();
        //    _resource.Remove();
        //}

        //public void Rename()
        //{
        //    using (RenameDialog dlg = new RenameDialog()) { dlg.ShowDialog(MainForm.Instance, _resource); }
        //}
    }
}
