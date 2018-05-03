using System.Collections.Generic;
using System.Linq;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Rendering
{
    public delegate void DelSetUniforms(int programBindingId);
    public interface IFrameBufferAttachement { }
    public class FrameBuffer : BaseRenderState
    {
        public FrameBuffer() : base(EObjectType.Framebuffer) { }
        
        public (IFrameBufferAttachement Target, EFramebufferAttachment Attachment, int MipLevel, int LayerIndex)[] Targets { get; private set; }
        public EDrawBuffersAttachment[] DrawBuffers { get; private set; }
        
        public void SetRenderTargets(params (IFrameBufferAttachement Target, EFramebufferAttachment Attachment, int MipLevel, int LayerIndex)[] textures)
        {
            if (IsActive)
                DetachAll();

            Targets = textures;

            List<EDrawBuffersAttachment> fboAttachments = new List<EDrawBuffersAttachment>();
            foreach (var (Target, Attachment, MipLevel, LayerIndex) in Targets)
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
            
            List<(IFrameBufferAttachement Target, EFramebufferAttachment Attachment, int MipLevel, int LayerIndex)> targets
                = new List<(IFrameBufferAttachement Target, EFramebufferAttachment Attachment, int MipLevel, int LayerIndex)>();

            foreach (BaseTexRef t in material.Textures.Where(x => x.FrameBufferAttachment.HasValue))
                targets.Add((t, t.FrameBufferAttachment.Value, 0, -1));
            
            Targets = targets.ToArray();

            List<EDrawBuffersAttachment> fboAttachments = new List<EDrawBuffersAttachment>();
            foreach (var (Texture, Attachment, MipLevel, LayerIndex) in Targets)
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
            foreach (var (Target, Attachment, MipLevel, LayerIndex) in Targets)
            {
                if (Target is BaseTexRef tref)
                {
                    BaseRenderTexture t = tref.GetTextureGeneric(true);
                    t.PushData();
                    t.Bind();
                    if (LayerIndex >= 0 && tref is TexRefCube cuberef)
                    {
                        cuberef.AttachFaceToFBO(EFramebufferTarget.Framebuffer, Attachment, ECubemapFace.PosX + LayerIndex, MipLevel);
                    }
                    else
                    {
                        tref.AttachToFBO(EFramebufferTarget.Framebuffer, Attachment, MipLevel);
                    }
                }
                else if (Target is RenderBuffer buf)
                {
                    buf.Bind();
                    buf.AttachToFBO(EFramebufferTarget.Framebuffer, Attachment);
                }
            }
            Engine.Renderer.SetDrawBuffers(DrawBuffers);
            Engine.Renderer.SetReadBuffer(EDrawBuffersAttachment.None);
            CheckErrors();
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, 0);
        }

        public void DetachAll()
        {
            if (BaseRenderPanel.NeedsInvoke(DetachAll, BaseRenderPanel.PanelType.World))
                return;
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, BindingId);
            foreach (var (Target, Attachment, MipLevel, LayerIndex) in Targets)
            {
                if (Target is BaseTexRef tref)
                {
                    tref.GetTextureGeneric(true).PushData();
                    if (LayerIndex >= 0 && tref is TexRefCube cuberef)
                    {
                        cuberef.DetachFaceFromFBO(EFramebufferTarget.Framebuffer, Attachment, ECubemapFace.PosX + LayerIndex, 0);
                    }
                    else
                    {
                        tref.DetachFromFBO(EFramebufferTarget.Framebuffer, Attachment, MipLevel);
                    }
                }
                else if (Target is RenderBuffer buf)
                {
                    buf.DetachFromFBO(EFramebufferTarget.Framebuffer, Attachment);
                }
                Engine.Renderer.AttachTextureToFrameBuffer(EFramebufferTarget.Framebuffer, Attachment, NullBindingId, MipLevel);
            }

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
            foreach (var (Texture, Attachment, MipLevel, LayerIndex) in Targets)
                if (Texture is TexRef2D t2d)
                    t2d.Resize(width, height);
        }
        protected override void PostGenerated() => AttachAll();
    }
}
