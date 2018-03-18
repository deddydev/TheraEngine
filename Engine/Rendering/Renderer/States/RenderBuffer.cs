namespace TheraEngine.Rendering
{
    public class RenderBuffer : BaseRenderState
    {
        public RenderBuffer() : base(EObjectType.Renderbuffer) { }
        public void Bind() => Engine.Renderer.BindRenderBuffer(BindingId);
        public void Unbind() => Engine.Renderer.BindRenderBuffer(NullBindingId);
        public void SetStorage(ERenderBufferStorage type, int width, int height)
            => Engine.Renderer.RenderbufferStorage(type, width, height);
        public void AttachToFrameBuffer(FrameBuffer fbo, EFramebufferAttachment attachment)
            => AttachToFrameBuffer(fbo.BindingId, attachment);
        public void AttachToFrameBuffer(EFramebufferTarget fboType, EFramebufferAttachment attachment)
            => Engine.Renderer.FramebufferRenderBuffer(fboType, attachment, BindingId);
        public void AttachToFrameBuffer(int fboBindingId, EFramebufferAttachment attachment)
            => Engine.Renderer.FramebufferRenderBuffer(fboBindingId, attachment, BindingId);
    }
}
