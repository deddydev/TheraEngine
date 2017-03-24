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
        public Texture() : base(GenType.Texture)
        {
            _width = 1;
            _height = 1;
        }
        public Texture(int bindingId) : base(GenType.Texture, bindingId)
        {
            _width = 1;
            _height = 1;
        }
        public Texture(TextureData data) : base(GenType.Texture)
        {
            _data = data;
            _width = _data != null && _data.Bitmap != null ? _data.Bitmap.Width : 1;
            _height = _data != null && _data.Bitmap != null ? _data.Bitmap.Height : 1;
        }
        public Texture(
            TextureData data,
            MinFilter minFilter,
            MagFilter magFilter,
            TexCoordWrap uWrap,
            TexCoordWrap vWrap,
            float lodBias)
            : this(data)
        {
            _minFilter = minFilter;
            _magFilter = magFilter;
            _uWrapMode = uWrap;
            _vWrapMode = vWrap;
            _lodBias = lodBias;
        }

        public Texture(TextureData data, MinFilter minFilter, MagFilter magFilter, TexCoordWrap uWrap, TexCoordWrap vWrap, float lodBias, 
            EPixelInternalFormat internalFormat, EPixelFormat pixelFormat, EPixelType pixelType)
            : this(data, minFilter, magFilter, uWrap, vWrap, lodBias)
        {
            _internalFormat = internalFormat;
            _pixelFormat = pixelFormat;
            _pixelType = pixelType;
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

        private int _width, _height;
        private TexCoordWrap _uWrapMode;
        private TexCoordWrap _vWrapMode;
        private MinFilter _minFilter;
        private MagFilter _magFilter;
        private float _lodBias;
        private TextureData _data;
        private EPixelInternalFormat _internalFormat = EPixelInternalFormat.Rgba8;
        private EPixelFormat _pixelFormat = EPixelFormat.Bgra;
        private EPixelType _pixelType = EPixelType.Byte;
        private ETexTarget _textureTarget = ETexTarget.Texture2D;

        public TextureData Data
        {
            get => _data;
            set
            {
                Delete();
                _data = value;
                _width = _data != null && _data.Bitmap != null ? _data.Bitmap.Width : 1;
                _height = _data != null && _data.Bitmap != null ? _data.Bitmap.Height : 1;
            }
        }

        public static Texture[] GenTextures(int count)
            => Engine.Renderer.CreateObjects<Texture>(GenType.Texture, count);
        public void AttachToFrameBuffer(EFramebufferTarget target, EFramebufferAttachment attachment)
            => Engine.Renderer.AttachTextureToFrameBuffer(target, attachment, _textureTarget, BindingId, 0);

        public void Bind()
        {
            if (!IsActive)
                Generate();
            Engine.Renderer.BindTexture(_textureTarget, BindingId);
        }
        public void PushData()
        {
            if (_data == null)
                return;

            Bind();

            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureBaseLevel, 0);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureMaxLevel, 0);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureMinLod, 0);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureMaxLod, 0);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureLodBias, _lodBias);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureMagFilter, (int)_magFilters[(int)_magFilter]);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureMinFilter, (int)_minFilters[(int)_minFilter]);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureWrapS, (int)_wraps[(int)_uWrapMode]);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureWrapT, (int)_wraps[(int)_vWrapMode]);
            
            Bitmap bmp = _data.Bitmap;
            if (bmp != null)
            {
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                Engine.Renderer.PushTextureData(_textureTarget, 0, _internalFormat, _width, _height, _pixelFormat, _pixelType, data.Scan0);
                bmp.UnlockBits(data);
            }
            else
            {
                Engine.Renderer.PushTextureData(_textureTarget, 0, _internalFormat, _width, _height, _pixelFormat, _pixelType, IntPtr.Zero);
            }
        }

        protected override int CreateObject()
            => Engine.Renderer.CreateTextures(3553, 1)[0];
        protected override void OnGenerated()
            => PushData();
    }
}
