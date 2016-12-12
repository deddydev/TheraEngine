using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;
using System.Drawing;

namespace CustomEngine.Worlds.Actors.Components
{
    public class LightComponent : TRSComponent
    {
        ColorF4 _color = Color.White;
        float _intensity = 1.0f;
        private int _lightIndex = -1;

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
    }
}
