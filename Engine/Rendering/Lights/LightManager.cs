using System.Collections.Generic;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Components.Scene.Lights;

namespace TheraEngine.Rendering
{
    public class LightManager : TObject
    {
        public const int MaxPointLights = 1;
        public const int MaxSpotLights = 3;
        public const int MaxDirectionalLights = 1;

        public HashSet<SpotLightComponent> SpotLights { get; } = new HashSet<SpotLightComponent>();
        public HashSet<PointLightComponent> PointLights { get; } = new HashSet<PointLightComponent>();
        public HashSet<DirectionalLightComponent> DirectionalLights { get; } = new HashSet<DirectionalLightComponent>();

        public ColorF3 GlobalAmbient { get; set; }

        //internal void SetUniforms(RenderProgram program)
        //{
        //    //Engine.Renderer.Uniform(programBindingId, "GlobalAmbient", _globalAmbient);
        //    //Engine.Renderer.Uniform(programBindingId, "DirLightCount", _directionalLights.Count);
        //    //Engine.Renderer.Uniform(programBindingId, "PointLightCount", _pointLights.Count);
        //    //Engine.Renderer.Uniform(programBindingId, "SpotLightCount", _spotLights.Count);

        //    foreach (DirectionalLightComponent l in DirectionalLights)
        //        l.SetUniforms(program);
        //    foreach (SpotLightComponent l in SpotLights)
        //        l.SetUniforms(program);
        //    foreach (PointLightComponent l in PointLights)
        //        l.SetUniforms(program);
        //}

        public void Add(DirectionalLightComponent light)    => DirectionalLights.Add(light);
        public void Remove(DirectionalLightComponent light) => DirectionalLights.Remove(light);

        public void Add(SpotLightComponent light)           => SpotLights.Add(light);
        public void Remove(SpotLightComponent light)        => SpotLights.Remove(light);

        public void Add(PointLightComponent light)          => PointLights.Add(light);
        public void Remove(PointLightComponent light)       => PointLights.Remove(light);
        
        public void SwapBuffers()
        {
            foreach (DirectionalLightComponent l in DirectionalLights)
                l.SwapBuffers();

            foreach (SpotLightComponent l in SpotLights)
                l.SwapBuffers();

            foreach (PointLightComponent l in PointLights)
                l.SwapBuffers();
        }
        public void UpdateShadowMaps(Scene3D scene)
        {
            foreach (DirectionalLightComponent l in DirectionalLights)
                l.UpdateShadowMap(scene);

            foreach (SpotLightComponent l in SpotLights)
                l.UpdateShadowMap(scene);

            foreach (PointLightComponent l in PointLights)
                l.UpdateShadowMap(scene);
        }
        public void RenderShadowMaps(Scene3D scene)
        {
            foreach (DirectionalLightComponent l in DirectionalLights)
                l.RenderShadowMap(scene);
            
            foreach (SpotLightComponent l in SpotLights)
                l.RenderShadowMap(scene);
            
            foreach (PointLightComponent l in PointLights)
                l.RenderShadowMap(scene);
        }
    }
}