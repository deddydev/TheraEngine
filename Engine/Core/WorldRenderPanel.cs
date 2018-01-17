using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine
{
    /// <summary>
    /// Renders the engine's scene that the current world spawns in.
    /// </summary>
    public class WorldRenderPanel : RenderPanel<Scene3D>
    {
        protected override Scene3D GetScene(Viewport v) => Engine.Scene;
        protected override void PreRender()
        {
            if (Engine.Scene == null)
                return;

            Engine.Scene.Voxelize();
            Engine.Scene.RenderShadowMaps();
        }
    }
}
