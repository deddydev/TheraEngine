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
        World World { get; }
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
        public World World { get; } = new World();
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public UIGameModeType GameMode { get; }

        IUserInterface IUIRenderPanel.UI => UI;
        World IUIRenderPanel.World => World;
        IUIGameMode IUIRenderPanel.GameMode => GameMode;

        protected override Scene2D GetScene(Viewport v) => UI?.UIScene;
        protected override Camera GetCamera(Viewport v) => UI?.Camera;

        public UIRenderPanel()
        {
            if (Engine.DesignMode)
                return;
            Viewport v = AddViewport();
            v.HUD = UI = new UIPawnType();
            v.Camera = UI.Camera;
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