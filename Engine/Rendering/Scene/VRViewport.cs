using System;
using System.IO;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Scene
{
    public class VRViewport : Viewport
    {
        public VRViewport() : base(null, 0) => SetInternalResolution(1080, 1200);

        public QuadFrameBuffer FBO { get; set; }
        public TexRef2D EyeTexture { get; set; }

        protected internal override void InitializeFBOs()
        {
            RegeneratingFBOs = true;

            ClearFBOs();

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
            EyeTexture = TexRef2D.CreateFrameBufferTexture("VREyeTex", InternalResolution.Extents,
                EPixelInternalFormat.Rgba16f, EPixelFormat.Rgba, EPixelType.HalfFloat);
            TexRef2D[] texRefs = new TexRef2D[] { EyeTexture };
            TMaterial mat = new TMaterial("VREyeMat", renderParams, texRefs, shader);
            FBO = new QuadFrameBuffer(mat);
            FBO.SetRenderTargets((EyeTexture, EFramebufferAttachment.ColorAttachment0, 0, -1));

            RegeneratingFBOs = false;
            FBOsInitialized = true;
        }
        public IntPtr VRRender()
        {
            FullRender(FBO);
            return (IntPtr)EyeTexture.GetTexture(true).BindingId;
        }
    }
}
