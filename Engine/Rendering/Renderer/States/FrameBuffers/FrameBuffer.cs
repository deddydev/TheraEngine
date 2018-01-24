namespace TheraEngine.Rendering
{
    public delegate void DelSetUniforms(int programBindingId);
    
    public class FrameBuffer : BaseRenderState
    {
        public FrameBuffer() : base(EObjectType.Framebuffer) { }
        public virtual void Bind(EFramebufferTarget type)
            => Engine.Renderer.BindFrameBuffer(type, BindingId);
        public virtual void Unbind(EFramebufferTarget type)
            => Engine.Renderer.BindFrameBuffer(type, 0);
        public void CheckErrors()
            => Engine.Renderer.CheckFrameBufferErrors();
    }
}
