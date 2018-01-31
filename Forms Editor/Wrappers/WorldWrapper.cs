using System.Windows.Forms;
using System.ComponentModel;
using TheraEngine.Worlds;
using TheraEditor.Windows.Forms;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(World), SystemImages.GenericFile)]
    public class WorldWrapper : FileWrapper<World>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static WorldWrapper()
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
            WorldWrapper w = GetInstance<WorldWrapper>();
        }
        #endregion

        public WorldWrapper() : base() { }

        public override void EditResource()
        {
            Editor.Instance.CurrentWorld = Resource;
            Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = Resource.Settings;
        }
    }
}
