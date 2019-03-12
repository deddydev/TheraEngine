using System;
using System.ComponentModel;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    public interface IInteractableUI
    {

    }
    /// <summary>
    /// Houses a viewport that renders a scene from a designated camera.
    /// </summary>
    public class UIViewportComponent : UIMaterialRectangleComponent, I2DRenderable, IPreRendered, IInteractableUI
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
                Engine.Files.LoadEngineShader("ViewportFBO.fs", EGLSLType.Fragment));
        }

        private void SetUniforms(RenderProgram vertexProgram, RenderProgram materialProgram)
        {
            SettingUniforms?.Invoke(materialProgram);
        }

        [Browsable(false)]
        public bool PreRenderEnabled => IsVisible && ViewportCamera?.OwningComponent?.OwningScene != null;
        [Browsable(false)]
        public virtual Camera ViewportCamera
        {
            get => Viewport.Camera;
            set => Viewport.Camera = value;
        }
        [Category("Rendering")]
        public Viewport Viewport { get; private set; } = new Viewport(1, 1);

        public override Vec2 Resize(Vec2 parentBounds)
        {
            Vec2 r = base.Resize(parentBounds);

            int 
                w = (int)Width.ClampMin(1.0f), 
                h = (int)Height.ClampMin(1.0f);

            Viewport.Resize(w, h);
            _fbo.Resize(w, h);

            return r;
        }

        public void PreRenderUpdate(Camera camera)
        {
            if (_updating)
                return;

            Camera c = ViewportCamera;
            if (!IsVisible || c == null)
                return;
            
            _updating = true;

            BaseScene scene = c.OwningComponent?.OwningScene;
            scene?.PreRenderUpdate(c);
            Viewport.Update(scene, c, c.Frustum);

            _updating = false;
        }
        public void PreRenderSwap()
        {
            if (_swapping)
                return;

            Camera c = ViewportCamera;
            if (!IsVisible || c == null)
                return;

            _swapping = true;

            BaseScene scene = ViewportCamera?.OwningComponent?.OwningScene;
            scene?.PreRenderSwap();
            Viewport.SwapBuffers();

            _swapping = false;
        }
        public void PreRender(Viewport viewport, Camera camera)
        {
            if (_rendering)
                return;

            Camera c = ViewportCamera;
            if (!IsVisible || c == null)
                return;
            
            _rendering = true;

            BaseScene scene = c.OwningComponent?.OwningScene;
            scene?.PreRender(Viewport, c);
            Viewport.Render(scene, c, _fbo);

            _rendering = false;
        }
    }
}
