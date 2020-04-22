using Extensions;
using System.ComponentModel;
using System.IO;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    /// <summary>
    /// Houses a viewport that renders a scene from a designated camera.
    /// </summary>
    public class UIViewportComponent : UIMaterialComponent, I2DRenderable, IPreRendered
    {
        public event DelSetUniforms SettingUniforms;

        private MaterialFrameBuffer _fbo;

        //These bools are to prevent infinite pre-rendering recursion
        private bool _updating = false;
        private bool _swapping = false;
        private bool _rendering = false;

        public UIViewportComponent() : base(GetViewportMaterial())
        {
            _fbo = new MaterialFrameBuffer(Material);
            RenderCommand.Mesh.SettingUniforms += SetUniforms;
        }

        private static TMaterial GetViewportMaterial()
            => new TMaterial("ViewportMat",
                new BaseTexRef[]
                {
                    TexRef2D.CreateFrameBufferTexture("OutColor", 1, 1,
                        EPixelInternalFormat.Rgba16f,
                        EPixelFormat.Rgba, EPixelType.HalfFloat,
                        EFramebufferAttachment.ColorAttachment0),
                },
                Engine.Files.Shader(Path.Combine("Common", "UnlitTexturedForward.fs"), EGLSLType.Fragment));

        private void SetUniforms(RenderProgram vertexProgram, RenderProgram materialProgram)
            => SettingUniforms?.Invoke(materialProgram);

        [Browsable(false)]
        public bool PreRenderEnabled => IsVisible && ViewportCamera?.OwningComponent?.OwningScene != null;
        [Browsable(false)]
        public virtual ICamera ViewportCamera
        {
            get => Viewport.AttachedCamera;
            set => Viewport.AttachedCamera = value;
        }
        [Category("Rendering")]
        public Viewport Viewport { get; private set; } = new Viewport(1, 1);

        protected override void OnResizeLayout(BoundingRectangleF parentRegion)
        {
            base.OnResizeLayout(parentRegion);

            int
                w = (int)ActualWidth.ClampMin(1.0f), 
                h = (int)ActualHeight.ClampMin(1.0f);
            
            Viewport.Resize(w, h);
            _fbo.Resize(w, h);
        }

        public void PreRenderUpdate(ICamera camera)
        {
            if (!IsVisible || _updating)
                return;

            _updating = true;
            Viewport.PreRenderUpdate();
            _updating = false;
        }
        public void PreRenderSwap()
        {
            if (!IsVisible || _swapping)
                return;

            _swapping = true;
            Viewport.PreRenderSwap();
            _swapping = false;
        }
        public void PreRender(Viewport viewport, ICamera camera)
        {
            if (!IsVisible || _rendering)
                return;

            _rendering = true;
            Viewport.Render(_fbo);
            _rendering = false;
        }
    }
}
