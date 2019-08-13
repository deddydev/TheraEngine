using TheraEngine.Actors;
using TheraEngine.GameModes;
using TheraEngine.Windows.Forms;
using TheraEngine.Worlds;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableModelEditorRenderForm : DockableWorldRenderFormBase<ModelEditorRenderPanel>
    {
        public DockableModelEditorRenderForm(ELocalPlayerIndex playerIndex, int formIndex, ModelEditorForm form) : base(playerIndex, formIndex)
        {
            ModelWindow = form;

            InitializeComponent();
            Controls.Add(RenderPanel);
            Text = $"Model Viewport {(FormIndex + 1).ToString()}";

            WindowGameMode = new EditorGameMode();
            EditorPawn.MouseTranslateSpeed = 0.02f;
            EditorPawn.ScrollSpeed = 0.5f;
            EditorPawn.GamepadTranslateSpeed = 15.0f;
        }

        public ModelEditorForm ModelWindow { get; private set; }
        public EditorGameMode WindowGameMode { get; set; }

        protected override IGameMode GameMode => WindowGameMode;
        protected override IWorld World => ModelWindow.World;
    }
}
