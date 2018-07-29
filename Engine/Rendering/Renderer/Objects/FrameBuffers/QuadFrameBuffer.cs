﻿using System;
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
        private OrthographicCamera _quadCamera;

        public PrimitiveManager FullScreenTriangle { get; }

        public static PrimitiveData Mesh()
        {
            VertexTriangle triangle = new VertexTriangle(
                new Vec3(0.0f, 0.0f, 0.0f),
                new Vec3(2.0f, 0.0f, 0.0f),
                new Vec3(0.0f, 2.0f, 0.0f));
            return PrimitiveData.FromTriangles(VertexShaderDesc.JustPositions(), triangle);
        }

        /// <summary>
        /// Renders a material to the screen using a fullscreen orthographic quad.
        /// </summary>
        /// <param name="mat">The material containing textures to render to this fullscreen quad.</param>
        public QuadFrameBuffer(TMaterial mat)
        {
            Material = mat;

            FullScreenTriangle = new PrimitiveManager(Mesh(), Material);
            FullScreenTriangle.SettingUniforms += SetUniforms;

            _quadCamera = new OrthographicCamera(Vec3.One, Vec3.Zero, Rotator.GetZero(), Vec2.Zero, -0.5f, 0.5f);
            _quadCamera.Resize(1.0f, 1.0f);
        }
        private void SetUniforms(RenderProgram vertexProgram, RenderProgram materialProgram)
        {
            SettingUniforms?.Invoke(materialProgram);
        }
        /// <summary>
        /// Renders the FBO to the entire region set by Engine.Renderer.PushRenderArea().
        /// </summary>
        public void RenderFullscreen()
        {
            Engine.Renderer.PushCamera(_quadCamera);
            FullScreenTriangle.Render();
            Engine.Renderer.PopCamera();
        }
    }
}
