using System;
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
            _renderCommand = new RenderCommandViewport
            {
                Primitives = _quad,
                NormalMatrix = Matrix3.Identity,
                Viewport = _viewport,
                ZIndex = 0,
                Framebuffer = _fbo
            };
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

        private void SetUniforms(int vertexBindingId, int fragGeomBindingId)
        {
            SettingUniforms?.Invoke(fragGeomBindingId);
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
        private RenderCommandViewport _renderCommand;
        public override void AddRenderables(RenderPasses passes)
        {
            if (!IsVisible)
                return;
            _renderCommand.WorldMatrix = WorldMatrix;
            passes.Add(_renderCommand, RenderInfo.RenderPass);
        }
        public void Update(object sender, FrameEventArgs args)
        {
            if (!IsVisible)
                return;

            BaseScene scene = ViewportCamera?.OwningComponent?.OwningScene;
            _viewport.Update(scene, ViewportCamera, ViewportCamera.Frustum);
        }
        public override void OnSpawned()
        {
            Engine.RegisterTick(null, Update, SwapBuffers);
            base.OnSpawned();
        }

        private void SwapBuffers()
        {
            BaseScene scene = ViewportCamera?.OwningComponent?.OwningScene;
            _viewport.SwapBuffers(scene);
        }

        public override void OnDespawned()
        {
            Engine.UnregisterTick(null, Update, SwapBuffers);
            base.OnDespawned();
        }
    }
}
