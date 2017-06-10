using CustomEngine.Rendering.Cameras;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Rendering.Textures;
using OpenTK.Graphics.OpenGL;
using System;
using System.Linq;

namespace CustomEngine.Rendering
{
    public class GBufferMeshProgram : MeshProgram
    {
        DrawBuffersAttachment[] _attachments;
        EFramebufferAttachment?[] _attachmentsPerTexture;
        GBuffer _buffer;
        //int _renderBufferId;
        int _width, _height;
        
        public GBufferMeshProgram(Material material, PrimitiveBufferInfo info) : base(material, info) { }

        public void Update(GBuffer buffer, EFramebufferAttachment?[] attachmentsPerTexture, DrawBuffersAttachment[] attachments, int width, int height)
        {
            _width = width;
            _height = height;
            _buffer = buffer;
            _attachments = attachments;
            _attachmentsPerTexture = attachmentsPerTexture;
            
            _buffer.Bind(EFramebufferTarget.Framebuffer);
            for (int i = 0; i < Textures.Length; ++i)
            {
                Texture t = _textures[i];
                t.Index = i;
                t.PostPushData += TexturePostPushData;
                t.Generate();
            }
            //_renderBufferId = Engine.Renderer.CreateObjects(GenType.Renderbuffer, 1)[0];
            //Engine.Renderer.BindRenderBuffer(_renderBufferId);
            //Engine.Renderer.RenderbufferStorage(ERenderBufferStorage.DepthComponent, _width, _height);
            //Engine.Renderer.FramebufferRenderBuffer(BindingId, EFramebufferAttachment.DepthAttachment, _renderBufferId);
            FramebufferErrorCode c = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (c != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("Problem compiling G-Buffer.");
            _buffer.Unbind(EFramebufferTarget.Framebuffer);
        }

        public override void SetMaterial(Material material)
        {
            _parameters = material.Parameters.ToArray();

            _textures = new Texture[material.Textures.Count];
            for (int i = 0; i < material.Textures.Count; ++i)
                _textures[i] = material.Textures[i].GetTexture();

            _fragmentShader = material._fragmentShader;
            _geometryShader = material._geometryShader;
            _tControlShader = material._tessellationControlShader;
            _tEvalShader = material._tessellationEvaluationShader;
            RemakeShaderArray();
        }
        private void TexturePostPushData(int index)
        {
            if (_attachmentsPerTexture[index].HasValue)
                Engine.Renderer.AttachTextureToFrameBuffer(EFramebufferTarget.Framebuffer, _attachmentsPerTexture[index].Value, ETexTarget.Texture2D, Textures[index].BindingId, 0);
        }
        protected internal override void BindTextures()
        {
            _buffer.Bind(EFramebufferTarget.Framebuffer);
            base.BindTextures();
            Engine.Renderer.SetDrawBuffers(_attachments);
            _buffer.Unbind(EFramebufferTarget.Framebuffer);
        }
        public void Resized(int width, int height)
        {
            foreach (Texture t in Textures)
                t.Resize(width, height);
        }
    }
    public class GBuffer : FrameBuffer
    {
        OrthographicCamera _camera;
        PrimitiveManager<GBufferMeshProgram> _fullScreenTriangle;
        bool _forward;
        
        public Texture[] Textures => _fullScreenTriangle?.Program.Textures;

        public Texture AlbedoSpec => _fullScreenTriangle?.Program.Textures[0];
        public Texture Positions => _fullScreenTriangle?.Program.Textures[1];
        public Texture Normals => _fullScreenTriangle?.Program.Textures[2];
        //public Texture Text => _fullScreenTriangle?.Program.Textures[3];
        public Texture Depth => _fullScreenTriangle?.Program.Textures[3];

        EFramebufferAttachment?[] _attachmentsPerTexture = new EFramebufferAttachment?[]
        {
            EFramebufferAttachment.ColorAttachment0, //AlbedoSpec
            EFramebufferAttachment.ColorAttachment1, //Position
            EFramebufferAttachment.ColorAttachment2, //Normal
            //null,
            EFramebufferAttachment.DepthAttachment, //Depth
        };
        DrawBuffersAttachment[] _attachments = new DrawBuffersAttachment[]
        {
            DrawBuffersAttachment.ColorAttachment0, //AlbedoSpec
            DrawBuffersAttachment.ColorAttachment1, //Position
            DrawBuffersAttachment.ColorAttachment2, //Normal
        };

        public GBuffer(BoundingRectangle region, bool forward)
        {
            _forward = forward;
            Vertex point0 = new Vec3(0.0f, 0.0f, -2.0f);
            Vertex point1 = new Vec3(2.0f, 0.0f, -2.0f);
            Vertex point2 = new Vec3(0.0f, 2.0f, -2.0f);
            //Vertex point3 = new Vec3(0.0f, 1.0f, -2.0f);
            //Vertex point4 = new Vec3(1.0f, 0.0f, -2.0f);
            //Vertex point5 = new Vec3(1.0f, 1.0f, -2.0f);

            VertexTriangle triangle1 = new VertexTriangle(point0, point1, point2);
            //VertexTriangle triangle2 = new VertexTriangle(point3, point4, point5);

            _fullScreenTriangle = new PrimitiveManager<GBufferMeshProgram>(
                PrimitiveData.FromTriangles(Culling.None, PrimitiveBufferInfo.JustPositions(), triangle1/*, triangle2*/),
                //PrimitiveData.FromQuads(Culling.Back, new PrimitiveBufferInfo(), VertexQuad.ZUpQuad(region)),
                Material.GetGBufferMaterial(region.IntWidth, region.IntHeight, forward, this));
            _fullScreenTriangle.Program.Update(this, _attachmentsPerTexture, _attachments, region.IntWidth, region.IntHeight);
            _fullScreenTriangle.SettingUniforms += Engine.Renderer.Scene.SetUniforms;

            _camera = new OrthographicCamera();
            _camera.SetGraphStyle();
            _camera.Resize(1.0f, 1.0f);
            //SetRegion(region);
        }
        ~GBuffer()
        {
            _fullScreenTriangle.SettingUniforms -= Engine.Renderer.Scene.SetUniforms;
        }
        public unsafe void Resize(int width, int height)
        {
            _fullScreenTriangle.Program.Resized(width, height);

            //VertexBuffer buffer = _fullScreenTriangle.Data[0];
            //Vec3* data = (Vec3*)buffer.Address;

            //data[0] = new Vec3(0.0f, 0.0f, -2.0f);
            //data[1] = new Vec3(region.Width, 0.0f, -2.0f);
            //data[2] = new Vec3(0.0f, region.Height, -2.0f);

            //Old method: full screen quad.
            //Not using this method because of the possible tear line between the two triangles without vsync
            //while waiting for BOTH triangles to finish rasterizing the given frame before displaying.
            // 3--2
            // |\ |
            // | \|
            // 0--1
            //0 1 3 3 1 2
            //remapped ->
            //0 1 3 2

            //data[0] = new Vec3(region.BottomLeft, 0.0f);
            //data[1] = new Vec3(region.BottomRight, 0.0f);
            //data[2] = new Vec3(region.TopLeft, 0.0f);
            //data[3] = new Vec3(region.TopRight, 0.0f);

            //_camera.Resize(region.Width, region.Height);
        }

        public void Render()
        {
            AbstractRenderer.CurrentCamera = _camera;
            _fullScreenTriangle.Render(Matrix4.Identity, Matrix3.Identity);
            AbstractRenderer.CurrentCamera = null;
        }
    }
}
