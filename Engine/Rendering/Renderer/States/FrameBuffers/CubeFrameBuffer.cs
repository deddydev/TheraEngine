using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Rendering
{
    public class CubeFrameBuffer : MaterialFrameBuffer
    {
        public event DelSetUniforms SettingUniforms;
        
        private Camera[] _cameras;
        private PrimitiveManager _cube;

        public Camera[] Cameras => _cameras;

        public CubeFrameBuffer(TMaterial mat, float nearZ = 1.0f, float farZ = 1000.0f, bool perspectiveCameras = true)
        {
            Material = mat;

            PrimitiveData cubeData = BoundingBox.SolidMesh(-1.0f, 1.0f, true);
            _cube = new PrimitiveManager(cubeData, Material);
            _cube.SettingUniforms += SetUniforms;

            _cameras = new Camera[6];
            Rotator[] rotations = new Rotator[]
            {
                new Rotator(0.0f, -90.0f, 180.0f), //+X
                new Rotator(0.0f,  90.0f, 180.0f), //-X
                new Rotator(90.0f,  0.0f, 0.0f), //+Y
                new Rotator(-90.0f, 0.0f, 0.0f), //-Y
                new Rotator(0.0f, 180.0f, 180.0f), //+Z
                new Rotator(0.0f,   0.0f, 180.0f), //-Z
            };

            Camera c;
            for (int i = 0; i < 6; ++i)
            {
                c = perspectiveCameras ?
                    new PerspectiveCamera(Vec3.Zero, rotations[i], nearZ, farZ, 90.0f, 1.0f) :
                    (Camera)new OrthographicCamera(2.0f, 2.0f, Vec3.One, Vec3.Zero, rotations[i], Vec2.Half, nearZ, farZ);
                c.Resize(2.0f, 2.0f);
                _cameras[i] = c;
            }

        }
        private void SetUniforms(RenderProgram vertexProgram, RenderProgram materialProgram)
        {
            SettingUniforms?.Invoke(materialProgram);
        }

        /// <summary>
        /// Renders the FBO to the entire region set by Engine.Renderer.PushRenderArea().
        /// </summary>
        public void RenderFullscreen(ECubemapFace face)
        {
            AbstractRenderer.PushCamera(_cameras[(int)face]);
            _cube.Render();
            AbstractRenderer.PopCamera();
        }
    }
}
