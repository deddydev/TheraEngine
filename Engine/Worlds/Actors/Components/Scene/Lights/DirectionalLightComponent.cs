using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Worlds.Actors.Components
{
    public class DirectionalLightComponent : LightComponent
    {
        public Vec3 Direction
        {
            get { return _rotation.GetDirection(); }
            set { _rotation.SetDirection(value.NormalizedFast()); }
        }

        public DirectionalLightComponent() : base() { }

        public override void OnSpawned()
        {
            Engine.Renderer.Scene.LightManager.AddLight(this);
        }
        public override void OnDespawned()
        {
            Engine.Renderer.Scene.LightManager.RemoveLight(this);
        }
    }
}
