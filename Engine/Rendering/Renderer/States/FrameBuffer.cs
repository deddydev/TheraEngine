namespace TheraEngine.Rendering
{
    public class FrameBuffer : BaseRenderState
    {
        public FrameBuffer() : base(EObjectType.Framebuffer) { }

        public void Bind(EFramebufferTarget type) { Engine.Renderer.BindFrameBuffer(type, BindingId); }
        public void Unbind(EFramebufferTarget type) { Engine.Renderer.BindFrameBuffer(type, 0); }
    }
}
