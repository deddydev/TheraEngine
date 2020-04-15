using System;
using System.Threading;
using TheraEngine.Rendering.Scene;

namespace TheraEngine.Rendering
{
    public class VRWrapperContext : RenderContext
    {
        public VRWrapperContext()
        {
            Handler = new VRRenderHandler();
        }

        public override AbstractRenderer Renderer
            => VRRenderHandler?.TrueContext?.Renderer;
        public VRRenderHandler VRRenderHandler
            => Handler as VRRenderHandler;

        public override void BeginDraw()
            => VRRenderHandler?.TrueContext?.BeginDraw();
        public override void EndDraw()
            => VRRenderHandler?.TrueContext?.EndDraw();
        public override void ErrorCheck()
            => VRRenderHandler?.TrueContext?.ErrorCheck();
        public override void Flush()
            => VRRenderHandler?.TrueContext?.Flush();
        public override void Initialize()
            => VRRenderHandler?.TrueContext?.Initialize();
        protected internal override ThreadSubContext CreateSubContext(IntPtr? handle, Thread thread)
            => VRRenderHandler?.TrueContext?.CreateSubContext(handle, thread);
        internal override void AfterRender()
            => VRRenderHandler?.TrueContext?.AfterRender();
        internal override void BeforeRender()
            => VRRenderHandler?.TrueContext?.BeforeRender();
    }
}
