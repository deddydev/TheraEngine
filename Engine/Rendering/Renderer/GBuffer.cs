using CustomEngine.Rendering.Cameras;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering
{
    public class GBuffer : FrameBuffer
    {
        BoundingRectangle _region;
        OrthographicCamera _camera;
        PrimitiveManager _fullScreenQuad;
        int _renderBufferId;
        bool _forward;

        public Texture[] Textures => _fullScreenQuad?.Program.Textures;
        public Texture DepthStencil => _fullScreenQuad?.Program.Textures[0];
        public Texture AlbedoSpec => _fullScreenQuad?.Program.Textures[1];
        public Texture Positions => _fullScreenQuad?.Program.Textures[2];
        public Texture Normals => _fullScreenQuad?.Program.Textures[3];
        public Texture Text => _fullScreenQuad?.Program.Textures[4];
        
        DrawBuffersAttachment[] _attachments = new DrawBuffersAttachment[]
        {
            DrawBuffersAttachment.ColorAttachment0, //AlbedoSpec
            DrawBuffersAttachment.ColorAttachment1, //Position
            DrawBuffersAttachment.ColorAttachment2, //Normal
        };

        public GBuffer(BoundingRectangle region, bool forward)
        {
            _forward = forward;
            _fullScreenQuad = new PrimitiveManager(
                PrimitiveData.FromQuads(Culling.Back, new PrimitiveBufferInfo(), 
                VertexQuad.ZUpQuad(region)),
                Material.GetGBufferMaterial(region.IntWidth, region.IntHeight, forward));
            _fullScreenQuad.SettingUniforms += Engine.Renderer.Scene.SetUniforms;
            _camera = new OrthographicCamera();
            _camera.SetGraphStyle();
        }
        ~GBuffer()
        {
            _fullScreenQuad.SettingUniforms -= Engine.Renderer.Scene.SetUniforms;
        }

        protected override void OnGenerated()
        {
            _renderBufferId = Engine.Renderer.CreateObjects(GenType.Renderbuffer, 1)[0];
            Engine.Renderer.BindRenderBuffer(_renderBufferId);
            Engine.Renderer.RenderbufferStorage(ERenderBufferStorage.Depth24Stencil8, _region.IntWidth, _region.IntHeight);
            Engine.Renderer.FramebufferRenderBuffer(BindingId, EFramebufferAttachment.DepthStencilAttachment, _renderBufferId);
            
            DepthStencil.AttachToFrameBuffer(BindingId, EFramebufferAttachment.DepthStencilAttachment);
            AlbedoSpec.AttachToFrameBuffer(BindingId, EFramebufferAttachment.ColorAttachment0);
            //Text.AttachToFrameBuffer(BindingId, EFramebufferAttachment.ColorAttachment3);
            if (!_forward)
            {
                Positions.AttachToFrameBuffer(BindingId, EFramebufferAttachment.ColorAttachment1);
                Normals.AttachToFrameBuffer(BindingId, EFramebufferAttachment.ColorAttachment2);
            }

            Engine.Renderer.SetDrawBuffers(BindingId, _attachments);
        }
        protected override void PostDeleted()
        {
            Engine.Renderer.DeleteObject(GenType.Renderbuffer, _renderBufferId);
            base.PostDeleted();
        }
        public unsafe void SetRegion(BoundingRectangle region)
        {
            // 3--2
            // |\ |
            // | \|
            // 0--1
            //0 1 3 3 1 2
            //remapped ->
            //0 1 3 2

            VertexBuffer buffer = _fullScreenQuad.Data[0];
            Vec3* data = (Vec3*)buffer.Address;
            data[0] = new Vec3(region.BottomLeft, 0.0f);
            data[1] = new Vec3(region.BottomRight, 0.0f);
            data[2] = new Vec3(region.TopLeft, 0.0f);
            data[3] = new Vec3(region.TopRight, 0.0f);

            _camera.Resize(region.Width, region.Height);
        }

        public void Render()
        {
            AbstractRenderer.CurrentCamera = _camera;
            _fullScreenQuad.Render(Matrix4.Identity, Matrix3.Identity);
            AbstractRenderer.CurrentCamera = null;
        }
    }
}
