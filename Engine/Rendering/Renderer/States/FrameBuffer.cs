using System;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering
{
    public delegate void DelSetUniforms(int programBindingId);
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
    public class MaterialFrameBuffer : FrameBuffer
    {
        public MaterialFrameBuffer() { }
        public MaterialFrameBuffer(TMaterial m) { Material = m; }

        private bool _compiled = false;

        private TMaterial _material;
        public TMaterial Material
        {
            get => _material;
            set
            {
                if (_material == value)
                    return;
                if (_material != null && _material.FrameBuffer == this)
                    _material.FrameBuffer = null;
                _material = value;
                if (_material != null)
                {
                    _material.FrameBuffer = this;
                    _compiled = false;
                }
            }
        }
        public BaseTextureReference[] Textures => Material?.Textures;
        public void ResizeTextures(int width, int height) => Material?.Resize2DTextures(width, height);
        public void Compile()
        {
            Compile(Material.FBOAttachments);
        }
        public void Compile(EDrawBuffersAttachment[] attachments)
        {
            if (Material == null)
                return;
            if (BaseRenderPanel.NeedsInvoke(Compile, BaseRenderPanel.PanelType.Game))
                return;
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, BindingId);
            Material.GenerateTextures(true);
            Engine.Renderer.SetDrawBuffers(attachments);
            //CheckErrors();
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, 0);
            _compiled = true;
        }
        public override void Bind(EFramebufferTarget type)
        {
            if (!_compiled)
                Compile();
            base.Bind(type);
        }
    }
    public class FrameBuffer : BaseRenderState
    {
        public FrameBuffer() : base(EObjectType.Framebuffer) { }
        public virtual void Bind(EFramebufferTarget type)
            => Engine.Renderer.BindFrameBuffer(type, BindingId);
        public virtual void Unbind(EFramebufferTarget type)
            => Engine.Renderer.BindFrameBuffer(type, 0);
        public void CheckErrors()
            => Engine.Renderer.CheckFrameBufferErrors();
    }
}
