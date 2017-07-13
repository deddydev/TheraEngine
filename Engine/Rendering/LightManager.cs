﻿using System;
using System.Collections.Generic;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds.Actors;

namespace TheraEngine.Rendering
{
    public class LightManager
    {
        public const int MaxPointLights = 16;
        public const int MaxSpotLights = 16;
        public const int MaxDirectionalLights = 2;

        private HashSet<DirectionalLightComponent> _directionalLights = new HashSet<DirectionalLightComponent>();
        private HashSet<SpotLightComponent> _spotLights = new HashSet<SpotLightComponent>();
        private HashSet<PointLightComponent> _pointLights = new HashSet<PointLightComponent>();
        
        internal void SetUniforms(int programBindingId)
        {
            Engine.Renderer.ProgramUniform(programBindingId, "GlobalAmbient", Engine.World.Settings.GlobalAmbient);
            Engine.Renderer.ProgramUniform(programBindingId, "DirLightCount", _directionalLights.Count.Clamp(0, MaxDirectionalLights));
            Engine.Renderer.ProgramUniform(programBindingId, "PointLightCount", _pointLights.Count.Clamp(0, MaxPointLights));
            Engine.Renderer.ProgramUniform(programBindingId, "SpotLightCount", _spotLights.Count.Clamp(0, MaxSpotLights));

            foreach (DirectionalLightComponent l in _directionalLights)
                l.SetUniforms(programBindingId);
            foreach (SpotLightComponent l in _spotLights)
                l.SetUniforms(programBindingId);
            foreach (PointLightComponent l in _pointLights)
                l.SetUniforms(programBindingId);
        }
        internal void Add(DirectionalLightComponent light)
        {
            if (_directionalLights.Count >= MaxDirectionalLights)
                return;
            light.LightIndex = _directionalLights.Count;
            _directionalLights.Add(light);
        }
        internal void Remove(DirectionalLightComponent light)
        {
            _directionalLights.Remove(light);
            light.LightIndex = -1;
        }
        internal void Add(SpotLightComponent light)
        {
            if (_spotLights.Count >= MaxSpotLights)
                return;
            light.LightIndex = _spotLights.Count;
            _spotLights.Add(light);
        }
        internal void Remove(SpotLightComponent light)
        {
            _spotLights.Remove(light);
            light.LightIndex = -1;
        }
        internal void Add(PointLightComponent light)
        {
            if (_pointLights.Count >= MaxPointLights)
                return;
            light.LightIndex = _pointLights.Count;
            _pointLights.Add(light);
        }
        internal void Remove(PointLightComponent light)
        {
            _pointLights.Remove(light);
            light.LightIndex = -1;
        }
        internal void RenderShadowMaps(SceneProcessor scene)
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
