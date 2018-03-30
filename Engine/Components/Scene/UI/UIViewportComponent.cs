using System;
using System.Drawing;
using TheraEngine.Core.Shapes;
using TheraEngine.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    /// <summary>
    /// Houses a viewport that renders a scene from a designated camera.
    /// </summary>
    public class UIViewportComponent : UIMaterialRectangleComponent, I2DRenderable
    {
        public event DelSetUniforms SettingUniforms;

        private Viewport _viewport = new Viewport(1.0f, 1.0f);
        private MaterialFrameBuffer _fbo;

        public UIViewportComponent() : base(GetViewportMaterial())
        {
            _fbo = new MaterialFrameBuffer(InterfaceMaterial);
            _quad.SettingUniforms += SetUniforms;
        }

        private static TMaterial GetViewportMaterial()
        {
            return new TMaterial("ViewportMat",
                TMaterial.UniformRequirements.None,
                new ShaderVar[0],
                new BaseTexRef[]
                {
                    TexRef2D.CreateFrameBufferTexture("OutColor", 1, 1,
                        EPixelInternalFormat.Rgba16f,
                        EPixelFormat.Rgba, EPixelType.HalfFloat,
                        EFramebufferAttachment.ColorAttachment0),
                },
                Engine.LoadEngineShader("ViewportFBO.fs", ShaderMode.Fragment));
        }

        private void SetUniforms()
        {
            int fragId = Engine.Settings.AllowShaderPipelines ?
               _quad.Material.Program.BindingId :
               _quad.VertexFragProgram.BindingId;

            SettingUniforms?.Invoke(fragId);
        }

        public Camera Camera
        {
            get => _viewport.Camera;
            set => _viewport.Camera = value;
        }

        public override Vec2 Resize(Vec2 parentBounds)
        {
            Vec2 r = base.Resize(parentBounds);

            float 
                w = Width.ClampMin(1.0f), 
                h = Height.ClampMin(1.0f);

            _viewport.Resize(w, h, true, 1.0f, 1.0f);
            _fbo.ResizeTextures((int)w, (int)h);

            return r;
        }
        public override void Render()
        {
            if (!IsVisible)
                return;

            Scene scene = Camera?.OwningComponent?.OwningScene;
            _viewport.Render(scene, Camera, Camera.Frustum, _fbo);
            base.Render();
        }
    }
}
