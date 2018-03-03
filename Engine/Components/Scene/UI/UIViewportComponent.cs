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

        public UIViewportComponent() : base(GetViewportMaterial())
        {
            _viewport = new Viewport(1.0f, 1.0f);
            _fbo = new QuadFrameBuffer(InterfaceMaterial);
            //_quad.SettingUniforms += SetUniforms;
        }

        private static TMaterial GetViewportMaterial()
        {
            return new TMaterial("ViewportMat",
                new RenderingParameters(),
                new ShaderVar[0],
                new BaseTexRef[]
                {
                    TexRef2D.CreateFrameBufferTexture("OutColor", 1, 1, EPixelInternalFormat.Rgba16f,
                    EPixelFormat.Rgba, EPixelType.HalfFloat, EFramebufferAttachment.ColorAttachment0),
                },
                Engine.LoadEngineShader("ViewportFBO.fs", ShaderMode.Fragment));
        }

        //private void SetUniforms()
        //{
        //    int fragId = Engine.Settings.AllowShaderPipelines ?
        //       _quad.Material.Program.BindingId :
        //       _quad.VertexFragProgram.BindingId;

        //    SettingUniforms?.Invoke(fragId);
        //}

        public Camera Camera { get; set; }

        private Viewport _viewport;
        private QuadFrameBuffer _fbo;
        
        public override Vec2 Resize(Vec2 parentBounds)
        {
            Vec2 r = base.Resize(parentBounds);
            //_fbo.ResizeTextures((int)Width, (int)Height);
            //_viewport.Resize(Width, Height, true, 1.0f, 1.0f);
            return r;
        }
        public override void Render()
        {
            //Scene scene = Camera?.OwningComponent?.OwningScene;
            //_viewport.Render(scene, Camera, Camera?.Frustum, _fbo);
            //_fbo.Render();
            base.Render();
        }
    }
}
