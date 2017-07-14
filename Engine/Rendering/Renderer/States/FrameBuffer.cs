using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class MaterialFrameBuffer : FrameBuffer
    {
        public MaterialFrameBuffer() { }
        public MaterialFrameBuffer(Material m) { Material = m; }

        private Material _material;
        public Material Material
        {
            get => _material;
            set
            {
                if (_material == value)
                    return;
                if (_material != null && _material.FrameBuffer == this)
                    _material.FrameBuffer = null;
                _material = value;
                if (_material != null)
                {
                    _material.FrameBuffer = this;
                    Compile();
                }
            }
        }
        public TextureReference[] Textures => Material?.TexRefs;
        public void ResizeTextures(int width, int height) => Material?.ResizeTextures(width, height);
        public void Compile()
        {
            if (Material == null)
                return;
            if (RenderPanel.NeedsInvoke(Compile, RenderPanel.PanelType.Game))
                return;
            Bind(EFramebufferTarget.Framebuffer);
            Material.GenerateTextures();
            Engine.Renderer.SetDrawBuffers(Material.FboAttachments);
            CheckErrors();
            Unbind(EFramebufferTarget.Framebuffer);
        }
    }
    public class FrameBuffer : BaseRenderState
    {
        public FrameBuffer() : base(EObjectType.Framebuffer) { }
        public void Bind(EFramebufferTarget type)
            => Engine.Renderer.BindFrameBuffer(type, BindingId);
        public void Unbind(EFramebufferTarget type)
            => Engine.Renderer.BindFrameBuffer(type, 0);
        public void CheckErrors()
            => Engine.Renderer.CheckFrameBufferErrors();
    }
}
