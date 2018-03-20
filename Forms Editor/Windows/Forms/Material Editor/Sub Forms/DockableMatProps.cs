using TheraEngine.Rendering.Models.Materials;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMatProps : DockContent
    {
        public DockableMatProps()
        {
            InitializeComponent();
        }

        public TMaterial TargetMaterial
        {
            get => materialControl1.Material;
            set => materialControl1.Material = value;
        }
    }
}
