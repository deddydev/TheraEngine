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
            get => propertyGrid.TargetFileObject as MaterialFunction;
            set => propertyGrid.TargetFileObject = value;
        }
    }
}
