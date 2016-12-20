using CustomEngine.Rendering.Cameras;
using CustomEngine.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering
{
    public class LightManager
    {
        private HashSet<DirectionalLightComponent> _directionalLights = new HashSet<DirectionalLightComponent>();
        private HashSet<SpotLightComponent> _spotLights = new HashSet<SpotLightComponent>();
        private HashSet<PointLightComponent> _pointLights = new HashSet<PointLightComponent>();
        
        internal void SetUniforms()
        {
            foreach (DirectionalLightComponent l in _directionalLights)
                l.SetUniforms();
            foreach (SpotLightComponent l in _spotLights)
                l.SetUniforms();
            foreach (PointLightComponent l in _pointLights)
                l.SetUniforms();
        }
        public void AddLight(DirectionalLightComponent light)
        {
            light.LightIndex = _directionalLights.Count;
            _directionalLights.Add(light);
        }
        public void RemoveLight(DirectionalLightComponent light)
        {
            _directionalLights.Remove(light);
            light.LightIndex = -1;
        }
        public void AddLight(SpotLightComponent light)
        {
            light.LightIndex = _spotLights.Count;
            _spotLights.Add(light);
        }
        public void RemoveLight(SpotLightComponent light)
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
