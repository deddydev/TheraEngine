using TheraEngine.GameModes;
using TheraEngine.Rendering;
using TheraEngine.Worlds;

namespace TheraEngine
{
    /// <summary>
    /// Renders the engine's scene that the current world spawns in.
    /// </summary>
    public class WorldRenderHandler : RenderHandler<IScene>
    {
        public WorldRenderHandler()
        {
            Engine.PreWorldChanged += Engine_PreWorldChanged;
            Engine.PostWorldChanged += Engine_PostWorldChanged;
        }

        private void Engine_PreWorldChanged()
        {
            if (Engine.World != null)
            {
                Engine.World.CurrentGameModePostChanged -= World_CurrentGameModePostChanged;
                Engine.World.CurrentGameMode?.TargetRenderHandlers?.Remove(this);
            }
        }
        private void Engine_PostWorldChanged()
        {
            if (Engine.World != null)
            {
                Engine.World.CurrentGameModePostChanged += World_CurrentGameModePostChanged;
                Engine.World.CurrentGameMode?.TargetRenderHandlers?.Add(this);
            }
        }
        private void World_CurrentGameModePostChanged(IWorld world, IGameMode previous, IGameMode next)
        {
            previous?.TargetRenderHandlers?.Remove(this);
            next?.TargetRenderHandlers?.Add(this);
        }

        protected override IScene GetScene(Viewport v) => Engine.Scene;
        protected override void GlobalPreRender()
        {
            Engine.Scene?.GlobalPreRender();
            base.GlobalPreRender();
        }
    }
}
