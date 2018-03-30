using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class MaterialFrameBuffer : FrameBuffer
    {
        public MaterialFrameBuffer() { }
        public MaterialFrameBuffer(TMaterial m) => Material = m;
        
        private TMaterial _material;
        public TMaterial Material
        {
            get => _material;
            set
            {
                if (_material == value)
                    return;
                _material = value;
                SetRenderTargets(_material);
            }
        }

        //public void Compile()
        //{
        //    Compile(Material.CollectFBOAttachments());
        //}
        //public void Compile(EDrawBuffersAttachment[] drawAttachments)
        //{
        //    if (Material == null)
        //        return;
        //    if (BaseRenderPanel.NeedsInvoke(Compile, BaseRenderPanel.PanelType.World))
        //        return;
        //    Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, BindingId);
        //    Material.GenerateTextures(true);
        //    foreach (BaseTexRef tref in Material.Textures)
        //        tref.AttachToFBO();
        //    Engine.Renderer.SetDrawBuffers(drawAttachments);
        //    Engine.Renderer.SetReadBuffer(EDrawBuffersAttachment.None);
        //    CheckErrors();
        //    Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, 0);
        //    Engine.PrintLine("COMPILED FBO " + BindingId);
        //}
        //protected override void PostGenerated()
        //{
        //    Compile();
        //}
    }
}
