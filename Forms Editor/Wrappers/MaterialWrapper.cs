using System.Windows.Forms;
using System.ComponentModel;
using TheraEditor.Windows.Forms;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(Material), SystemImages.GenericFile)]
    public class MaterialWrapper : FileWrapper<Material>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static MaterialWrapper()
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
        
        public MaterialWrapper() : base() { }

        public override void EditResource()
        {
            MaterialEditorForm m = new MaterialEditorForm();
            m.Material = Resource;
            m.ShowDialog();
        }
    }
}
