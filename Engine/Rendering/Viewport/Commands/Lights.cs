using TheraEngine.Components.Scene.Lights;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering
{
    public class LightPassVPRC : ViewportFBORenderCommand
    {
        public override void Execute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
        {
            //Viewport light combine fbo
            PreviousPassFBO.Bind(EFramebufferTarget.DrawFramebuffer);
            {
                //Start with blank slate so additive blending doesn't ghost old frames
                Engine.Renderer.Clear(EFBOTextureType.Color);

                if (scene is Scene3D s3d)
                {
                    foreach (PointLightComponent c in s3d.Lights.PointLights)
                        viewport.RenderPointLight(c);

                    foreach (SpotLightComponent c in s3d.Lights.SpotLights)
                        viewport.RenderSpotLight(c);

                    foreach (DirectionalLightComponent c in s3d.Lights.DirectionalLights)
                        viewport.RenderDirLight(c);
                }
            }
            PreviousPassFBO.Unbind(EFramebufferTarget.DrawFramebuffer);
        }
    }
}
