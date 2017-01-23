using System;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace TheraEditor.Wrappers
{
    public class FolderWrapper : GenericWrapper
    {
        #region Menu
        
        static FolderWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Re&name", null, RenameAction, Keys.Control | Keys.N));
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add(new ToolStripMenuItem("&Delete", null, DeleteAction, Keys.Control | Keys.Delete));
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;
        }
        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            _menu.Items[0].Enabled = _menu.Items[2].Enabled = true;
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            //GenericWrapper w = GetInstance<GenericWrapper>();
            //_menu.Items[1].Enabled = _menu.Items[8].Enabled = w.Parent != null;
            //_menu.Items[2].Enabled = ((w._resource.IsDirty) || (w._resource.IsBranch));
            //_menu.Items[4].Enabled = w.PrevNode != null;
            //_menu.Items[5].Enabled = w.NextNode != null;
        }

        #endregion

        public FolderWrapper(string path) : base(path) { }
    }
}
