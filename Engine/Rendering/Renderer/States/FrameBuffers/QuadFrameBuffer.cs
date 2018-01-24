using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    /// <summary>
    /// Represents a framebuffer, material, quad (actually a giant triangle), and camera to render with.
    /// </summary>
    public class QuadFrameBuffer : MaterialFrameBuffer
    {
        /// <summary>
        /// Use to set uniforms to the program containing the fragment shader.
        /// </summary>
        public event DelSetUniforms SettingUniforms;

        /// <summary>
        /// 2D camera for capturing the screen rendered to the framebuffer.
        /// </summary>
        OrthographicCamera _quadCamera;

        //One giant triangle is better than a quad with two triangles. 
        //Using two triangles may introduce tearing on the line through the screen,
        //because the two triangles may not be rasterized at the exact same time.
        PrimitiveManager _fullScreenTriangle;

        public QuadFrameBuffer(TMaterial mat)
        {
            Material = mat;

            VertexTriangle triangle = new VertexTriangle(
                new Vec3(0.0f, 0.0f, 0.0f),
                new Vec3(2.0f, 0.0f, 0.0f),
                new Vec3(0.0f, 2.0f, 0.0f));

            _fullScreenTriangle = new PrimitiveManager(PrimitiveData.FromTriangles(Culling.None, VertexShaderDesc.JustPositions(), triangle), Material);
            _fullScreenTriangle.SettingUniforms += SetUniforms;

            _quadCamera = new OrthographicCamera(Vec3.One, Vec3.Zero, Rotator.GetZero(), Vec2.Zero, -0.5f, 0.5f);
            _quadCamera.Resize(1.0f, 1.0f);
        }

        private void SetUniforms()
        {
            int fragId = Engine.Settings.AllowShaderPipelines ?
               _fullScreenTriangle.Material.Program.BindingId :
               _fullScreenTriangle.VertexFragProgram.BindingId;

            SettingUniforms?.Invoke(fragId);
        }

        /// <summary>
        /// Renders the framebuffer.
        /// </summary>
        public void Render()
        {
            AbstractRenderer.PushCurrentCamera(_quadCamera);
            _fullScreenTriangle.Render(Matrix4.Identity, Matrix3.Identity);
            AbstractRenderer.PopCurrentCamera();
        }
    }
}
