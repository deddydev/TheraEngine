using TheraEngine.Core;

namespace TheraEngine.Rendering.Scene
{
    public class VRRenderHandler : BaseRenderHandler
    {
        public override void PreRenderUpdate()
        {
            EngineVR.PreRenderUpdate();
        }
        public override void SwapBuffers()
        {
            EngineVR.SwapBuffers();
        }
        public override void Render()
        {
            EngineVR.Render();
        }
    }
}