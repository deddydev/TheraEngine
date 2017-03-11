using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering
{
    public class GBuffer : FrameBuffer
    {
        int _bufferCount;
        GBufferTextureType _types;
        Texture[] _textures;
        Texture _depthTexture;
        PrimitiveManager _fullScreenQuad;

        public GBuffer(GBufferTextureType types)
        {
            _types = types;
            _bufferCount = ((byte)types).CountBits();
            _textures = new Texture[_bufferCount];
            _fullScreenQuad = new PrimitiveManager(PrimitiveData.FromQuads(Culling.Back, new PrimitiveBufferInfo(), 
                VertexQuad.MakeQuad(Vec3.Zero, new Vec3(1.0f, 0.0f, 0.0f), new Vec3(1.0f, 1.0f, 0.0f), new Vec3(0.0f, 1.0f, 0.0f))),
                Material.GetGBufferMaterial());
        }

        protected override void OnGenerated()
        {
            Bind(FramebufferType.Write);

            Viewport v = Viewport.CurrentlyRendering;

            _textures = Texture.GenTextures(_bufferCount);
            (_depthTexture = new Texture()).Generate();
            for (int i = 0; i < _bufferCount; ++i)
            {
                Texture t = _textures[i];
                //t.SetData(TextureData.EmptyFrameBuffer(v.Width, v.Height));
                t.Bind();
                //t.PushData();
                t.AttachFrameBuffer(FramebufferType.ReadWrite, (DrawBuffersAttachment)i, TextureTarget.Texture2D, 0);
            }
            //_depthTexture.SetData(TextureData.EmptyDepthFrameBuffer(v.Width, v.Height));
            _depthTexture.Bind();
            //_depthTexture.PushData();
            _depthTexture.AttachFrameBuffer(FramebufferType.ReadWrite, DrawBuffersAttachment.DepthAttachement, TextureTarget.Texture2D, 0);

            Unbind(FramebufferType.Write);
        }
        public void BindForWriting()
        {

        }
        public void BindForReading()
        {

        }
        public unsafe void Resize(float width, float height)
        {
            // 3--2
            // |\ |
            // | \|
            // 0--1
            //0 1 3 3 1 2

            //Vec3 bottomLeft = new Vec3(0.0f, 0.0f, 0.0f);
            Vec3 bottomRight = new Vec3(width, 0.0f, 0.0f);
            Vec3 topRight = new Vec3(width, height, 0.0f);
            Vec3 topLeft = new Vec3(0.0f, height, 0.0f);

            Vec3* data = (Vec3*)_fullScreenQuad.Data[0].Address;
            //data[0] = bottomLeft;
            data[1] = data[4] = bottomRight;
            data[2] = data[3] = topLeft;
            data[5] = topRight;
        }

        public void Render()
        {
            _fullScreenQuad.Render(Matrix4.Identity, Matrix3.Identity);
        }
    }

    [Flags]
    public enum GBufferTextureType
    {
        Position    = 0x01,
        Normal      = 0x02,
        Diffuse     = 0x04,
        TexCoord    = 0x08,
        Stencil     = 0x10,
    }
}
