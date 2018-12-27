using System.ComponentModel;
using TheraEditor.Windows.Forms;
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
        TWorld World { get; }
        IUIGameMode GameMode { get; }
    }
    public class UIRenderPanel<UIPawnType, UIGameModeType, UIControllerType> : RenderPanel<Scene2D>, IUIRenderPanel 
        where UIPawnType : class, IUserInterface, new()
        where UIGameModeType : UIGameMode<UIPawnType, UIControllerType>
        where UIControllerType : LocalPlayerController
    {
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public UIPawnType UI { get; }
        /// <summary>
        /// The self-contained world for items displayed by this render panel.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public TWorld World { get; } = new TWorld();
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public UIGameModeType GameMode { get; }

        IUserInterface IUIRenderPanel.UI => UI;
        TWorld IUIRenderPanel.World => World;
        IUIGameMode IUIRenderPanel.GameMode => GameMode;

        protected override Scene2D GetScene(Viewport v) => UI?.ScreenSpaceUIScene;
        protected override Camera GetCamera(Viewport v) => UI?.ScreenOverlayCamera;

        public UIRenderPanel()
        {
            if (Engine.DesignMode)
                return;
            Viewport v = AddViewport();
            v.HUD = UI = new UIPawnType();
            v.Camera = UI.ScreenOverlayCamera;
        }
        public void FormShown()
        {
            World.SpawnActor(UI);
            RegisterTick();
        }
        public void FormClosed()
        {
            World.DespawnActor(UI);
            UnregisterTick();
        }
    }
}