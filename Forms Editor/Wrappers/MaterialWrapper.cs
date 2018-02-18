using System.Windows.Forms;
using System.ComponentModel;
using TheraEditor.Windows.Forms;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(TMaterial), SystemImages.GenericFile)]
    public class MaterialWrapper : FileWrapper<TMaterial>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static MaterialWrapper()
        {
            _menu = new ContextMenuStrip();
            FillContextMenuDefaults(_menu);
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
            MaterialEditorForm m = new MaterialEditorForm()
            {
                Material = ResourceRef
            };
            m.Show();
        }
    }
}
