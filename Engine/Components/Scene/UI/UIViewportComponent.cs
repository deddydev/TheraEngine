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
            _fbo = new MaterialFrameBuffer(InterfaceMaterial);
            _fbo.Generate();
            //_quad.SettingUniforms += SetUniforms;
        }

        private static TMaterial GetViewportMaterial()
        {
            return new TMaterial("ViewportMat",
                new RenderingParameters(),
                new ShaderVar[0],
                new BaseTexRef[]
                {
                    TexRef2D.CreateFrameBufferTexture("OutColor", 1, 1,
                        EPixelInternalFormat.Rgba16f,
                        EPixelFormat.Rgba, EPixelType.HalfFloat,
                        EFramebufferAttachment.ColorAttachment0),
                    TexRef2D.CreateFrameBufferTexture("Depth", 1, 1,
                        EPixelInternalFormat.DepthComponent32f, 
                        EPixelFormat.DepthComponent, EPixelType.Float,
                        EFramebufferAttachment.DepthAttachment),
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

        private Camera _camera;
        public Camera Camera
        {
            get => _camera;
            set
            {
                _camera = value;
                _camera?.Resize(Width, Height);
            }
        }

        private Viewport _viewport;
        private MaterialFrameBuffer _fbo;
        
        public override Vec2 Resize(Vec2 parentBounds)
        {
            Vec2 r = base.Resize(parentBounds);
            _viewport.Resize(Width, Height, true, 1.0f, 1.0f);
            _fbo.ResizeTextures((int)Width, (int)Height);
            Camera?.Resize(Width, Height);
            return r;
        }
        public override void Render()
        {
            Scene scene = Camera?.OwningComponent?.OwningScene;
            _fbo.Bind(EFramebufferTarget.DrawFramebuffer);
            Engine.Renderer.PushRenderArea(new BoundingRectangle(0.0f, 0.0f, Width, Height, 0.0f, 0.0f));

            Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
            Engine.Renderer.AllowDepthWrite(true);

            scene.CollectVisibleRenderables(Camera.Frustum, false);
            scene.Render(Camera, null, null);
            Engine.Renderer.PopRenderArea();
            _fbo.Unbind(EFramebufferTarget.DrawFramebuffer);
            base.Render();
        }
    }
}
