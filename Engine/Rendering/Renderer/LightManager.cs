using CustomEngine.Rendering.Cameras;
using CustomEngine.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors;

namespace CustomEngine.Rendering
{
    public class LightManager
    {
        public const int MaxPointLights = 16;
        public const int MaxSpotLights = 16;
        public const int MaxDirectionalLights = 2;

        private HashSet<DirectionalLightComponent> _directionalLights = new HashSet<DirectionalLightComponent>();
        private HashSet<SpotLightComponent> _spotLights = new HashSet<SpotLightComponent>();
        private HashSet<PointLightComponent> _pointLights = new HashSet<PointLightComponent>();
        
        internal void SetUniforms()
        {
            Engine.Renderer.Uniform("DirLightCount", _directionalLights.Count.Clamp(0, MaxDirectionalLights));
            Engine.Renderer.Uniform("PointLightCount", _pointLights.Count.Clamp(0, MaxPointLights));
            Engine.Renderer.Uniform("SpotLightCount", _spotLights.Count.Clamp(0, MaxSpotLights));

            foreach (DirectionalLightComponent l in _directionalLights)
                l.SetUniforms();
            foreach (SpotLightComponent l in _spotLights)
                l.SetUniforms();
            foreach (PointLightComponent l in _pointLights)
                l.SetUniforms();
        }
        public void Add(DirectionalLightComponent light)
        {
            light.LightIndex = _directionalLights.Count;
            _directionalLights.Add(light);
        }
        public void Remove(DirectionalLightComponent light)
        {
            _directionalLights.Remove(light);
            light.LightIndex = -1;
        }
        public void Add(SpotLightComponent light)
        {
            light.LightIndex = _spotLights.Count;
            _spotLights.Add(light);
        }
        public void Remove(SpotLightComponent light)
        {
            _spotLights.Remove(light);
            light.LightIndex = -1;
        }
        public void Add(PointLightComponent light)
        {
            light.LightIndex = _pointLights.Count;
            _pointLights.Add(light);
        }
        public void Remove(PointLightComponent light)
        {
            _pointLights.Remove(light);
            light.LightIndex = -1;
        }
    }
}
