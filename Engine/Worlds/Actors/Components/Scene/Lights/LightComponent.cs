using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;
using System.Drawing;

namespace CustomEngine.Worlds.Actors
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
        protected ColorF3 _color = Color.Blue/*, _specular = Color.Yellow*/;
        protected float _diffuseIntensity = 1.0f, _ambientIntensity = 0.2f;
        protected int _lightIndex = -1;
        protected LightType _type;

        public ColorF3 LightColor
        {
            get => _color;
            set => _color = value;
        }
        public float DiffuseIntensity
        {
            get => _diffuseIntensity;
            set => _diffuseIntensity = value;
        }
        public float AmbientIntensity
        {
            get => _ambientIntensity;
            set => _ambientIntensity = value;
        }

        public int LightIndex
        {
            get => _lightIndex;
            internal set => _lightIndex = value;
        }
        
        public LightComponent(ColorF3 color, float diffuseIntensity, float ambientIntensity) : base()
        {
            _color = color;
            _diffuseIntensity = diffuseIntensity;
            _ambientIntensity = ambientIntensity;
        }

        public override int GetHashCode()
        {
            return LightIndex;
        }

        public void BakeShadowMaps()
        {

        }

        public abstract void SetUniforms();
    }
}
