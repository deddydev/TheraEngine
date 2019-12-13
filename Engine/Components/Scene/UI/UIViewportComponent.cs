using Extensions;
using System;
using System.ComponentModel;
using System.IO;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    /// <summary>
    /// Houses a viewport that renders a scene from a designated camera.
    /// </summary>
    public class UIViewportComponent : UIMaterialRectangleComponent, I2DRenderable, IPreRendered
    {
        public event DelSetUniforms SettingUniforms;

        private MaterialFrameBuffer _fbo;

        //These bools are to prevent infinite pre-rendering recursion
        private bool _updating = false;
        private bool _swapping = false;
        private bool _rendering = false;

        public UIViewportComponent() : base(GetViewportMaterial())
        {
            _fbo = new MaterialFrameBuffer(InterfaceMaterial);
            RenderCommand.Mesh.SettingUniforms += SetUniforms;
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
                Engine.Files.Shader(Path.Combine("Common", "UnlitTexturedForward.fs"), EGLSLType.Fragment));
        }

        private void SetUniforms(RenderProgram vertexProgram, RenderProgram materialProgram)
        {
            SettingUniforms?.Invoke(materialProgram);
        }

        [Browsable(false)]
        public bool PreRenderEnabled => IsVisible && ViewportCamera?.OwningComponent?.OwningScene != null;
        [Browsable(false)]
        public virtual ICamera ViewportCamera
        {
            get => Viewport.Camera;
            set => Viewport.Camera = value;
        }
        [Category("Rendering")]
        public Viewport Viewport { get; private set; } = new Viewport(1, 1);

        public override Vec2 OnResize(Vec2 parentBounds)
        {
            Vec2 r = base.OnResize(parentBounds);

            int 
                w = (int)Width.ClampMin(1.0f), 
                h = (int)Height.ClampMin(1.0f);

            Viewport.Resize(w, h);
            _fbo.Resize(w, h);

            return r;
        }

        public void PreRenderUpdate(ICamera camera)
        {
            if (_updating)
                return;

            ICamera c = ViewportCamera;
            if (!IsVisible || c is null)
                return;
            
            _updating = true;

            IScene scene = c.OwningComponent?.OwningScene;
            scene?.PreRenderUpdate(c);
            Viewport.Update(scene, c, c.Frustum);

            _updating = false;
        }
        public void PreRenderSwap()
        {
            if (_swapping)
                return;

            ICamera c = ViewportCamera;
            if (!IsVisible || c is null)
                return;

            _swapping = true;

            IScene scene = ViewportCamera?.OwningComponent?.OwningScene;
            scene?.PreRenderSwap();
            Viewport.SwapBuffers();

            _swapping = false;
        }
        public void PreRender(Viewport viewport, ICamera camera)
        {
            if (_rendering)
                return;

            ICamera c = ViewportCamera;
            if (!IsVisible || c is null)
                return;
            
            _rendering = true;

            IScene scene = c.OwningComponent?.OwningScene;
            scene?.PreRender(Viewport, c);
            Viewport.Render(scene, c, _fbo);

            _rendering = false;
        }
    }
}
