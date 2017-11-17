using System;
using System.Drawing;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Worlds.Actors.Components.Scene.Transforms;

namespace TheraEngine.Worlds.Actors.Components.Scene.Lights
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
    [FileClass("clight", "Light Component")]
    public abstract class LightComponent : TRComponent
    {
        protected EventColorF3 _color = (ColorF3)Color.Blue/*, _specular = Color.Yellow*/;
        protected float _diffuseIntensity = 1.0f, _ambientIntensity = 0.2f;
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
        [Category("Light Component")]
        public float AmbientIntensity
        {
            get => _ambientIntensity;
            set => _ambientIntensity = value;
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

        public LightComponent(ColorF3 color, float diffuseIntensity, float ambientIntensity) : base()
        {
            _color = color;
            _diffuseIntensity = diffuseIntensity;
            _ambientIntensity = ambientIntensity;
        }

        public override int GetHashCode()
            => LightIndex;
        
        public abstract void RenderShadowMap(SceneProcessor scene);
        public abstract void BakeShadowMaps();
        public abstract void SetUniforms(int programBindingId);
    }
}
