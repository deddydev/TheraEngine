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
    public class WorldRenderPanel : RenderPanel<SceneProcessor3D>
    {
        protected override SceneProcessor3D GetScene()
        {
            return Engine.Scene;
        }
        protected override void PreRender(SceneProcessor3D scene)
        {
            scene.Voxelize();
            scene.RenderShadowMaps();
        }
    }
}
