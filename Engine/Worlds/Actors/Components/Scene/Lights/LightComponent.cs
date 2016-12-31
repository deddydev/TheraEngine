using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;
using System.Drawing;

namespace CustomEngine.Worlds.Actors.Components
{
    public enum LightType
    {
        //Does not move. Allows baking light into shadow maps.
        //Static,
        //Movable. Always calculates light for everything per-frame.
        Dynamic,
        //Moveable. Bakes into shadow maps when not moving.
        //DynamicCached,
    }
    public abstract class LightComponent : TRComponent
    {
        protected ColorF3 _color = Color.Blue/*, _specular = Color.Yellow*/;
        protected float _diffuseIntensity = 1.0f, _ambientIntensity = 0.2f;
        protected int _lightIndex = -1;
        protected LightType _type;

        public ColorF3 LightColor
        {
            get { return _color; }
            set { _color = value; }
        }
        public float DiffuseIntensity
        {
            get { return _diffuseIntensity; }
            set { _diffuseIntensity = value; }
        }
        public float AmbientIntensity
        {
            get { return _ambientIntensity; }
            set { _ambientIntensity = value; }
        }

        public int LightIndex
        {
            get { return _lightIndex; }
            internal set { _lightIndex = value; }
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
