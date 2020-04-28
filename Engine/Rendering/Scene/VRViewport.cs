using System;
using System.IO;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Scene
{
    public class VRViewport : Viewport
    {
        public VRViewport() : base(new VRRenderHandler(), 0) => Resize(1080, 1200);

        public QuadFrameBuffer FBO { get; set; }
        public TexRef2D EyeTexture { get; set; }

        protected void InitOutputFBO()
        {
            RenderingParameters renderParams = new RenderingParameters()
            {
                DepthTest =
                {
                    Enabled = ERenderParamUsage.Unchanged,
                    UpdateDepth = false,
                    Function = EComparison.Always,
                }
            };

            GLSLScript shader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "HudFBO.fs"), EGLSLType.Fragment);

            EyeTexture = TexRef2D.CreateFrameBufferTexture(
                "VREyeTex",
                InternalResolution.Extents,
                EPixelInternalFormat.Rgba8, 
                EPixelFormat.Rgba,
                EPixelType.UnsignedByte);

            FBO = new QuadFrameBuffer(new TMaterial("VREyeMat", renderParams, new TexRef2D[] { EyeTexture }, shader));
            FBO.SetRenderTargets((EyeTexture, EFramebufferAttachment.ColorAttachment0, 0, -1));
        }
        public IntPtr VRRender()
        {
            if (FBO is null)
                InitOutputFBO();
            
            Render(FBO);

            int bindingId = EyeTexture.GetTexture(true).BindingId;

            return (IntPtr)bindingId;
        }
    }
}
