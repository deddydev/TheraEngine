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
    public class MaterialGraphRenderPanel : RenderPanel<SceneProcessor2D>
    {
        public SceneProcessor2D Scene { get; } = new SceneProcessor2D();
        protected override SceneProcessor2D GetScene() => Scene;
    }
}
