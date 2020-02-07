using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.GameModes;
using TheraEngine.Input;
using TheraEngine.Rendering.UI;

namespace TheraEditor.Windows.Forms
{
    public interface IUIGameMode : IGameMode
    {
        IUIRenderHandler RenderHandler { get; set; }
    }
    public class UIGameMode<PawnType, ControllerType> : GameMode<PawnType, ControllerType>, IUIGameMode
        where PawnType : class, IActor, IUserInterfacePawn, new()
        where ControllerType : LocalPlayerController
    {
        private IUIRenderHandler _renderHandler;
        public IUIRenderHandler RenderHandler
        {
            get => _renderHandler;
            set
            {
                Set(ref _renderHandler, value);
                //TargetRenderHandlers.Clear();
                //if (_renderHandler != null)
                //    TargetRenderHandlers.Add(_renderHandler);
            }
        }
        protected override void HandleLocalPlayerJoined(ControllerType item)
        {
            RenderHandler.RegisterController(item);

            item.EnqueuePosession(RenderHandler.UI);
            item.Viewport.AttachedHUD = RenderHandler.UI;
            item.ViewportCamera = ((IUICanvasComponent)RenderHandler.UI.RootComponent).ScreenSpaceCamera;
        }
        protected override void HandleLocalPlayerLeft(ControllerType item)
        {
            RenderHandler.UnregisterController(item);
            item.UnlinkControlledPawn();
        }
    }
}
