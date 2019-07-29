using TheraEngine.GameModes;
using TheraEngine.Rendering;
using TheraEngine.Worlds;

namespace TheraEngine
{
    /// <summary>
    /// Renders the engine's scene that the current world spawns in.
    /// </summary>
    public class WorldRenderPanel : RenderPanel<IScene>
    {
        public WorldRenderPanel()
        {
            Engine.Instance.PreWorldChanged += Engine_PreWorldChanged;
            Engine.Instance.PostWorldChanged += Engine_PostWorldChanged;
        }

        private void Engine_PreWorldChanged()
        {
            if (Engine.World != null)
            {
                Engine.World.CurrentGameModePostChanged -= World_CurrentGameModePostChanged;
                Engine.World.CurrentGameMode?.TargetRenderPanels?.Remove(this);
            }
        }
        private void Engine_PostWorldChanged()
        {
            if (Engine.World != null)
            {
                Engine.World.CurrentGameModePostChanged += World_CurrentGameModePostChanged;
                Engine.World.CurrentGameMode?.TargetRenderPanels?.Add(this);
            }
        }
        private void World_CurrentGameModePostChanged(IWorld world, IGameMode previous, IGameMode next)
        {
            previous?.TargetRenderPanels?.Remove(this);
            next?.TargetRenderPanels?.Add(this);
        }

        protected override IScene GetScene(Viewport v) => Engine.Scene;
        protected override void GlobalPreRender()
        {
            Engine.Scene?.GlobalPreRender();
            base.GlobalPreRender();
        }
    }
}
