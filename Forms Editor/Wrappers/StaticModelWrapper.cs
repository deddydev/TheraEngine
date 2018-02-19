using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Properties;
using TheraEditor.Windows.Forms;
using TheraEngine.Rendering.Models;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(StaticModel), nameof(Resources.GenericFile))]
    public class StaticModelWrapper : FileWrapper<StaticModel>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static StaticModelWrapper()
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
            StaticModelWrapper w = GetInstance<StaticModelWrapper>();
        }
        #endregion
        
        public StaticModelWrapper() : base() { }

        public override void EditResource()
        {
            ModelEditorForm d = new ModelEditorForm();
            d.Show();
            d.SetModel(Resource);
        }
    }
}