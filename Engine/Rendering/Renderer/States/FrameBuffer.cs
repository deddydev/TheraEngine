using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class MaterialFrameBuffer : FrameBuffer
    {
        public MaterialFrameBuffer() { }
        public MaterialFrameBuffer(Material m) { Material = m; }

        private bool _compiled = false;

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
                    _compiled = false;
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
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, BindingId);
            Material.GenerateTextures();
            Engine.Renderer.SetDrawBuffers(Material.FboAttachments);
            CheckErrors();
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, 0);
            _compiled = true;
        }
        public override void Bind(EFramebufferTarget type)
        {
            if (!_compiled)
                Compile();
            base.Bind(type);
        }
    }
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
