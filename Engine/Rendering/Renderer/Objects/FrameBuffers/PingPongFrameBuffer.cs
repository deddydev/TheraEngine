﻿using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class PingPongFrameBuffer
    {
        private MaterialFrameBuffer[] _fbos = new MaterialFrameBuffer[2];
        private TMaterial[] _materials = new TMaterial[3];
        private bool _hasInitial;

        public bool Ping { get; private set; } = true;
        public int Iteration { get; private set; }
        
        /// <summary>
        /// Use to set uniforms to the program containing the fragment shader.
        /// </summary>
        public event DelSetUniforms SettingUniforms;

        /// <summary>
        /// 2D camera for capturing the screen rendered to the framebuffer.
        /// </summary>
        private OrthographicCamera _quadCamera;

        //One giant triangle is better than a quad with two triangles. 
        //Using two triangles may introduce tearing on the line through the screen,
        //because the two triangles may not be rasterized at the exact same time.
        private MeshRenderer _fullScreenTriangle;
        
        public PingPongFrameBuffer(TMaterial pingMat, TMaterial pongMat, TMaterial initialPassMaterial)
        {
            _materials[0] = pingMat;
            _materials[1] = pongMat;
            _materials[2] = initialPassMaterial;

            _hasInitial = initialPassMaterial != null;
            TMaterial first = initialPassMaterial ?? pingMat;
            _fbos[0] = new MaterialFrameBuffer(first);
            _fbos[1] = new MaterialFrameBuffer(pongMat);

            TVertexTriangle triangle = new TVertexTriangle(
                new Vec3(0.0f, 0.0f, 0.0f),
                new Vec3(2.0f, 0.0f, 0.0f),
                new Vec3(0.0f, 2.0f, 0.0f));

            _fullScreenTriangle = new MeshRenderer(TMesh.Create(VertexShaderDesc.JustPositions(), triangle), first);
            _fullScreenTriangle.SettingUniforms += SetUniforms;

            _quadCamera = new OrthographicCamera(Vec3.One, Vec3.Zero, Rotator.GetZero(), Vec2.Zero, -0.5f, 0.5f);
            _quadCamera.Resize(1.0f, 1.0f);
        }
        private void SetUniforms(RenderProgram vertexProgram, RenderProgram materialProgram)
        {
            materialProgram.Uniform("Ping", Ping ? 0.0f : 1.0f);
            materialProgram.Uniform("Iteration", Iteration);
            SettingUniforms?.Invoke(materialProgram);
        }
        public void BindCurrentTarget(EFramebufferTarget target)
        {
            _fbos[Ping ? 0 : 1].Bind(target);
        }
        public void Bind(EFramebufferTarget target, bool ping)
        {
            _fbos[ping ? 0 : 1].Bind(target);
        }
        public void Reset()
        {
            _hasInitial = _materials[2] != null;
            _fbos[0].Material = _fullScreenTriangle.Material = _materials[2] ?? _materials[0];
            Ping = true;
            Iteration = 0;
        }
        public void RenderFullscreenAndSwitch()
        {
            RenderFullscreen();
            Switch();
        }
        public void RenderFullscreen()
        {
            Engine.Renderer.PushCamera(_quadCamera);
            _fullScreenTriangle.Render(Matrix4.Identity, Matrix3.Identity);
            Engine.Renderer.PopCamera();
        }
        public void Switch()
        {
            ++Iteration;
            Ping = !Ping;
            int i = Ping ? 0 : 1;
            _fullScreenTriangle.Material = _fbos[i].Material;
            if (_hasInitial)
            {
                _fbos[0].Material = _materials[0];
                _hasInitial = false;
            }
        }

        public void ResizeTextures(int w, int h)
        {
            _fbos[0].Resize(w, h);
            _fbos[1].Resize(w, h);
        }

        public void Destroy()
        {
            _fbos[0].Destroy();
            _fbos[1].Destroy();
        }
    }
}
