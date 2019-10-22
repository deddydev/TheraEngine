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
        }

        public Func<Viewport, IVolume> GetCullingVolumeOverride { get; set; }
        public bool IsEditView { get; set; }
        public EditorGameMode WindowGameMode { get; set; } = new EditorGameMode();

        public override IWorld World => WorldManager?.World;
        public override IGameMode GameMode => WindowGameMode;
        protected override void OnWorldManagerPreChanged()
        {
            PreWorldChanged();
            base.OnWorldManagerPreChanged();
        }
        protected override void OnWorldManagerPostChanged()
        {
            PostWorldChanged();
            base.OnWorldManagerPostChanged();
        }

        protected override void LinkEngineWorldChangeEvents() { } //World will not change

        protected override IVolume GetCullingVolume(Viewport v)
            => GetCullingVolumeOverride?.Invoke(v) ?? base.GetCullingVolume(v);
    }
}
