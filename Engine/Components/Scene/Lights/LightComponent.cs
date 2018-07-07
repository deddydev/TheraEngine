using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Components.Scene.Lights
{
    public enum LightType
    {
        //Movable. Always calculates light for everything per-frame.
        Dynamic,
        //Moveable. Bakes into shadow maps when not moving.
        DynamicCached,
        //Does not move. Allows baking light into shadow maps.
        Static,
    }
    public abstract class LightComponent : TRComponent
    {
        protected EventColorF3 _color = (ColorF3)Color.White;
        protected float _diffuseIntensity = 1.0f;
        protected int _lightIndex = -1;
        protected RenderPasses _passes = new RenderPasses();

        protected internal Matrix4 LightMatrix { get; protected set; }
        protected internal MaterialFrameBuffer ShadowMap { get; protected set; }
        protected internal Camera ShadowCamera { get; protected set; }

        protected BoundingRectangle _region = new BoundingRectangle();

        [Category("Light Component")]
        public int Width
        {
            get => _region.Width;
            set
            {
                _region.Width = value;
            }
        }
        [Category("Light Component")]
        public int Height
        {
            get => _region.Height;
            set
            {
                _region.Height = value;
            }
        }
        [Category("Light Component")]
        public EventColorF3 LightColor
        {
            get => _color;
            set => _color = value;
        }
        [Category("Light Component")]
        public float DiffuseIntensity
        {
            get => _diffuseIntensity;
            set => _diffuseIntensity = value;
        }
        protected LightType Type { get; set; } = LightType.Dynamic;

        public LightComponent(ColorF3 color, float diffuseIntensity) : base()
        {
            _color = color;
            _diffuseIntensity = diffuseIntensity;
        }
        
        internal void SwapBuffers() => _passes.SwapBuffers();

        public abstract void SetUniforms(RenderProgram program);
        protected virtual IVolume GetShadowVolume() => ShadowCamera.Frustum;

        public void UpdateShadowMap(BaseScene scene)
        {
            scene.Update(_passes, GetShadowVolume(), ShadowCamera, null, true);
        }

        public void RenderShadowMap(BaseScene scene)
        {
            if (ShadowMap == null)
                return;

            Engine.Renderer.MaterialOverride = ShadowMap.Material;
            ShadowMap.Bind(EFramebufferTarget.DrawFramebuffer);
            Engine.Renderer.PushRenderArea(_region);
            {
                Engine.Renderer.ClearDepth(1.0f);
                Engine.Renderer.EnableDepthTest(true);
                Engine.Renderer.AllowDepthWrite(true);
                Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                scene.Render(_passes, ShadowCamera, null, null, null);
            }
            Engine.Renderer.PopRenderArea();
            ShadowMap.Unbind(EFramebufferTarget.DrawFramebuffer);
            Engine.Renderer.MaterialOverride = null;
        }
        public static EPixelInternalFormat GetShadowMapFormat(EDepthPrecision precision)
        {
            switch (precision)
            {
                case EDepthPrecision.Int16: return EPixelInternalFormat.DepthComponent16;
                case EDepthPrecision.Int24: return EPixelInternalFormat.DepthComponent24;
                case EDepthPrecision.Int32: return EPixelInternalFormat.DepthComponent32;
            }
            return EPixelInternalFormat.DepthComponent32f;
        }
    }
}
