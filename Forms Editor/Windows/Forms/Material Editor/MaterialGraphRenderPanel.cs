using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Actors.Types.Pawns;

namespace TheraEngine
{
    public class MaterialGraphRenderPanel : RenderPanel<Scene2D>
    {
        public UIManager UI { get; set; }
        protected override Scene2D GetScene(Viewport v) => UI?.Scene;
        protected override Camera GetCamera(Viewport v) => UI?.Camera;
    }
}