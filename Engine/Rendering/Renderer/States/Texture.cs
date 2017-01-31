using FreeImageAPI;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace CustomEngine.Rendering.Textures
{
    public enum TextureFormat
    {

    }
    public class Texture : BaseRenderState
    {
        public Texture() : base(GenType.Texture) { }
        public Texture(int bindingId) : base(GenType.Texture, bindingId) { }
        public Texture(TextureData data) : base(GenType.Texture) { _data = data; }

        private TextureData _data;
        public TextureData Data
        {
            get { return _data; }
            set
            {
                Delete();
                _data = value;
            }
        }

        public static Texture[] GenTextures(int count)
        {
            return Engine.Renderer.GenObjects<Texture>(GenType.Texture, count);
        }

        public void Bind()
        {
            if (!IsActive)
                Generate();
            
            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, BindingId);
        }

        public void PushData()
        {
            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, BindingId);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 0);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, TextureParameterName.TextureMinLod, 0);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, TextureParameterName.TextureMaxLod, 0);

            FreeImageBitmap bmp = _data.Bitmap;
            if (bmp != null)
            {
                BitmapData data = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height), 
                    ImageLockMode.ReadOnly, 
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(
                    OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, 
                    0, 
                    OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba8, 
                    data.Width, 
                    data.Height, 
                    0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, 
                    OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, 
                    data.Scan0);

                bmp.UnlockBits(data);
            }
        }

        public void AttachFrameBuffer(FramebufferType type, DrawBuffersAttachment attachment, TextureTarget target = TextureTarget.Texture2D, int mipLevel = 0)
        {

        }

        protected override void OnGenerated()
        {
            PushData();
        }

        protected override void OnDeleted()
        {
            base.OnDeleted();
        }
    }
}
