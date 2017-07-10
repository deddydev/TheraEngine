using System;

namespace TheraEngine.Rendering
{
    public class RenderBuffer : BaseRenderState
    {
        public RenderBuffer() : base(EObjectType.Renderbuffer) { }

        public void Bind()
        {
            if (!IsActive)
                Generate();
            Engine.Renderer.BindRenderBuffer(BindingId);
        }
        public void Unbind()
        {
            Engine.Renderer.BindRenderBuffer(0);
        }
        public void SetStorage(ERenderBufferStorage type, int width, int height)
        {
            Engine.Renderer.RenderbufferStorage(type, width, height);
        }
        public void AttachToFrameBuffer(EFramebufferTarget fboType, EFramebufferAttachment attachment)
        {
            //Engine.Renderer.AttachRenderBufferToFrameBuffer(fboType, attachment, BindingId);
        }
        public void AttachToFrameBuffer(int fboBindingId, EFramebufferAttachment attachment)
        {
            //Engine.Renderer.AttachRenderBufferToFrameBuffer(fboBindingId, attachment, BindingId);
        }
        public void AttachToFrameBuffer(FrameBuffer fbo, EFramebufferAttachment attachment)
        {
            //Engine.Renderer.AttachRenderBufferToFrameBuffer(fbo.BindingId, attachment, BindingId);
        }
    }
}
