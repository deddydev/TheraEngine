using System.Windows.Forms;
using System.ComponentModel;
using TheraEngine.Worlds;
using TheraEditor.Windows.Forms;
using TheraEditor.Properties;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(nameof(Resources.GenericFile))]
    public class WorldWrapper : FileWrapper<TWorld>
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
            ResourceRef.GetInstanceAsync().ContinueWith(t => 
            {
                if (t.Result != null)
                {
                    Editor.Instance.CurrentWorld = t.Result;
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = t.Result.Settings;
                }
            });
            int i = 0;
            for (; i < 4; ++i)
            {
                if (Editor.Instance.RenderFormActive(i))
                {
                    Editor.Instance.GetRenderForm(i).Focus();
                    break;
                }
            }
            if (i == 4)
                Editor.Instance.RenderForm1.Focus();

        }
    }
}
