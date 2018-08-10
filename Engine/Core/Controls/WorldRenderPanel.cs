using TheraEngine.Rendering;

namespace TheraEngine
{
    /// <summary>
    /// Renders the engine's scene that the current world spawns in.
    /// </summary>
    public class WorldRenderPanel : RenderPanel<BaseScene>
    {
        protected override BaseScene GetScene(Viewport v) => Engine.Scene;
        protected override void PreRender()
        {
            Engine.Scene?.GlobalPreRender();
            base.PreRender();
        }
    }
}
