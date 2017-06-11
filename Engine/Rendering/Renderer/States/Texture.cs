using TheraEngine.Rendering.Models.Materials;
using FreeImageAPI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace TheraEngine.Rendering.Textures
{
    public class Texture : BaseRenderState
    {
        public Texture() : this(null) { }
        public Texture(TextureData data, int bindingId) : base(GenType.Texture, bindingId)
        {
            _data = data;
            _width = _data != null && _data.Bitmap != null ? _data.Bitmap.Width : 1;
            _height = _data != null && _data.Bitmap != null ? _data.Bitmap.Height : 1;
            _internalFormat = EPixelInternalFormat.Four;
            _pixelFormat = EPixelFormat.Bgra;
            _pixelType = EPixelType.UnsignedByte;
        }
        public Texture(TextureData data) : base(GenType.Texture)
        {
            _data = data;
            _width = _data != null && _data.Bitmap != null ? _data.Bitmap.Width : 1;
            _height = _data != null && _data.Bitmap != null ? _data.Bitmap.Height : 1;
            _internalFormat = EPixelInternalFormat.Four;
            _pixelFormat = EPixelFormat.Bgra;
            _pixelType = EPixelType.UnsignedByte;
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
            _internalFormat = EPixelInternalFormat.Four;
            _pixelFormat = EPixelFormat.Bgra;
            _pixelType = EPixelType.UnsignedByte;
        }

        public Texture(
            int width,
            int height,
            MinFilter minFilter,
            MagFilter magFilter,
            TexCoordWrap uWrap,
            TexCoordWrap vWrap,
            float lodBias,
            EPixelInternalFormat internalFormat,
            EPixelFormat pixelFormat,
            EPixelType pixelType)
            : this(null, minFilter, magFilter, uWrap, vWrap, lodBias)
        {
            _width = width;
            _height = height;
            _internalFormat = internalFormat;
            _pixelFormat = pixelFormat;
            _pixelType = pixelType;
        }

        public Texture(
            TextureData data,
            MinFilter minFilter,
            MagFilter magFilter,
            TexCoordWrap uWrap,
            TexCoordWrap vWrap,
            float lodBias,
            EPixelInternalFormat internalFormat,
            EPixelFormat pixelFormat,
            EPixelType pixelType)
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

        private int _index;
        private int _width, _height;
        private TexCoordWrap _uWrapMode;
        private TexCoordWrap _vWrapMode;
        private MinFilter _minFilter;
        private MagFilter _magFilter;
        private float _lodBias;
        private TextureData _data;
        private EPixelInternalFormat _internalFormat;
        private EPixelFormat _pixelFormat;
        private EPixelType _pixelType;
        private ETexTarget _textureTarget = ETexTarget.Texture2D;

        public event Action<int> PostPushData;

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

        public int Index { get => _index; set => _index = value; }

        public static Texture[] GenTextures(int count)
            => Engine.Renderer.CreateObjects<Texture>(GenType.Texture, count);
        //public void AttachToFrameBuffer(int frameBufferBindingId, EFramebufferAttachment attachment)
        //{
        //    Engine.Renderer.BindTexture(ETexTarget.Texture2D, BindingId);
        //    Engine.Renderer.AttachTextureToFrameBuffer(frameBufferBindingId, attachment, BindingId, 0);
        //}
        public void Bind()
        {
            Engine.Renderer.BindTexture(_textureTarget, BindingId);
        }
        public void PushData()
        {
            Bind();

            Bitmap bmp = _data?.Bitmap;
            if (bmp != null)
            {
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                Engine.Renderer.PushTextureData(_textureTarget, 0, _internalFormat, _width, _height, _pixelFormat, _pixelType, data.Scan0);
                bmp.UnlockBits(data);
            }
            else
                Engine.Renderer.PushTextureData(_textureTarget, 0, _internalFormat, _width, _height, _pixelFormat, _pixelType, IntPtr.Zero);

            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureBaseLevel, 0);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureMaxLevel, 0);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureMinLod, 0);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureMaxLod, 0);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureLodBias, _lodBias);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureMagFilter, (int)_magFilters[(int)_magFilter]);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureMinFilter, (int)_minFilters[(int)_minFilter]);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureWrapS, (int)_wraps[(int)_uWrapMode]);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureWrapT, (int)_wraps[(int)_vWrapMode]);

            PostPushData?.Invoke(Index);
        }
        public void Resize(int width, int height)
        {
            _width = width;
            _height = height;
            if (_data != null && _data.Bitmap != null)
                _data.Bitmap = _data.Bitmap.Resized(width, height);
            PushData();
        }
        protected override int CreateObject()
            => Engine.Renderer.CreateTextures(ETexTarget.Texture2D, 1)[0];
        protected override void OnGenerated()
            => PushData();
    }
}
