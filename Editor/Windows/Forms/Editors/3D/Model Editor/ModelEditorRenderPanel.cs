using System;
using TheraEditor.Windows.Forms;
using TheraEngine.Core.Shapes;
using TheraEngine.GameModes;
using TheraEngine.Rendering;
using TheraEngine.Worlds;

namespace TheraEngine.Windows.Forms
{
    public class ModelEditorRenderPanel : RenderPanel<IScene>
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
                    _owner.GameMode?.TargetRenderPanels?.Remove(this);
                }
                _owner = value;
                if (_owner?.ModelWindow?.World != null)
                {
                    _owner.ModelWindow.World.CurrentGameModePostChanged += World_CurrentGameModePostChanged;
                    _owner.GameMode?.TargetRenderPanels?.Add(this);
                }
            }
        }
        private void World_CurrentGameModePostChanged(IWorld world, IGameMode previous, IGameMode next)
        {
            previous?.TargetRenderPanels?.Remove(this);
            next?.TargetRenderPanels?.Add(this);
        }

        protected override IScene GetScene(Viewport v)
            => Owner?.ModelWindow?.World?.Scene;
        protected override IVolume GetCullingVolume(Viewport v)
            => GetCullingVolumeOverride?.Invoke(v) ?? base.GetCullingVolume(v);
    }
}
