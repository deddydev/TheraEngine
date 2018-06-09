using TheraEngine.Rendering.Models.Materials;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMaterialPreview : DockContent
    {
        public DockableMaterialPreview()
        {
            InitializeComponent();
        }
        public void SetMaterial(TMaterial mat)
        {
            materialControl1.Material = mat;
        }
    }
}
