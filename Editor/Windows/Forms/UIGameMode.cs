﻿using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.GameModes;
using TheraEngine.Input;

namespace TheraEditor.Windows.Forms
{
    public interface IUIGameMode : IGameMode
    {
        IUIRenderHandler RenderHandler { get; set; }
    }
    public class UIGameMode<PawnType, ControllerType> : GameMode<PawnType, ControllerType>, IUIGameMode
        where PawnType : class, IActor, IUserInterface, new()
        where ControllerType : LocalPlayerController
    {
        private IUIRenderHandler _renderHandler;
        public IUIRenderHandler RenderHandler
        {
            get => _renderHandler;
            set
            {
                _renderHandler = value;
                TargetRenderHandlers.Clear();
                if (_renderHandler != null)
                    TargetRenderHandlers.Add(_renderHandler);
            }
        }
        protected override void HandleLocalPlayerJoined(ControllerType item)
        {
            RenderHandler.RegisterController(item);

            item.EnqueuePosession(RenderHandler.UI);
            item.Viewport.HUD = RenderHandler.UI;
            item.ViewportCamera = RenderHandler.UI.ScreenOverlayCamera;
        }
        protected override void HandleLocalPlayerLeft(ControllerType item)
        {
            RenderHandler.UnregisterController(item);
            item.UnlinkControlledPawn();
        }
    }
}
