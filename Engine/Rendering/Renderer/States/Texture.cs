using CustomEngine.Rendering.Models.Materials;
using FreeImageAPI;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace CustomEngine.Rendering.Textures
{
    public class Texture : BaseRenderState
    {
        protected override int CreateObject()
        {
            return Engine.Renderer.CreateTextures(3553, 1)[0];
        }

        public Texture() : base(GenType.Texture) { }
        public Texture(int bindingId) : base(GenType.Texture, bindingId) { }
        public Texture(TextureData data) : base(GenType.Texture) { _data = data; }
        public Texture(TextureData data, MinFilter minFilter, MagFilter magFilter, TexCoordWrap uWrap, TexCoordWrap vWrap, float lodBias) : this(data)
        {
            _minFilter = minFilter;
            _magFilter = magFilter;
            _uWrapMode = uWrap;
            _vWrapMode = vWrap;
            _lodBias = lodBias;
        }

        public static readonly TextureMagFilter[] _magFilters = 
        {
            TextureMagFilter.Nearest,
            TextureMagFilter.Linear
        };
        public static readonly TextureMinFilter[] _minFilters = 
        {
            TextureMinFilter.Nearest,
            TextureMinFilter.Linear,
            TextureMinFilter.NearestMipmapNearest,
            TextureMinFilter.NearestMipmapLinear,
            TextureMinFilter.LinearMipmapNearest,
            TextureMinFilter.LinearMipmapLinear,
        };
        public static readonly TextureWrapMode[] _wraps =
        {
            TextureWrapMode.ClampToEdge,
            TextureWrapMode.Repeat,
            TextureWrapMode.MirroredRepeat
        };

        private TexCoordWrap _uWrapMode;
        private TexCoordWrap _vWrapMode;
        private MinFilter _minFilter;
        private MagFilter _magFilter;
        private float _lodBias;
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
            return Engine.Renderer.CreateObjects<Texture>(GenType.Texture, count);
        }

        public void Bind()
        {
            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, BindingId);
        }

        public void AttachFrameBuffer(FramebufferType type, DrawBuffersAttachment attachment, TextureTarget target = TextureTarget.Texture2D, int mipLevel = 0)
        {

        }

        protected override void OnGenerated()
        {
            if (_data == null)
                return;

            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, BindingId);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 0);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, TextureParameterName.TextureMinLod, 0);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, TextureParameterName.TextureMaxLod, 0);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, TextureParameterName.TextureLodBias, _lodBias);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)_magFilters[(int)_magFilter]);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)_minFilters[(int)_minFilter]);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)_wraps[(int)_uWrapMode]);
            GL.TexParameter(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)_wraps[(int)_vWrapMode]);

            Bitmap bmp = _data.Bitmap;
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

        protected override void PostDeleted()
        {
            base.PostDeleted();
        }
    }
}
