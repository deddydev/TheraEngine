using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Properties;
using TheraEditor.Windows.Forms;
using TheraEngine.Scripting;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(nameof(Resources.CSharpScript))]
    public class CSharpWrapper : FileWrapper<CSharpScript>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static CSharpWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;
        }
        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            CSharpWrapper w = GetInstance<CSharpWrapper>();
        }
        #endregion
        
        public CSharpWrapper() : base() { }

        public override void EditResource()
        {
            DockableTextEditor.ShowNew(
                Editor.Instance.DockPanel,
                DockState.Document, 
                Resource);
        }
    }
}