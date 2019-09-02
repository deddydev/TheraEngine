using System;
using TheraEditor;
using TheraEditor.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Core.Shapes;
using TheraEngine.GameModes;
using TheraEngine.Rendering;
using TheraEngine.Worlds;

namespace TheraEngine.Windows.Forms
{
    public class ModelEditorRenderHandler : WorldEditorRenderHandler
    {
        public ModelEditorRenderHandler(ELocalPlayerIndex playerIndex) : base(playerIndex)
        {
            EditorPawn.MouseTranslateSpeed = 0.02f;
            EditorPawn.ScrollSpeed = 0.5f;
            EditorPawn.GamepadTranslateSpeed = 15.0f;

            WindowGameMode?.TargetRenderHandlers?.Add(this);

            if (!ModelEditorWorld.IsPlaying)
                ModelEditorWorld.BeginPlay();
        }

        public Func<Viewport, IVolume> GetCullingVolumeOverride { get; set; }
        public bool IsEditView { get; set; }
        public EditorGameMode WindowGameMode { get; set; } = new EditorGameMode();

        //TODO: Cache new world per target model.
        public static World ModelEditorWorld { get; } = new World();

        public override IWorld World => ModelEditorWorld;
        protected override void LinkWorldChangeEvents() { } //World will not change

        protected override IVolume GetCullingVolume(Viewport v)
            => GetCullingVolumeOverride?.Invoke(v) ?? base.GetCullingVolume(v);
    }
}
