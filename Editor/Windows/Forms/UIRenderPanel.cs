using System;
using System.ComponentModel;
using TheraEditor.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Input;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;

namespace TheraEngine
{
    public interface IUIRenderPanel : IRenderPanel
    {
        IUserInterface UI { get; }
        World World { get; }
        IUIGameMode GameMode { get; }

        void FormShown();
        void FormClosed();
    }
    public class UIRenderPanel<UIPawnType, UIGameModeType, UIControllerType> : RenderPanel<Scene2D>, IUIRenderPanel 
        where UIPawnType : BaseActor, IUserInterface, new()
        where UIGameModeType : UIGameMode<UIPawnType, UIControllerType>, new()
        where UIControllerType : LocalPlayerController
    {
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public UIPawnType UI { get; }
        /// <summary>
        /// The self-contained world for items displayed by this render panel.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public World World { get; } = new World(new WorldSettings() { TwoDimensional = true });
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public UIGameModeType GameMode { get; }

        IUserInterface IUIRenderPanel.UI => UI;
        World IUIRenderPanel.World => World;
        IUIGameMode IUIRenderPanel.GameMode => GameMode;

        protected override Scene2D GetScene(Viewport v) => World.Scene2D;
        protected override Camera GetCamera(Viewport v) => UI?.ScreenOverlayCamera;

        public UIRenderPanel()
        {
            Viewport v = AddViewport(ELocalPlayerIndex.One);

            GameMode = new UIGameModeType();
            GameMode.TargetRenderPanels.Add(this);
            World.CurrentGameMode = GameMode;

            UI = new UIPawnType();

            v.HUD = UI;
            v.Camera = UI.ScreenOverlayCamera;
        }
        public void FormShown()
        {
            World.BeginPlay();
            World.SpawnActor(UI);
            RegisterTick();
        }
        public void FormClosed()
        {
            World.EndPlay();
            World.DespawnActor(UI);
            UnregisterTick();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UI.Resize(new Vec2(Width, Height));
        }
    }
}