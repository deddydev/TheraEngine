using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine.Core.Files;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    public class GenericTextFileWrapper<T> : FileWrapper<T> where T : TextFile
    {
        #region Menu
        //private static ContextMenuStrip _menu;
        static GenericTextFileWrapper()
        {
            //_menu = new ContextMenuStrip();
            //_menu.Opening += MenuOpening;
            //_menu.Closing += MenuClosing;
        }
        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {

        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            //GenericTextFileWrapper<T> w = GetInstance<GenericTextFileWrapper<T>>();
        }
        #endregion

        public GenericTextFileWrapper() : base() { }

        public override void EditResource()
            => DockableTextEditor.ShowNew(Editor.Instance.DockPanel, DockState.Document, Resource);
    }
}
