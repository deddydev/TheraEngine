using System;
using System.IO;
using TheraEngine.Core;
using TheraEngine.Rendering.Models.Materials;
using Valve.VR;

namespace TheraEngine.Rendering.Scene
{
    public class VRViewport : Viewport
    {
        public VRViewport() : base(new VRRenderHandler(), 0) { }

        private VRTextureBounds_t _eyeTexBounds = new VRTextureBounds_t()
        {
            uMin = 0.0f,
            uMax = 1.0f,
            vMin = 0.0f,
            vMax = 1.0f,
        };

        private Texture_t _eyeTex = new Texture_t
        {
            eColorSpace = EColorSpace.Auto,
            eType = Valve.VR.ETextureType.OpenGL,
        };

        public EVREye EyeTarget { get; set; }
        public bool IsLeftEye
        {
            get => EyeTarget == EVREye.Eye_Left;
            set => EyeTarget = value ? EVREye.Eye_Left : EVREye.Eye_Right;
        }

        private TexRef2D _eyeTexture;
        public TexRef2D EyeTexture
        {
            get => _eyeTexture;
            set => Set(ref _eyeTexture, value, BeforeSet, AfterSet);
        }

        private void BeforeSet()
        {
            var rtex = EyeTexture?.GetRenderTextureGeneric(true);
            if (rtex != null)
                rtex.Generated -= VRViewport_Generated;
        }
        private void AfterSet()
        {
            var rtex = EyeTexture?.GetRenderTextureGeneric(true);
            if (rtex != null)
            {
                if (rtex.IsActive)
                    VRViewport_Generated();
                else
                    rtex.Generated += VRViewport_Generated;
            }
        }

        protected override void OnInitializeFBOs()
        {
            DefaultRenderTarget = new MaterialFrameBuffer(new TMaterial("MatVREyeFBO",
                new BaseTexRef[]
                {
                    EyeTexture = TexRef2D.CreateFrameBufferTexture(
                        "TexVREyeFBO",
                        InternalResolution.Width,
                        InternalResolution.Height,
                        EPixelInternalFormat.Rgba,
                        EPixelFormat.Bgra,
                        EPixelType.UnsignedByte,
                        EFramebufferAttachment.ColorAttachment0),
                },
                Engine.Files.Shader(Path.Combine("Common", "UnlitTexturedForward.fs"), EGLSLType.Fragment)));
        }

        private void VRViewport_Generated()
        {
            int bindingId = EyeTexture?.GetRenderTextureGeneric(true)?.BindingId ?? 0;
            //if (bindingId > 0)
                _eyeTex.handle = (IntPtr)bindingId;
        }

        public Matrix4 GetEyeProjectionMatrix(float nearZ, float farZ)
            => OpenVR.System.GetProjectionMatrix(EyeTarget, nearZ, farZ);

        public Matrix4 GetEyeToHeadTransform()
            => OpenVR.System.GetEyeToHeadTransform(EyeTarget);

        public void Submit(EVRSubmitFlags flags = EVRSubmitFlags.Submit_Default)
            => EngineVR.CheckError(OpenVR.Compositor.Submit(EyeTarget, ref _eyeTex, ref _eyeTexBounds, flags)); //Engine.Out($"Submitted {(IsLeftEye ? "left" : "right")} eye");
    }
}
