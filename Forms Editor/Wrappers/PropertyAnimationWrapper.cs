using System.Windows.Forms;
using System.ComponentModel;
using TheraEditor.Windows.Forms;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Animation;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(PropAnimFloat), SystemImages.GenericFile)]
    public class PropAnimFloatWrapper : FileWrapper<PropAnimFloat>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static PropAnimFloatWrapper()
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
            PropAnimFloatWrapper w = GetInstance<PropAnimFloatWrapper>();
        }
        #endregion
        
        public PropAnimFloatWrapper() : base() { }

        public override void EditResource()
        {
            PropAnimFloatEditor m = new PropAnimFloatEditor();
            m.Animation = ResourceRef;
            m.Show();
        }
    }
}
