using TheraEngine.Rendering.Models.Materials.Functions;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMatFuncProps : DockContent
    {
        public DockableMatFuncProps()
        {
            InitializeComponent();
        }

        public MaterialFunction TargetFunc
        {
            get => propertyGrid.TargetObject as MaterialFunction;
            set => propertyGrid.TargetObject = value;
        }
    }
}
