using System;
using TheraEngine.Rendering;

namespace TheraEngine
{
    /// <summary>
    /// Renders the engine's scene that the current world spawns in.
    /// </summary>
    public class WorldRenderPanel : RenderPanel<BaseScene>
    {
        public WorldRenderPanel()
        {
            Engine.PreWorldChanged += Engine_PreWorldChanged;
            Engine.PostWorldChanged += Engine_PostWorldChanged;
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
        private void World_CurrentGameModePostChanged(Worlds.World world, GameModes.BaseGameMode previous, GameModes.BaseGameMode next)
        {
            previous?.TargetRenderPanels?.Remove(this);
            next?.TargetRenderPanels?.Add(this);
        }

        protected override BaseScene GetScene(Viewport v) => Engine.Scene;
        protected override void GlobalPreRender()
        {
            Engine.Scene?.GlobalPreRender();
            base.GlobalPreRender();
        }
    }
}
