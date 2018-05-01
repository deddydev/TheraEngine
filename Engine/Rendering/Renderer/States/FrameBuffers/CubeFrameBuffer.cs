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
        
        public CubeFrameBuffer(TMaterial mat, bool perspectiveCameras = true)
        {
            Material = mat;

            PrimitiveData cubeData = BoundingBox.SolidMesh(-1.0f, 1.0f, true);
            _cube = new PrimitiveManager(cubeData, Material);
            _cube.SettingUniforms += SetUniforms;

            _cameras = new Camera[6];
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
                _cameras[i] = perspectiveCameras ?
                    new PerspectiveCamera(Vec3.Zero, rotations[i], 1.0f, 10000.0f, 90.0f, 1.0f) :
                    (Camera)new OrthographicCamera(2.0f, 2.0f, Vec3.One, Vec3.Zero, rotations[i], Vec2.Half, 1.0f, 10000.0f);

        }
        private void SetUniforms()
        {
            int fragId = Engine.Settings.AllowShaderPipelines ?
               _cube.Material.Program.BindingId :
               _cube.VertexFragProgram.BindingId;

            SettingUniforms?.Invoke(fragId);
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
