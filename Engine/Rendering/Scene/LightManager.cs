using System.Collections.Generic;
using TheraEngine.Components.Scene.Lights;

namespace TheraEngine.Rendering
{
    public class LightManager : TObject
    {
        public List<SpotLightComponent> SpotLights { get; } = new List<SpotLightComponent>();
        public List<PointLightComponent> PointLights { get; } = new List<PointLightComponent>();
        public List<DirectionalLightComponent> DirectionalLights { get; } = new List<DirectionalLightComponent>();

        public bool RenderingShadowMaps { get; private set; } = false;

        internal void SetForwardLightingUniforms(RenderProgram program)
        {
            program.Uniform("DirLightCount", DirectionalLights.Count);
            program.Uniform("PointLightCount", PointLights.Count);
            program.Uniform("SpotLightCount", SpotLights.Count);

            for (int i = 0; i < DirectionalLights.Count; ++i)
                DirectionalLights[i].SetUniforms(program, $"DirLightData[{i}]");

            for (int i = 0; i < SpotLights.Count; ++i)
                SpotLights[i].SetUniforms(program, $"SpotLightData[{i}]");

            for (int i = 0; i < PointLights.Count; ++i)
                PointLights[i].SetUniforms(program, $"PointLightData[{i}]");
        }

        public void Add(DirectionalLightComponent light)
            => DirectionalLights.Add(light);
        public void Remove(DirectionalLightComponent light)
            => DirectionalLights.Remove(light);

        public void Add(SpotLightComponent light)
            => SpotLights.Add(light);
        public void Remove(SpotLightComponent light)
            => SpotLights.Remove(light);

        public void Add(PointLightComponent light)
            => PointLights.Add(light);
        public void Remove(PointLightComponent light)
            => PointLights.Remove(light);
        
        public void SwapBuffers()
        {
            foreach (DirectionalLightComponent l in DirectionalLights)
                l.SwapBuffers();

            foreach (SpotLightComponent l in SpotLights)
                l.SwapBuffers();

            foreach (PointLightComponent l in PointLights)
                l.SwapBuffers();
        }
        public void CollectShadowMaps(Scene3D scene)
        {
            foreach (DirectionalLightComponent l in DirectionalLights)
                l.CollectShadowMap(scene);

            foreach (SpotLightComponent l in SpotLights)
                l.CollectShadowMap(scene);

            foreach (PointLightComponent l in PointLights)
                l.CollectShadowMap(scene);
        }
        public void RenderShadowMaps(Scene3D scene)
        {
            RenderingShadowMaps = true;

            foreach (DirectionalLightComponent l in DirectionalLights)
                l.RenderShadowMap(scene);
            
            foreach (SpotLightComponent l in SpotLights)
                l.RenderShadowMap(scene);
            
            foreach (PointLightComponent l in PointLights)
                l.RenderShadowMap(scene);

            RenderingShadowMaps = false;
        }

        public void Clear()
        {
            SpotLights.Clear();
            PointLights.Clear();
            DirectionalLights.Clear();
        }
    }
}