using System;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Actors.Types
{
    public class SceneCaptureComponent : TranslationComponent
    {
        protected PerspectiveCamera[] _cameras;
        protected FrameBuffer _renderFBO;
        protected int _resolution;
        protected Viewport _viewport;
        protected TexRefCube _cubeTex;

        public TexRefCube ResultTexture => _cubeTex;

        public SceneCaptureComponent() { }
        
        public void SetCubeResolution(int resolution)
        {
            _resolution = resolution;
            if (_renderFBO == null)
                Initialize(resolution);
            else
                _renderFBO.ResizeTextures(resolution, resolution);
        }
        protected virtual void Initialize(int resolution)
        {
            _viewport = new Viewport(resolution, resolution);

            _cameras = new PerspectiveCamera[6];
            Rotator[] rotations = new Rotator[]
            {
                new Rotator(0.0f,  90.0f, 0.0f, RotationOrder.YPR), //+X right
                new Rotator(0.0f, -90.0f, 0.0f, RotationOrder.YPR), //-X left
                new Rotator(90.0f,  0.0f, 0.0f, RotationOrder.YPR), //+Y up
                new Rotator(-90.0f, 0.0f, 0.0f, RotationOrder.YPR), //-Y down
                new Rotator(0.0f, 180.0f, 0.0f, RotationOrder.YPR), //+Z backward
                new Rotator(0.0f,   0.0f, 0.0f, RotationOrder.YPR), //-Z forward
            };

            for (int i = 0; i < 6; ++i)
                _cameras[i] = new PerspectiveCamera(
                    Vec3.Zero, rotations[i], 1.0f, 10000.0f, 90.0f, 1.0f);

            _renderFBO = GetFBO(resolution);
        }
        protected virtual FrameBuffer GetFBO(int cubeExtent)
        {
            _cubeTex = new TexRefCube("SceneCaptureCubeMap", cubeExtent,
                EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat)
            {
                MinFilter = ETexMinFilter.LinearMipmapLinear,
                MagFilter = ETexMagFilter.Linear,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
                WWrap = ETexWrapMode.ClampToEdge,
            };
            
            FrameBuffer f = new FrameBuffer();
            f.SetRenderTargets((_cubeTex, EFramebufferAttachment.ColorAttachment0, 0));
            return f;
        }
        
        /// <summary>
        /// Renders the scene to the ResultTexture cubemap.
        /// </summary>
        public void Capture()
        {
            if (_renderFBO == null)
                SetCubeResolution(512);

            BaseScene scene = OwningScene;
            for (int i = 0; i < 6; ++i)
            {
                Camera camera = _cameras[i];
                camera.LocalPoint.Raw = WorldPoint;

                _viewport.Update(scene, camera, camera.Frustum);
                _viewport.SwapBuffers();

                _cubeTex.AttachFaceToFBO(_renderFBO.BindingId, ECubemapFace.PosX + i);
                _viewport.Render(scene, camera, _renderFBO);
            }

            var tex = _cubeTex.GetTexture(true);
            tex.Bind();
            tex.SetMipmapGenParams();
            tex.GenerateMipmaps();
        }
    }
}
