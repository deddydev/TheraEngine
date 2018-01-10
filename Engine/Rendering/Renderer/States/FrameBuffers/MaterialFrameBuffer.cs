using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class MaterialFrameBuffer : FrameBuffer
    {
        public MaterialFrameBuffer() { }
        public MaterialFrameBuffer(TMaterial m) { Material = m; }

        private bool _compiled = false;

        private TMaterial _material;
        public TMaterial Material
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
        public BaseTexRef[] Textures => Material?.Textures;
        public void ResizeTextures(int width, int height) => Material?.Resize2DTextures(width, height);
        public void Compile()
        {
            Compile(Material.FBODrawAttachments);
        }
        public void Compile(EDrawBuffersAttachment[] drawAttachments)
        {
            if (Material == null)
                return;
            if (BaseRenderPanel.NeedsInvoke(Compile, BaseRenderPanel.PanelType.Game))
                return;
            Material.GenerateTextures(true);
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, BindingId);
            foreach (BaseTexRef tref in Material.Textures)
            {
                tref.AttachToFBO();
            }
            Engine.Renderer.SetDrawBuffers(drawAttachments);
            Engine.Renderer.SetReadBuffer(EDrawBuffersAttachment.None);
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
}
