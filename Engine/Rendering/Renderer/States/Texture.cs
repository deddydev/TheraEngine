using CustomEngine.Rendering.Models.Materials;
using FreeImageAPI;
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

        public static readonly ETexMagFilter[] _magFilters = 
        {
            ETexMagFilter.Nearest,
            ETexMagFilter.Linear
        };
        public static readonly ETexMinFilter[] _minFilters = 
        {
            ETexMinFilter.Nearest,
            ETexMinFilter.Linear,
            ETexMinFilter.NearestMipmapNearest,
            ETexMinFilter.NearestMipmapLinear,
            ETexMinFilter.LinearMipmapNearest,
            ETexMinFilter.LinearMipmapLinear,
        };
        public static readonly ETexWrapMode[] _wraps =
        {
            ETexWrapMode.ClampToEdge,
            ETexWrapMode.Repeat,
            ETexWrapMode.MirroredRepeat
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
            if (!IsActive)
                Generate();
            Engine.Renderer.BindTexture(ETexTarget.Texture2D, BindingId);
        }

        public void AttachToFrameBuffer(EFramebufferTarget target, EFramebufferAttachment attachment, ETexTarget texTarget)
            => Engine.Renderer.AttachTextureToFrameBuffer(target, attachment, texTarget, BindingId, 0);
        
        public void PushData()
        {
            if (_data == null)
                return;

            Bind();

            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureBaseLevel, 0);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureMaxLevel, 0);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureMinLod, 0);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureMaxLod, 0);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureLodBias, _lodBias);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureMagFilter, (int)_magFilters[(int)_magFilter]);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureMinFilter, (int)_minFilters[(int)_minFilter]);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureWrapS, (int)_wraps[(int)_uWrapMode]);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureWrapT, (int)_wraps[(int)_vWrapMode]);
            
            Bitmap bmp = _data.Bitmap;
            if (bmp != null)
            {
                BitmapData data = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                OpenTK.Graphics.OpenGL.GL.TexImage2D(
                    OpenTK.Graphics.OpenGL.TextureTarget.Texture2D,
                    0,
                    OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba8,
                    data.Width,
                    data.Height,
                    0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    OpenTK.Graphics.OpenGL.PixelType.UnsignedByte,
                    data.Scan0);

                //Engine.Renderer.BindTextureData(
                //    ETexTarget.Texture2D, 
                //    0, 
                //    EPixelInternalFormat.Rgba8,
                //    data.Width,
                //    data.Height,
                //    )

                bmp.UnlockBits(data);
            }
        }
        protected override void OnGenerated() => PushData();
    }
}
