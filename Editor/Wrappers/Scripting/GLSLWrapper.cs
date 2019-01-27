using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Properties;
using TheraEditor.Windows.Forms;
using TheraEngine.Rendering.Models.Materials;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(nameof(Resources.GenericFile))]
    public class GLSLWrapper : FileWrapper<GLSLScript>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static GLSLWrapper()
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
            GLSLWrapper w = GetInstance<GLSLWrapper>();
        }
        #endregion
        
        public GLSLWrapper() : base() { }
        
        public override void EditResource()
            => DockableTextEditor.ShowNew(Editor.Instance.DockPanel, DockState.Document, ResourceRef.File);
    }
}