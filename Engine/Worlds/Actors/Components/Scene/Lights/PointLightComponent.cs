using System;
using TheraEngine.Rendering.Models.Materials;
using System.ComponentModel;
using TheraEngine.Rendering;

namespace TheraEngine.Worlds.Actors
{
    public class PointLightComponent : LightComponent, I3DRenderable
    {
        [Category("Point Light Component")]
        public float Radius
        {
            get => _cullingVolume.Radius;
            set => _cullingVolume.Radius = value;
        }

        protected Sphere _cullingVolume;

        public bool HasTransparency => false;
        public Shape CullingVolume => _cullingVolume;
        public IOctreeNode OctreeNode { get; set; }
        public bool IsRendering { get; set; }
        
        public PointLightComponent(float radius, ColorF3 color, float diffuseIntensity, float ambientIntensity) 
            : base(color, diffuseIntensity, ambientIntensity)
        {
            _cullingVolume = new Sphere(radius);
        }

        protected override void OnWorldTransformChanged()
        {
            _cullingVolume.SetRenderTransform(WorldMatrix);
            base.OnWorldTransformChanged();
        }

        public override void OnSpawned()
        {
            if (_type == LightType.Dynamic)
                Engine.Scene.Lights.Add(this);
            if (Engine.Settings.RenderLights)
                Engine.Scene.Add(_cullingVolume);
        }
        public override void OnDespawned()
        {
            if (_type == LightType.Dynamic)
                Engine.Scene.Lights.Remove(this);
            if (Engine.Settings.RenderLights)
                Engine.Scene.Remove(_cullingVolume);
        }

        public override void SetUniforms(int programBindingId)
        {
            string indexer = Uniform.PointLightsName + "[" + _lightIndex + "].";
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.Color", _color.Raw);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.AmbientIntensity", _ambientIntensity);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.DiffuseIntensity", _diffuseIntensity);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Position", _cullingVolume.Center);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Radius", Radius);
        }

        public override void RenderShadowMap(SceneProcessor scene)
        {

        }

        public override void BakeShadowMaps()
        {

        }

        public void Render()
        {

        }
    }
}
