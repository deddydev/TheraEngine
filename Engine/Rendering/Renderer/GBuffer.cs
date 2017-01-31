using CustomEngine.Rendering.Models;
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
                t.PushData();
                t.AttachFrameBuffer(FramebufferType.ReadWrite, (DrawBuffersAttachment)i, TextureTarget.Texture2D, 0);
            }
            //_depthTexture.SetData(TextureData.EmptyDepthFrameBuffer(v.Width, v.Height));
            _depthTexture.Bind();
            _depthTexture.PushData();
            _depthTexture.AttachFrameBuffer(FramebufferType.ReadWrite, DrawBuffersAttachment.DepthAttachement, TextureTarget.Texture2D, 0);

            Unbind(FramebufferType.Write);
        }
        public void BindForWriting()
        {

        }
        public void BindForReading()
        {

        }
        public void Resize(float width, float height)
        {

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
