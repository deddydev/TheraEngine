using System.Collections.Concurrent;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class LightManager : TObject
    {
        public ConcurrentHashSet<SpotLightComponent> SpotLights { get; } 
            = new ConcurrentHashSet<SpotLightComponent>();
        public ConcurrentHashSet<PointLightComponent> PointLights { get; } 
            = new ConcurrentHashSet<PointLightComponent>();
        public ConcurrentHashSet<DirectionalLightComponent> DirectionalLights { get; } 
            = new ConcurrentHashSet<DirectionalLightComponent>();

        public ColorF3 GlobalAmbient { get; set; }
        public bool RenderingShadowMaps { get; private set; } = false;

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
        public void Remove(DirectionalLightComponent light) => DirectionalLights.TryRemove(light);

        public void Add(SpotLightComponent light)           => SpotLights.Add(light);
        public void Remove(SpotLightComponent light)        => SpotLights.TryRemove(light);

        public void Add(PointLightComponent light)          => PointLights.Add(light);
        public void Remove(PointLightComponent light)       => PointLights.TryRemove(light);
        
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
    }
}