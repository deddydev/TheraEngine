using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.GameModes;
using TheraEngine.Input;

namespace TheraEditor.Windows.Forms
{
    public interface IUIGameMode
    {
        IUIRenderPanel RenderPanel { get; set; }
    }
    public class UIGameMode<PawnType, ControllerType> : GameMode<PawnType, ControllerType>, IUIGameMode
        where PawnType : BaseActor, IUserInterface, new()
        where ControllerType : LocalPlayerController
    {
        private IUIRenderPanel _renderPanel;
        public IUIRenderPanel RenderPanel
        {
            get => _renderPanel;
            set
            {
                _renderPanel = value;
                TargetRenderPanels.Clear();
                if (_renderPanel is BaseRenderPanel renderPanel)
                    TargetRenderPanels.Add(renderPanel);
            }
        }
        protected override void HandleLocalPlayerJoined(ControllerType item)
        {
            if (RenderPanel == null)
            {
                Engine.LogWarning($"No UI render panel set.");
                return;
            }

            RenderPanel.GetOrAddViewport(item.LocalPlayerIndex)?.RegisterController(item);

            item.EnqueuePosession(RenderPanel.UI);
            item.Viewport.HUD = RenderPanel.UI;
            item.ViewportCamera = RenderPanel.UI.ScreenOverlayCamera;
        }
        protected override void HandleLocalPlayerLeft(ControllerType item)
        {
            RenderPanel.UnregisterController(item);
            item.UnlinkControlledPawn();
        }
    }
}
