using System.Windows.Forms;
using System.ComponentModel;
using TheraEditor.Windows.Forms;
using TheraEngine.Rendering.Models;
using TheraEditor.Properties;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(SkeletalModel), nameof(Resources.GenericFile))]
    public class SkeletalModelWrapper : FileWrapper<SkeletalModel>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static SkeletalModelWrapper()
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
        
        public SkeletalModelWrapper() : base() { }

        public override void EditResource()
        {
            ModelEditorForm d = new ModelEditorForm();
            d.Show();
            d.SetModel(Resource);
        }
    }
}