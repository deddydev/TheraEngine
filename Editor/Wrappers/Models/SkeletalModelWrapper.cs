using System.Windows.Forms;
using System.ComponentModel;
using TheraEditor.Windows.Forms;
using TheraEngine.Rendering.Models;
using TheraEditor.Properties;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(nameof(Resources.GenericFile))]
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
            SkeletalModelWrapper w = GetInstance<SkeletalModelWrapper>();
        }
        #endregion
        
        public SkeletalModelWrapper() : base() { }

        public override async void EditResource()
        {
            ModelEditorForm d = new ModelEditorForm();
            d.Show();
            var mdl = await ResourceRef.GetInstanceAsync();
            d.SetModel(mdl);
        }
    }
}