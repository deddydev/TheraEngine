using System.Collections.Generic;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Rendering
{
    public class LightManager : TObject
    {
        public const int MaxPointLights = 1;
        public const int MaxSpotLights = 3;
        public const int MaxDirectionalLights = 1;

        private ColorF3 _globalAmbient;
        private HashSet<DirectionalLightComponent> _directionalLights = new HashSet<DirectionalLightComponent>();
        private HashSet<SpotLightComponent> _spotLights = new HashSet<SpotLightComponent>();
        private HashSet<PointLightComponent> _pointLights = new HashSet<PointLightComponent>();
        
        public HashSet<SpotLightComponent> SpotLights => _spotLights;
        public HashSet<PointLightComponent> PointLights => _pointLights;
        public HashSet<DirectionalLightComponent> DirectionalLights => _directionalLights;

        public ColorF3 GlobalAmbient
        {
            get => _globalAmbient;
            set => _globalAmbient = value;
        }

        internal void SetUniforms(int programBindingId)
        {
            Engine.Renderer.Uniform(programBindingId, "GlobalAmbient", _globalAmbient);
            Engine.Renderer.Uniform(programBindingId, "DirLightCount", _directionalLights.Count);
            Engine.Renderer.Uniform(programBindingId, "PointLightCount", _pointLights.Count);
            Engine.Renderer.Uniform(programBindingId, "SpotLightCount", _spotLights.Count);

            foreach (DirectionalLightComponent l in _directionalLights)
                l.SetUniforms(programBindingId);
            foreach (SpotLightComponent l in _spotLights)
                l.SetUniforms(programBindingId);
            foreach (PointLightComponent l in _pointLights)
                l.SetUniforms(programBindingId);
        }
        public void Add(DirectionalLightComponent light)
        {
            if (_directionalLights.Count >= MaxDirectionalLights)
            {
                Engine.LogWarning("Exceeded maximum directional lights.");
                return;
            }
            light.LightIndex = _directionalLights.Count;
            _directionalLights.Add(light);

            //if (Engine.Settings.RenderLights)
            //    Engine.Scene.Add(light.ShadowCamera);
        }
        public void Remove(DirectionalLightComponent light)
        {
            _directionalLights.Remove(light);
            light.LightIndex = -1;

            //if (Engine.Settings.RenderLights)
            //    Engine.Scene.Remove(light.ShadowCamera);
        }
        public void Add(SpotLightComponent light)
        {
            if (_spotLights.Count >= MaxSpotLights)
            {
                Engine.LogWarning("Exceeded maximum spotlights.");
                return;
            }
            light.LightIndex = _spotLights.Count;
            _spotLights.Add(light);

            //if (Engine.Settings.RenderLights)
            //{
            //    Engine.Scene.Add(light.OuterCone);
            //    Engine.Scene.Add(light.InnerCone);
            //}
        }
        public void Remove(SpotLightComponent light)
        {
            _spotLights.Remove(light);
            light.LightIndex = -1;

            //if (Engine.Settings.RenderLights)
            //{
            //    Engine.Scene.Remove(light.OuterCone);
            //    Engine.Scene.Remove(light.InnerCone);
            //}
        }
        public void Add(PointLightComponent light)
        {
            if (_pointLights.Count >= MaxPointLights)
            {
                Engine.LogWarning("Exceeded maximum point lights.");
                return;
            }
            light.LightIndex = _pointLights.Count;
            _pointLights.Add(light);

            //if (Engine.Settings.RenderLights)
            //    Engine.Scene.Add(light._cullingVolume);
        }
        public void Remove(PointLightComponent light)
        {
            _pointLights.Remove(light);
            light.LightIndex = -1;

            //if (Engine.Settings.RenderLights)
            //    Engine.Scene.Remove(light._cullingVolume);
        }
        public void SwapBuffers()
        {
            foreach (DirectionalLightComponent l in _directionalLights)
                l.SwapBuffers();

            foreach (SpotLightComponent l in _spotLights)
                l.SwapBuffers();

            foreach (PointLightComponent l in _pointLights)
                l.SwapBuffers();
        }
        public void UpdateShadowMaps(Scene3D scene)
        {
            foreach (DirectionalLightComponent l in _directionalLights)
                l.UpdateShadowMap(scene);

            foreach (SpotLightComponent l in _spotLights)
                l.UpdateShadowMap(scene);

            foreach (PointLightComponent l in _pointLights)
                l.UpdateShadowMap(scene);
        }
        public void RenderShadowMaps(Scene3D scene)
        {
            foreach (DirectionalLightComponent l in _directionalLights)
                l.RenderShadowMap(scene);
            
            foreach (SpotLightComponent l in _spotLights)
                l.RenderShadowMap(scene);
            
            foreach (PointLightComponent l in _pointLights)
                l.RenderShadowMap(scene);
        }
    }
}