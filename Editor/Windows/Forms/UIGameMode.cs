using TheraEngine;
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
        where PawnType : class, IUserInterface, new()
        where ControllerType : LocalPlayerController
    {
        public IUIRenderPanel RenderPanel { get; set; }
        protected override void HandleLocalPlayerJoined(ControllerType item)
        {
            RenderPanel.GetOrAddViewport(0)?.RegisterController(item);
            item.EnqueuePosession(RenderPanel.UI);
            item.Viewport.HUD = RenderPanel.UI;
            item.ViewportCamera = RenderPanel.UI.Camera;
        }
        protected override void HandleLocalPlayerLeft(ControllerType item)
        {
            RenderPanel.UnregisterController(item);
            item.UnlinkControlledPawn();
        }
    }
}
