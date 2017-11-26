using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds.Actors.Types.Pawns;

namespace TheraEngine
{
    public class MaterialGraphRenderPanel : RenderPanel<Scene2D>
    {
        public UIManager UI { get; set; }
        protected override Scene2D GetScene(Viewport v) => UI?.Scene;
        protected override Camera GetCamera(Viewport v) => UI?.Camera;
    }
}