﻿namespace TheraEngine.Rendering
{
    public class RenderBuffer : BaseRenderState, IFrameBufferAttachement
    {
        public RenderBuffer() : base(EObjectType.Renderbuffer) { }
        public void Bind() => Engine.Renderer.BindRenderBuffer(BindingId);
        public void Unbind() => Engine.Renderer.BindRenderBuffer(NullBindingId);
        public void SetStorage(ERenderBufferStorage type, int width, int height)
        {
            Bind();
            Engine.Renderer.RenderbufferStorage(type, width, height);
            Unbind();
        }
        public void AttachToFBO(FrameBuffer fbo, EFramebufferAttachment attachment)
            => AttachToFBO(fbo.BindingId, attachment);
        public void AttachToFBO(EFramebufferTarget fboType, EFramebufferAttachment attachment)
            => Engine.Renderer.FramebufferRenderBuffer(fboType, attachment, BindingId);
        public void AttachToFBO(int fboBindingId, EFramebufferAttachment attachment)
            => Engine.Renderer.FramebufferRenderBuffer(fboBindingId, attachment, BindingId);
    }
}
