using System;
using System.IO;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Scene
{
    public class VRViewport : Viewport
    {
        public VRViewport() : base(new VRRenderHandler(), 0) => Resize(1080, 1200);

        public MaterialFrameBuffer FBO { get; set; }
        public TexRef2D EyeTexture { get; set; }

        protected void InitOutputFBO()
        {
            FBO = new MaterialFrameBuffer(new TMaterial("VREyeMat",
                new BaseTexRef[]
                {
                    EyeTexture = TexRef2D.CreateFrameBufferTexture(
                        "VREyeTex", 
                        InternalResolution.Width,
                        InternalResolution.Height,
                        EPixelInternalFormat.Rgba,
                        EPixelFormat.Bgra,
                        EPixelType.UnsignedByte,
                        EFramebufferAttachment.ColorAttachment0),
                },
                Engine.Files.Shader(Path.Combine("Common", "UnlitTexturedForward.fs"), EGLSLType.Fragment)));
        }
        public IntPtr VRRender()
        {
            if (!FBOsInitialized || FBO is null)
                InitOutputFBO();
            
            Render(FBO);

            int bindingId = EyeTexture.GetTexture(true).BindingId;

            return (IntPtr)bindingId;
        }
    }
}
