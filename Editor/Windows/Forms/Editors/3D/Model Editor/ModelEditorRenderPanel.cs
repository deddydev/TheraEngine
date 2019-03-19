using System;
using TheraEditor.Windows.Forms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;

namespace TheraEngine.Windows.Forms
{
    public class ModelEditorRenderPanel : RenderPanel<BaseScene>
    {
        public Func<Viewport, IVolume> GetCullingVolumeOverride { get; set; }
        public bool IsEditView { get; set; }
        private DockableModelEditorRenderForm _owner;

        public DockableModelEditorRenderForm Owner
        {
            get => _owner;
            set
            {
                if (_owner?.ModelWindow?.World != null)
                {
                    _owner.ModelWindow.World.CurrentGameModePostChanged -= World_CurrentGameModePostChanged;
                    _owner.ModelWindow.World.CurrentGameMode?.TargetRenderPanels?.Remove(this);
                }
                _owner = value;
                if (_owner?.ModelWindow?.World != null)
                {
                    _owner.ModelWindow.World.CurrentGameModePostChanged += World_CurrentGameModePostChanged;
                    _owner.ModelWindow.World.CurrentGameMode?.TargetRenderPanels?.Add(this);
                }
            }
        }
        private void World_CurrentGameModePostChanged(Worlds.World world, GameModes.BaseGameMode previous, GameModes.BaseGameMode next)
        {
            previous?.TargetRenderPanels?.Remove(this);
            next?.TargetRenderPanels?.Add(this);
        }

        protected override BaseScene GetScene(Viewport v)
            => Owner?.ModelWindow?.World?.Scene;
        protected override IVolume GetCullingVolume(Viewport v)
            => GetCullingVolumeOverride?.Invoke(v) ?? base.GetCullingVolume(v);
    }
}
