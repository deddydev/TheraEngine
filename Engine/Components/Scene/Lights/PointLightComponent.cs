﻿using System;
using TheraEngine.Rendering.Models.Materials;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Worlds.Actors.Components.Scene.Lights
{
    [FileDef("Point Light Component")]
    public class PointLightComponent : LightComponent
    {
        [Category("Point Light Component")]
        public float Radius
        {
            get => _cullingVolume.Radius;
            set => _cullingVolume.Radius = value;
        }

        [Browsable(false)]
        public RenderInfo3D RenderInfo => ((I3DRenderable)_cullingVolume).RenderInfo;
        [Browsable(false)]
        public Shape CullingVolume => ((I3DRenderable)_cullingVolume).CullingVolume;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get => ((I3DRenderable)_cullingVolume).OctreeNode; set => ((I3DRenderable)_cullingVolume).OctreeNode = value; }

        private Sphere _cullingVolume;
        
        public PointLightComponent() : this(100.0f, new ColorF3(1.0f, 1.0f, 1.0f), 1.0f, 0.0f) { }
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
            if (Type == LightType.Dynamic)
                Engine.Scene.Lights.Add(this);
        }
        public override void OnDespawned()
        {
            if (Type == LightType.Dynamic)
                Engine.Scene.Lights.Remove(this);
        }

        public override void SetUniforms(int programBindingId)
        {
            string indexer = Uniform.PointLightsName + "[" + _lightIndex + "].";
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Color", _color.Raw);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.AmbientIntensity", _ambientIntensity);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.DiffuseIntensity", _diffuseIntensity);
            Engine.Renderer.Uniform(programBindingId, indexer + "Position", _cullingVolume.Center);
            Engine.Renderer.Uniform(programBindingId, indexer + "Radius", Radius);
        }

        public override void RenderShadowMap(Scene3D scene)
        {

        }

        public override void BakeShadowMaps()
        {

        }
        
#if EDITOR
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (selected)
                Engine.Scene.Add(_cullingVolume);
            else
                Engine.Scene.Remove(_cullingVolume);
            base.OnSelectedChanged(selected);
        }
    }
#endif
}
