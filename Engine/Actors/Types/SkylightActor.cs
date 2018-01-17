using System;
using System.Drawing;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials.Textures;
using TheraEngine.Worlds.Actors.Components.Scene;
using TheraEngine.Worlds.Actors.Components.Scene.Transforms;
using TheraEngine.Rendering;

namespace TheraEngine.Worlds.Actors.Types
{
    public class SkylightActor : Actor<TranslationComponent>
    {
        RenderTexCube _skyTexture;
        
        public SkylightActor()
        {
            
        }
        
        public void Capture() => Capture(OwningWorld.Settings.Bounds.HalfExtents.LengthFast);
        public void Capture(float distance)
        {
           // Viewport v = new Viewport(_skyTexture.Width, _skyTexture.Height); 
            foreach (CameraComponent comp in RootComponent.ChildComponents)
            {
                PerspectiveCamera cam = (PerspectiveCamera)comp.Camera;

                //prefilterShader.use();
                //prefilterShader.setInt("environmentMap", 0);
                //prefilterShader.setMat4("projection", captureProjection);
                //glActiveTexture(GL_TEXTURE0);
                //glBindTexture(GL_TEXTURE_CUBE_MAP, envCubemap);

                //glBindFramebuffer(GL_FRAMEBUFFER, captureFBO);
                //unsigned int maxMipLevels = 5;
                //for (unsigned int mip = 0; mip < maxMipLevels; ++mip)
                //{
                //    // reisze framebuffer according to mip-level size.
                //    unsigned int mipWidth = 128 * std::pow(0.5, mip);
                //    unsigned int mipHeight = 128 * std::pow(0.5, mip);
                //    glBindRenderbuffer(GL_RENDERBUFFER, captureRBO);
                //    glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT24, mipWidth, mipHeight);
                //    glViewport(0, 0, mipWidth, mipHeight);

                //    float roughness = (float)mip / (float)(maxMipLevels - 1);
                //    prefilterShader.setFloat("roughness", roughness);
                //    for (unsigned int i = 0; i < 6; ++i)
                //    {
                //        prefilterShader.setMat4("view", captureViews[i]);
                //        glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, prefilterMap, mip);

                //        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
                //        renderCube();
                //    }
                //}
                //glBindFramebuffer(GL_FRAMEBUFFER, 0);

                //TODO: render to each of the sky texture's sides
                //Engine.Scene.Render(cam, cam.Frustum, v, false);
            }
        }

        protected override TranslationComponent OnConstruct()
        {
            Rotator[] rotations = new Rotator[]
            {
                new Rotator(0.0f,   0.0f, 0.0f, RotationOrder.YPR), //forward
                new Rotator(0.0f, 180.0f, 0.0f, RotationOrder.YPR), //backward
                new Rotator(0.0f, -90.0f, 0.0f, RotationOrder.YPR), //left
                new Rotator(0.0f,  90.0f, 0.0f, RotationOrder.YPR), //right
                new Rotator(90.0f,  0.0f, 0.0f, RotationOrder.YPR), //up
                new Rotator(-90.0f, 0.0f, 0.0f, RotationOrder.YPR), //down
            };
            TranslationComponent pos = new TranslationComponent();
            for (int i = 0; i < 6; ++i)
            {
                CameraComponent cam = new CameraComponent(new PerspectiveCamera(
                    Vec3.Zero, rotations[i], 1.0f, 10000.0f, 90.0f, 1.0f));
                pos.ChildComponents.Add(cam);
            }
            return pos;
        }
    }
}
