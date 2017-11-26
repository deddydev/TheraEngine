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
    public class EditorWorldRenderPanel : WorldRenderPanel
    {
        protected override Camera GetCamera(Viewport v)
        {
            return v.Camera;
        }
        protected override Frustum GetFrustum(Viewport v)
        {
            return v.Camera.Frustum;
        }
    }
}
