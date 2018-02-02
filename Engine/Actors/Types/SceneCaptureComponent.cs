using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials.Textures;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Transforms;

namespace TheraEngine.Actors.Types
{
    public class SceneCaptureComponent : TranslationComponent
    {
        RenderTexCube _skyTexture;
        private PerspectiveCamera[] _cameras;

        public SceneCaptureComponent()
        {
            _cameras = new PerspectiveCamera[6];
            Rotator[] rotations = new Rotator[]
            {
               new Rotator(0.0f,   0.0f, 0.0f, RotationOrder.YPR), //forward
               new Rotator(0.0f, 180.0f, 0.0f, RotationOrder.YPR), //backward
               new Rotator(0.0f, -90.0f, 0.0f, RotationOrder.YPR), //left
               new Rotator(0.0f,  90.0f, 0.0f, RotationOrder.YPR), //right
               new Rotator(90.0f,  0.0f, 0.0f, RotationOrder.YPR), //up
               new Rotator(-90.0f, 0.0f, 0.0f, RotationOrder.YPR), //down
            };
            for (int i = 0; i < 6; ++i)
                _cameras[i] = new PerspectiveCamera(
                    Vec3.Zero, rotations[i], 1.0f, 10000.0f, 90.0f, 1.0f);
        }
        
        public void Capture() => Capture(OwningWorld.Settings.Bounds.HalfExtents.LengthFast);
        public void Capture(float distance)
        {
           // Viewport v = new Viewport(_skyTexture.Width, _skyTexture.Height); 
            foreach (PerspectiveCamera cam in _cameras)
            {
                //prefilterShader.use();
                //prefilterShader.setInt("environmentMap", 0);
                //prefilterShader.setMat4("projection", captureProjection);
                //glActiveTexture(GL_TEXTURE0);
                //glBindTexture(GL_TEXTURE_CUBE_MAP, envCubemap);

                //glBindFramebuffer(GL_FRAMEBUFFER, captureFBO);
                //unsigned int maxMipLevels = 5;
                //for (unsigned int mip = 0; mip < maxMipLevels; ++mip)
                //{
                //    // resize framebuffer according to mip-level size.
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
    }
}
