using System.Windows.Forms;
using System.ComponentModel;
using TheraEditor.Windows.Forms;
using TheraEngine.Scripting;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(PythonScript), SystemImages.GenericFile)]
    public class PythonWrapper : FileWrapper<PythonScript>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static PythonWrapper()
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
            MaterialWrapper w = GetInstance<MaterialWrapper>();
        }
        #endregion
        
        public PythonWrapper() : base() { }

        public override void EditResource()
        {
            DockableTextEditor m = new DockableTextEditor();
            m.Show(Editor.Instance.DockPanel, DockState.Document);
            m.Script = ResourceRef.File;
        }
    }
}
