using System;
using System.Collections.Generic;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class DeferredRenderPipeline : RenderPipeline
    {
        public TexRef2D DepthStencilTexture { get; private set; }
        public TexRefView2D DepthViewTexture { get; private set; }
        public TexRefView2D StencilViewTexture { get; private set; }

        public TexRef2D AlbedoOpacityTexture { get; private set; }
        public TexRef2D NormalTexture { get; private set; }
        public TexRef2D RMSITexture { get; private set; }

        public QuadFrameBuffer SSAOFBO { get; private set; }
        public QuadFrameBuffer SSAOBlurFBO { get; private set; }
        public QuadFrameBuffer GBufferFBO { get; private set; }
        
        public override void GenerateFBOs(Viewport viewport)
        {
            int width = viewport.InternalResolution.Width;
            int height = viewport.InternalResolution.Height;

            GenDepthStencil(width, height);
            GenDepthView();
            GenStencilView();
            GenAlbedoOpacity(width, height);
            GenNormal(width, height);
            GenRMSI(width, height);

            base.GenerateFBOs(viewport);
        }

        protected override void GenerateCommandChain(List<ViewportRenderCommand> commands, Viewport viewport)
        {
            GenStartCommands(commands);

            GenDeferredPassCommands(commands);

            commands.Add(new VPRC_BlitFBO(SSAOFBO, SSAOBlurFBO));
            commands.Add(new VPRC_BlitFBO(SSAOBlurFBO, GBufferFBO));
            commands.Add(new LightPassVPRC());
            commands.Add(new ForwardPassVPRC());

            GenEndCommands(commands);
        }

        private void GenStartCommands(List<ViewportRenderCommand> commands)
        {
            commands.Add(new VPRC_Basic(EVPRCommand.PushViewportCamera));
            commands.Add(new VPRC_Basic(EVPRCommand.PushViewportScene));
            commands.Add(new VPRC_Basic(EVPRCommand.PushViewportInternalResolutionRenderArea));
        }
        private void GenEndCommands(List<ViewportRenderCommand> commands)
        {
            commands.Add(new VPRC_Basic(EVPRCommand.PopRenderArea));
            commands.Add(new VPRC_Basic(EVPRCommand.PopScene));
            commands.Add(new VPRC_Basic(EVPRCommand.PopCamera));
        }
        private void GenDeferredPassCommands(List<ViewportRenderCommand> commands)
        {
            commands.Add(new VPRC_BindFBO(SSAOFBO, false, EFramebufferTarget.DrawFramebuffer));
            commands.Add(new VPRC_Custom(PreDeferredPassRender));
            commands.Add(new VRPC_RenderPass(ERenderPass.OpaqueDeferredLit));
            commands.Add(new VRPC_RenderPass(ERenderPass.DeferredDecals));
            commands.Add(new VPRC_Custom(PostDeferredPassRender));
            commands.Add(new VPRC_BindFBO(SSAOFBO, true, EFramebufferTarget.DrawFramebuffer));
        }

        private void PreDeferredPassRender(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
        {
            Engine.Renderer.StencilMask(~0);
            Engine.Renderer.ClearStencil(0);
            Engine.Renderer.Clear(EFBOTextureType.Color | EFBOTextureType.Depth | EFBOTextureType.Stencil);
            Engine.Renderer.EnableDepthTest(true);
            Engine.Renderer.ClearDepth(1.0f);
        }
        private void PostDeferredPassRender(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
        {
            Engine.Renderer.EnableDepthTest(false);
        }

        private void GenRMSI(int width, int height)
            => SetTexture(RMSITexture = TexRef2D.CreateFrameBufferTexture(
                "RoughnessMetallicSpecularUnused",
                width,
                height,
                EPixelInternalFormat.Rgba8,
                EPixelFormat.Rgba,
                EPixelType.UnsignedByte));

        private void GenNormal(int width, int height)
            => SetTexture(NormalTexture = TexRef2D.CreateFrameBufferTexture(
                "Normal",
                width,
                height,
                EPixelInternalFormat.Rgb16f,
                EPixelFormat.Rgb,
                EPixelType.HalfFloat));

        private void GenAlbedoOpacity(int width, int height)
            => SetTexture(AlbedoOpacityTexture = TexRef2D.CreateFrameBufferTexture(
                "AlbedoOpacity",
                width,
                height,
                EPixelInternalFormat.Rgba16f,
                EPixelFormat.Rgba,
                EPixelType.HalfFloat));

        private void GenDepthStencil(int width, int height)
        {
            DepthStencilTexture = TexRef2D.CreateFrameBufferTexture(
                "DepthStencil",
                width,
                height,
                EPixelInternalFormat.Depth24Stencil8,
                EPixelFormat.DepthStencil,
                EPixelType.UnsignedInt248,
                EFramebufferAttachment.DepthStencilAttachment);

            DepthStencilTexture.MinFilter = ETexMinFilter.Nearest;
            DepthStencilTexture.MagFilter = ETexMagFilter.Nearest;
            DepthStencilTexture.Resizable = false;

            SetTexture(DepthStencilTexture);
        }

        private void GenDepthView() 
            => SetTexture(DepthViewTexture = new TexRefView2D(
                "DepthView",
                DepthStencilTexture,
                0, 1, 0, 1,
                EPixelType.UnsignedInt248,
                EPixelFormat.DepthStencil,
                EPixelInternalFormat.Depth24Stencil8)
                {
                    Resizable = false,
                    DepthStencilFormat = EDepthStencilFmt.Depth,
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.ClampToEdge,
                    VWrap = ETexWrapMode.ClampToEdge,
                });

        private void GenStencilView()
            => SetTexture(StencilViewTexture = new TexRefView2D(
                "StencilView",
                DepthStencilTexture,
                0, 1, 0, 1,
                EPixelType.UnsignedInt248,
                EPixelFormat.DepthStencil,
                EPixelInternalFormat.Depth24Stencil8)
                {
                    Resizable = false,
                    DepthStencilFormat = EDepthStencilFmt.Stencil,
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.ClampToEdge,
                    VWrap = ETexWrapMode.ClampToEdge,
                });
    }
}
