using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TheraEngine.Rendering
{
    public delegate void DelSetUniforms(int programBindingId);
    /// <summary>
    /// Represents a framebuffer, material, quad (actually a giant triangle), and camera to render with.
    /// </summary>
    internal class QuadFrameBuffer : MaterialFrameBuffer
    {
        public event DelSetUniforms SettingUniforms;

        /// <summary>
        /// 2D camera for capturing the screen rendered to the framebuffer.
        /// </summary>
        OrthographicCamera _quadCamera;

        //One giant triangle is better than a quad with two triangles. 
        //Using two triangles may introduce tearing on the line through the screen,
        //because the two triangles may not be rasterized at the exact same time.
        PrimitiveManager _fullScreenTriangle;
        
        public QuadFrameBuffer(Material mat)
        {
            Material = mat;

            VertexTriangle triangle = new VertexTriangle(
                new Vec3(0.0f, 0.0f, 0.0f),
                new Vec3(2.0f, 0.0f, 0.0f),
                new Vec3(0.0f, 2.0f, 0.0f));
            
            _fullScreenTriangle = new PrimitiveManager(PrimitiveData.FromTriangles(Culling.None, VertexShaderDesc.JustPositions(), triangle), Material);
            _fullScreenTriangle.SettingUniforms += SetUniforms;

            _quadCamera = new OrthographicCamera() { NearZ = -0.5f, FarZ = 0.5f };
            _quadCamera.SetGraphStyle();
            _quadCamera.Resize(1.0f, 1.0f);
        }

        private void SetUniforms()
        {
            int fragId = Engine.Settings.AllowShaderPipelines ?
               _fullScreenTriangle.Material.Program.BindingId :
               _fullScreenTriangle.VertexFragProgram.BindingId;

            SettingUniforms?.Invoke(fragId);
        }

        public void Render()
        {
            AbstractRenderer.PushCurrentCamera(_quadCamera);
            _fullScreenTriangle.Render(Matrix4.Identity, Matrix3.Identity);
            AbstractRenderer.PopCurrentCamera();
        }
    }
}
