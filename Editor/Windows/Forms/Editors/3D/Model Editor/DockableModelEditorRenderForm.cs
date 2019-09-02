using TheraEngine.Actors;
using TheraEngine.GameModes;
using TheraEngine.Windows.Forms;
using TheraEngine.Worlds;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableModelEditorRenderForm : DockableWorldRenderFormBase<ModelEditorRenderHandler>
    {
        public DockableModelEditorRenderForm(ELocalPlayerIndex playerIndex, int formIndex, ModelEditorForm form) 
            : base(playerIndex, formIndex)
        {
            ModelWindow = form;

            InitializeComponent();

            Text = $"Model Viewport {(FormIndex + 1).ToString()}";
        }

        public ModelEditorForm ModelWindow { get; private set; }
    }
}
