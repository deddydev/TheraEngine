using System;
using System.Drawing;
using TheraEngine.Core.Shapes;
using TheraEngine.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Timers;

namespace TheraEngine.Rendering.UI
{
    /// <summary>
    /// Houses a viewport that renders a scene from a designated camera.
    /// </summary>
    public class UIViewportComponent : UIMaterialRectangleComponent, I2DRenderable//, IPreRendered
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
                new BaseTexRef[]
                {
                    TexRef2D.CreateFrameBufferTexture("OutColor", 1, 1,
                        EPixelInternalFormat.Rgba16f,
                        EPixelFormat.Rgba, EPixelType.HalfFloat,
                        EFramebufferAttachment.ColorAttachment0),
                },
                Engine.LoadEngineShader("ViewportFBO.fs", EShaderMode.Fragment));
        }

        private void SetUniforms()
        {
            int fragId = Engine.Settings.AllowShaderPipelines ?
               _quad.Material.Program.BindingId :
               _quad.VertexFragProgram.BindingId;

            SettingUniforms?.Invoke(fragId);
        }

        public Camera ViewportCamera
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
        private RenderCommandViewport _renderCommand = new RenderCommandViewport();
        public override void AddRenderables(RenderPasses passes)
        {
            if (!IsVisible)
                return;
            _renderCommand.Primitives = _quad;
            _renderCommand.WorldMatrix = WorldMatrix;
            _renderCommand.NormalMatrix = Matrix3.Identity;
            _renderCommand.Viewport = _viewport;
            _renderCommand.ZIndex = 0;
            passes.Add(_renderCommand, RenderInfo.RenderPass);
        }
        public void PreRender(object sender, FrameEventArgs args)
        {
            if (!IsVisible)
                return;

            BaseScene scene = ViewportCamera?.OwningComponent?.OwningScene;
            _viewport.Update(scene, ViewportCamera, ViewportCamera.Frustum);
        }
        public override void OnSpawned()
        {
            Engine.RegisterTick(null, PreRender, SwapBuffers);
            base.OnSpawned();
        }

        private void SwapBuffers()
        {
            _viewport.SwapBuffers();
        }

        public override void OnDespawned()
        {
            Engine.UnregisterTick(null, PreRender, SwapBuffers);
            base.OnDespawned();
        }
    }
}
