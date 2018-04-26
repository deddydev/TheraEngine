using System.ComponentModel;
using System.Drawing;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Rendering;
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
        private LightType _type;

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
        [Browsable(false)]
        public int LightIndex
        {
            get => _lightIndex;
            internal set => _lightIndex = value;
        }
        protected LightType Type
        {
            get => _type;
            set => _type = value;
        }

        public LightComponent(ColorF3 color, float diffuseIntensity) : base()
        {
            _color = color;
            _diffuseIntensity = diffuseIntensity;
        }

        public override int GetHashCode() => LightIndex;

        protected RenderPasses _passes = new RenderPasses();
        internal void SwapBuffers() => _passes.SwapBuffers();
        public abstract void UpdateShadowMap(BaseScene scene);
        public abstract void RenderShadowMap(BaseScene scene);
        public abstract void BakeShadowMaps();
        public abstract void SetUniforms(int programBindingId);
    }
}
