using System;
using TheraEditor.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Input;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;

namespace TheraEngine
{
    public interface IUIRenderHandler : IRenderHandler
    {
        IUserInterface UI { get; }
        IWorld World { get; }
        IUIGameMode GameMode { get; }

        void FormShown();
        void FormClosed();
    }
    public class UIRenderHandler<UIPawnType, UIGameModeType, UIControllerType> : 
        BaseEditorRenderHandler<IScene2D, UIPawnType, UIGameModeType>, IUIRenderHandler 
        where UIPawnType : class, IPawn, IUserInterface, new()
        where UIGameModeType : UIGameMode<UIPawnType, UIControllerType>, new()
        where UIControllerType : LocalPlayerController
    {
        /// <summary>
        /// The self-contained world for items displayed by this render panel.
        /// </summary>
        public override IWorld World => WorldManager?.World;
        public override UIGameModeType GameMode { get; }
        public UIPawnType UI { get; }

        IUserInterface IUIRenderHandler.UI => UI;
        IWorld IUIRenderHandler.World => World;
        IUIGameMode IUIRenderHandler.GameMode => GameMode;
        public override UIPawnType EditorPawn => UI;

        protected override IScene2D GetScene(Viewport v) => World?.Scene2D;
        protected override ICamera GetCamera(Viewport v) => UI?.ScreenOverlayCamera;

        public UIRenderHandler() : base(ELocalPlayerIndex.One)
        {
            Viewport v = GetOrAddViewport(ELocalPlayerIndex.One);

            GameMode = new UIGameModeType { RenderHandler = this };
            if (World != null)
                World.CurrentGameMode = GameMode;

            UI = new UIPawnType();

            v.HUD = UI;
            v.Camera = UI.ScreenOverlayCamera;
        }
        protected override void OnWorldManagerPreChanged()
        {
            if (Visible)
            {
                if (World != null)
                {
                    World.DespawnActor(UI);
                    World.EndPlay();
                    World.CurrentGameMode = null;
                }
            }
            base.OnWorldManagerPreChanged();
        }
        protected override void OnWorldManagerPostChanged()
        {
            if (Visible)
            {
                if (World != null)
                {
                    World.CurrentGameMode = GameMode;
                    World.BeginPlay();
                    World.SpawnActor(UI);
                }
            }
            base.OnWorldManagerPostChanged();
        }
        public bool Visible { get; private set; }
        public void FormShown()
        {
            Visible = true;
            if (World is null)
                return;
            World.BeginPlay();
            World.SpawnActor(UI);
            //RegisterTick();
        }
        public void FormClosed()
        {
            Visible = false;
            if (World is null)
                return;
            World.EndPlay();
            World.DespawnActor(UI);
            //UnregisterTick();
        }
        public override void Resize(int width, int height)
        {
            base.Resize(width, height);
            UI?.Resize(new Vec2(Width, Height));
        }
    }
}