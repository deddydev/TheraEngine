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
        protected ColorF4 _color = Color.White, _specular = Color.Black;
        protected int _lightIndex = -1;
        protected LightType _type;

        public int LightIndex
        {
            get { return _lightIndex; }
            internal set { _lightIndex = value; }
        }
        
        public LightComponent() : base() { }

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
