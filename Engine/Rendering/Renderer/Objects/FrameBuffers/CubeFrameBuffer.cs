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

        private MeshRenderer _cube;

        public TypicalCamera[] Cameras { get; }
        public Vec3 Position { get; private set; }

        public CubeFrameBuffer(TMaterial mat, float nearZ = 1.0f, float farZ = 1000.0f, bool perspectiveCameras = true)
        {
            Material = mat;

            float middle = (nearZ + farZ) * 0.5f;
            Mesh cubeData = BoundingBox.SolidMesh(-middle, middle, true);
            _cube = new MeshRenderer(cubeData, Material);
            _cube.SettingUniforms += SetUniforms;

            Cameras = new TypicalCamera[6];
            Rotator[] rotations = new Rotator[]
            {
                new Rotator(  0.0f,  90.0f,   0.0f), //+X
                new Rotator(  0.0f, -90.0f,   0.0f), //-X
                new Rotator(-90.0f,   0.0f, 180.0f), //+Y
                new Rotator( 90.0f,   0.0f, 180.0f), //-Y
                new Rotator(  0.0f, 180.0f,   0.0f), //+Z
                new Rotator(  0.0f,   0.0f,   0.0f), //-Z
            };

            TypicalCamera cam;
            float range = farZ - nearZ;
            for (int i = 0; i < 6; ++i)
            {
                cam = perspectiveCameras ?
                    new PerspectiveCamera(Vec3.Zero, rotations[i], nearZ, farZ, 90.0f, 1.0f) :
                    (TypicalCamera)new OrthographicCamera(2.0f, 2.0f, Vec3.One, Vec3.Zero, rotations[i], Vec2.Half, nearZ, farZ);

                cam.Resize(range, range);

                Cameras[i] = cam;
            }
        }

        private void SetUniforms(RenderProgram vertexProgram, RenderProgram materialProgram)
            => SettingUniforms?.Invoke(materialProgram);
        
        /// <summary>
        /// Renders the one side of the FBO to the entire region set by Engine.Renderer.PushRenderArea().
        /// </summary>
        public void RenderFullscreen(ECubemapFace face)
        {
            Engine.Renderer.PushCamera(Cameras[(int)face]);
            _cube.Render();
            Engine.Renderer.PopCamera();
        }

        public void SetTransform(Vec3 point)
        {
            Position = point;
            foreach (TypicalCamera c in Cameras)
                c.LocalPoint.Raw = Position;
        }
    }
}
