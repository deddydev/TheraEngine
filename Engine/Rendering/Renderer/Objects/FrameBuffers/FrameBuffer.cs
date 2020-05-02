using System.Collections.Generic;
using System.Linq;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Rendering
{
    public delegate void DelSetUniforms(RenderProgram program);
    public interface IFrameBufferAttachement { }
    public class FrameBuffer : BaseRenderObject
    {
        public FrameBuffer() : base(EObjectType.Framebuffer) { }

        public static FrameBuffer CurrentlyBound { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public EFBOTextureType TextureTypes { get; private set; } = EFBOTextureType.None;

        public (IFrameBufferAttachement Target, EFramebufferAttachment Attachment, int MipLevel, int LayerIndex)[] Targets { get; private set; }
        public EDrawBuffersAttachment[] DrawBuffers { get; private set; }
        
        public void UpdateRenderTarget(int i, (IFrameBufferAttachement Target, EFramebufferAttachment Attachment, int MipLevel, int LayerIndex) target)
        {
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, BindingId);

            if (IsActive)
                Detach(i);

            Targets[i] = target;

            if (IsActive)
                Attach(i);

            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, NullBindingId);
        }
        /// <summary>
        /// Informs the framebuffer where it is writing shader output data to;
        /// any combination of textures and renderbuffers.
        /// </summary>
        /// <param name="targets">The array of targets to render to.
        /// <list type="bullet">
        /// <item><description><see cref="IFrameBufferAttachement"/> Target: the <see cref="BaseTexRef"/> or <see cref="RenderBuffer"/> to render to.</description></item>
        /// <item><description><see cref="EFramebufferAttachment"/> Attachment: which shader output to capture.</description></item>
        /// <item><description><see cref="int"/> MipLevel: the level of detail to write to.</description></item>
        /// <item><description><see cref="int"/> LayerIndex: the layer to write to. For example, a cubemap has 6 layers, one for each face.</description></item>
        /// </list>
        /// </param>
        public void SetRenderTargets(params (IFrameBufferAttachement Target, EFramebufferAttachment Attachment, int MipLevel, int LayerIndex)[] targets)
        {
            if (IsActive)
                DetachAll();

            Targets = targets;
            TextureTypes = EFBOTextureType.None;

            List<EDrawBuffersAttachment> fboAttachments = new List<EDrawBuffersAttachment>();
            foreach (var (_, Attachment, _, _) in Targets)
            {
                switch (Attachment)
                {
                    case EFramebufferAttachment.Color:
                        TextureTypes |= EFBOTextureType.Color;
                        continue;
                    case EFramebufferAttachment.Depth:
                    case EFramebufferAttachment.DepthAttachment:
                        TextureTypes |= EFBOTextureType.Depth;
                        continue;
                    case EFramebufferAttachment.DepthStencilAttachment:
                        TextureTypes |= EFBOTextureType.Depth | EFBOTextureType.Stencil;
                        continue;
                    case EFramebufferAttachment.Stencil:
                    case EFramebufferAttachment.StencilAttachment:
                        TextureTypes |= EFBOTextureType.Stencil;
                        continue;
                }
                fboAttachments.Add((EDrawBuffersAttachment)(int)Attachment);
                TextureTypes |= EFBOTextureType.Color;
            }

            DrawBuffers = fboAttachments.ToArray();

            if (IsActive)
                AttachAll();
        }
        /// <summary>
        /// Informs the framebuffer where it is writing shader output data to;
        /// either a texture or a renderbuffer.
        /// </summary>
        /// <param name="targets">The targets to render to.</param>
        /// </summary>
        /// <param name="material"></param>
        public void SetRenderTargets(TMaterial material)
        {
            SetRenderTargets(material.Textures.
                Where(x => x?.FrameBufferAttachment != null).
                Select(x => ((IFrameBufferAttachement)x, x.FrameBufferAttachment.Value, 0, -1)).ToArray());

            //if (IsActive)
            //    DetachAll();
            
            //List<(IFrameBufferAttachement Target, EFramebufferAttachment Attachment, int MipLevel, int LayerIndex)> targets
            //    = new List<(IFrameBufferAttachement Target, EFramebufferAttachment Attachment, int MipLevel, int LayerIndex)>();

            //foreach (BaseTexRef t in material.Textures.Where(x => x.FrameBufferAttachment.HasValue))
            //    targets.Add((t, t.FrameBufferAttachment.Value, 0, -1));
            
            //Targets = targets.ToArray();

            //List<EDrawBuffersAttachment> fboAttachments = new List<EDrawBuffersAttachment>();
            //foreach (var (Texture, Attachment, MipLevel, LayerIndex) in Targets)
            //{
            //    switch (Attachment)
            //    {
            //        case EFramebufferAttachment.Color:
            //        case EFramebufferAttachment.Depth:
            //        case EFramebufferAttachment.DepthAttachment:
            //        case EFramebufferAttachment.DepthStencilAttachment:
            //        case EFramebufferAttachment.Stencil:
            //        case EFramebufferAttachment.StencilAttachment:
            //            continue;
            //    }
            //    fboAttachments.Add((EDrawBuffersAttachment)(int)Attachment);
            //}
            //DrawBuffers = fboAttachments.ToArray();

            //if (IsActive)
            //    AttachAll();
        }
        public void AttachAll()
        {
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, BindingId);

            if (Targets != null)
                for (int i = 0; i < Targets.Length; ++i)
                    Attach(i);

            Engine.Renderer.SetDrawBuffers(DrawBuffers);
            Engine.Renderer.SetReadBuffer(EDrawBuffersAttachment.None);

            CheckErrors();

            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, NullBindingId);
        }
        public void DetachAll()
        {
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, BindingId);

            if (Targets != null)
                for (int i = 0; i < Targets.Length; ++i)
                    Detach(i);

            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, NullBindingId);
        }
        public void Attach(int i)
        {
            var (Target, Attachment, MipLevel, LayerIndex) = Targets[i];
            if (Target is BaseTexRef tref)
            {
                BaseRenderTexture t = tref.GetRenderTextureGeneric(true);

                t.PushData();
                t.Bind();

                if (LayerIndex >= 0 && tref is TexRefCube cuberef)
                    cuberef.AttachFaceToFBO(EFramebufferTarget.Framebuffer, Attachment, ECubemapFace.PosX + LayerIndex, MipLevel);
                else
                    tref.AttachToFBO(EFramebufferTarget.Framebuffer, Attachment, MipLevel);
            }
            else if (Target is RenderBuffer buf)
            {
                buf.Bind();
                buf.AttachToFBO(EFramebufferTarget.Framebuffer, Attachment);
            }
        }
        public void Detach(int i)
        {
            var (Target, Attachment, MipLevel, LayerIndex) = Targets[i];
            if (Target is BaseTexRef tref)
            {
                tref.GetRenderTextureGeneric(true).PushData();

                if (LayerIndex >= 0 && tref is TexRefCube cuberef)
                    cuberef.DetachFaceFromFBO(EFramebufferTarget.Framebuffer, Attachment, ECubemapFace.PosX + LayerIndex, MipLevel);
                else
                    tref.DetachFromFBO(EFramebufferTarget.Framebuffer, Attachment, MipLevel);
            }
            else if (Target is RenderBuffer buf)
            {
                buf.Bind();
                buf.DetachFromFBO(EFramebufferTarget.Framebuffer, Attachment);
            }
        }

        public virtual void Bind(EFramebufferTarget type)
        {
            //if (Width <= 0 || Height <= 0)
            //{
            //    Engine.LogWarning("Can't bind an FBO with no area.");
            //    return;
            //}
            Engine.Renderer.BindFrameBuffer(type, BindingId);
            CurrentlyBound = this;
        }
        public virtual void Unbind(EFramebufferTarget type)
        {
            Engine.Renderer.BindFrameBuffer(type, NullBindingId);
            CurrentlyBound = null;
        }

        public void CheckErrors()
            => Engine.Renderer.CheckFrameBufferErrors();

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            foreach (var (Texture, Attachment, MipLevel, LayerIndex) in Targets)
                if (Texture is TexRef2D t2d)
                    t2d.Resize(width, height);
        }
        protected override void PostGenerated() => AttachAll();
    }
}
