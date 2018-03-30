using EnumsNET;
using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public delegate void DelSetUniforms(int programBindingId);
    
    public class FrameBuffer : BaseRenderState
    {
        public FrameBuffer() : base(EObjectType.Framebuffer) { }
        
        public (BaseTexRef Texture, EFramebufferAttachment Attachment, int MipLevel)[] Targets { get; private set; }
        public EFramebufferAttachment[] Attachments { get; private set; }
        public EDrawBuffersAttachment[] DrawBuffers { get; private set; }

        private EDrawBuffersAttachment _readBuffer = EDrawBuffersAttachment.None;
        public EDrawBuffersAttachment ReadBuffer
        {
            get => _readBuffer;
            set
            {
                _readBuffer = value;
                if (IsActive)
                {
                    Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, BindingId);
                    Engine.Renderer.SetReadBuffer(ReadBuffer);
                    CheckErrors();
                    Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, 0);
                }
            }
        }

        public void SetRenderTargets(params (BaseTexRef Texture, EFramebufferAttachment Attachment, int MipLevel)[] textures)
        {
            if (IsActive)
                DetachAll();

            Targets = textures;

            List<EDrawBuffersAttachment> fboAttachments = new List<EDrawBuffersAttachment>();
            foreach (var (Texture, Attachment, MipLevel) in Targets)
            {
                switch (Attachment)
                {
                    case EFramebufferAttachment.Color:
                    case EFramebufferAttachment.Depth:
                    case EFramebufferAttachment.DepthAttachment:
                    case EFramebufferAttachment.DepthStencilAttachment:
                    case EFramebufferAttachment.Stencil:
                    case EFramebufferAttachment.StencilAttachment:
                        continue;
                }
                fboAttachments.Add((EDrawBuffersAttachment)(int)Attachment);
            }
            DrawBuffers = fboAttachments.ToArray();

            if (IsActive)
                AttachAll();
        }
        public void SetRenderTargets(TMaterial material)
        {
            if (IsActive)
                DetachAll();
            
            List<(BaseTexRef Texture, EFramebufferAttachment Attachment, int MipLevel)> targets
                = new List<(BaseTexRef Texture, EFramebufferAttachment Attachment, int MipLevel)>();
            foreach (BaseTexRef t in material.Textures.Where(x => x.FrameBufferAttachment.HasValue))
                targets.Add((t, t.FrameBufferAttachment.Value, 0));
            Targets = targets.ToArray();

            List<EDrawBuffersAttachment> fboAttachments = new List<EDrawBuffersAttachment>();
            foreach (var (Texture, Attachment, MipLevel) in Targets)
            {
                switch (Attachment)
                {
                    case EFramebufferAttachment.Color:
                    case EFramebufferAttachment.Depth:
                    case EFramebufferAttachment.DepthAttachment:
                    case EFramebufferAttachment.DepthStencilAttachment:
                    case EFramebufferAttachment.Stencil:
                    case EFramebufferAttachment.StencilAttachment:
                        continue;
                }
                fboAttachments.Add((EDrawBuffersAttachment)(int)Attachment);
            }
            DrawBuffers = fboAttachments.ToArray();

            if (IsActive)
                AttachAll();
        }

        public void AttachAll()
        {
            if (BaseRenderPanel.NeedsInvoke(AttachAll, BaseRenderPanel.PanelType.World))
                return;
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, BindingId);
            foreach (var (Texture, Attachment, MipLevel) in Targets)
            {
                Texture.GetTextureGeneric(true).PushData();
                Texture.AttachToFBO(Attachment, MipLevel);
            }
            Engine.Renderer.SetDrawBuffers(DrawBuffers);
            Engine.Renderer.SetReadBuffer(ReadBuffer);
            CheckErrors();
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, 0);
        }

        public void DetachAll()
        {
            if (BaseRenderPanel.NeedsInvoke(DetachAll, BaseRenderPanel.PanelType.World))
                return;
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, BindingId);
            foreach (var (Texture, Attachment, MipLevel) in Targets)
                Engine.Renderer.AttachTextureToFrameBuffer(EFramebufferTarget.Framebuffer, Attachment, NullBindingId, MipLevel);
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, 0);
        }

        public virtual void Bind(EFramebufferTarget type)
            => Engine.Renderer.BindFrameBuffer(type, BindingId);
        public virtual void Unbind(EFramebufferTarget type)
            => Engine.Renderer.BindFrameBuffer(type, 0);
        public void CheckErrors()
            => Engine.Renderer.CheckFrameBufferErrors();
        public void ResizeTextures(int width, int height)
        {
            foreach (var (Texture, Attachment, MipLevel) in Targets)
                if (Texture is TexRef2D t2d)
                    t2d.Resize(width, height);
        }
        protected override void PostGenerated() => AttachAll();
    }
}
