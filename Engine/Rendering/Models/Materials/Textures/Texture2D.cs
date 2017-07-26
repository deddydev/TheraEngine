using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TheraEngine.Rendering.Textures
{
    public enum TextureType
    {
        Texture2D,
        Texture3D,
        TextureCubeMap
    }
    public class Texture2D : BaseRenderState, IDisposable
    {
        public Texture2D() : this(null) { }
        public Texture2D(int bindingId) : base(EObjectType.Texture, bindingId) => Init(null);
        public Texture2D(params Bitmap[] mipmaps) : base(EObjectType.Texture) => Init(mipmaps);
        public Texture2D(
            EPixelInternalFormat internalFormat,
            EPixelFormat pixelFormat,
            EPixelType pixelType,
            params Bitmap[] mipmaps)
            : this(mipmaps)
        {
            InternalFormat = internalFormat;
            PixelFormat = pixelFormat;
            PixelType = pixelType;
        }
        public Texture2D(int bindingId, params Bitmap[] mipmaps) : base(EObjectType.Texture, bindingId) => Init(mipmaps);
        /// <summary>
        /// Initializes the texture as an unallocated texture to be filled by a framebuffer.
        /// </summary>
        public Texture2D(int width, int height, EPixelInternalFormat internalFormat, EPixelFormat pixelFormat, EPixelType pixelType) : this(null)
        {
            _width = width;
            _height = height;
            InternalFormat = internalFormat;
            PixelFormat = pixelFormat;
            PixelType = pixelType;
            _mipmaps = null;
        }
        private void Init(params Bitmap[] mipmaps)
        {
            _mipmaps = mipmaps;
            _width = mipmaps != null && mipmaps.Length > 0 ? mipmaps[0].Width : 1;
            _height = mipmaps != null && mipmaps.Length > 0 ? mipmaps[0].Height : 1;
        }
        
        private int _width, _height;
        private EPixelInternalFormat _internalFormat;
        private EPixelFormat _pixelFormat;
        private EPixelType _pixelType;
        private ETexTarget _textureTarget = ETexTarget.Texture2D;
        private Bitmap[] _mipmaps;

        public EPixelInternalFormat InternalFormat { get => _internalFormat; set => _internalFormat = value; }
        public EPixelFormat PixelFormat { get => _pixelFormat; set => _pixelFormat = value; }
        public EPixelType PixelType { get => _pixelType; set => _pixelType = value; }
        public Bitmap[] Mipmaps
        {
            get => _mipmaps;
            set
            {
                _mipmaps = value;
                _width = _mipmaps != null && _mipmaps.Length > 0 ? _mipmaps[0].Width : 1;
                _height = _mipmaps != null && _mipmaps.Length > 0 ? _mipmaps[0].Height : 1;
            }
        }

        public int Width => _width;
        public int Height => _height;

        public event Action PrePushData;
        public event Action PostPushData;
        
        public static Texture2D[] GenTextures(int count)
            => Engine.Renderer.CreateObjects<Texture2D>(EObjectType.Texture, count);

        public void AttachToFrameBuffer(int frameBufferBindingId, EFramebufferAttachment attachment, int mipLevel = 0)
        {
            Engine.Renderer.AttachTextureToFrameBuffer(frameBufferBindingId, attachment, BindingId, mipLevel);
        }

        public void Bind()
        {
            Engine.Renderer.BindTexture(_textureTarget, BindingId);
        }
        public void PushData()
        {
            if (RenderPanel.NeedsInvoke(PushData, RenderPanel.PanelType.Rendering))
                return;

            Bind();

            PrePushData?.Invoke();

            if (_mipmaps == null || _mipmaps.Length == 0)
                Engine.Renderer.PushTextureData(_textureTarget, 0, InternalFormat, _width, _height, PixelFormat, PixelType, IntPtr.Zero);
            else
                for (int i = 0; i < _mipmaps.Length; ++i)
                {
                    Bitmap bmp = _mipmaps[i];
                    if (bmp != null)
                    {
                        BitmapData data = bmp.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                        Engine.Renderer.PushTextureData(_textureTarget, i, InternalFormat, _width, _height, PixelFormat, PixelType, data.Scan0);
                        bmp.UnlockBits(data);
                    }
                    else
                        Engine.Renderer.PushTextureData(_textureTarget, i, InternalFormat, _width, _height, PixelFormat, PixelType, IntPtr.Zero);
                }

            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureBaseLevel, 0);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureMaxLevel, _mipmaps == null ? 0 : _mipmaps.Length - 1);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureMinLod, 0);
            Engine.Renderer.TexParameter(_textureTarget, ETexParamName.TextureMaxLod, _mipmaps == null ? 0 : _mipmaps.Length - 1);

            PostPushData?.Invoke();
        }
        public void Resize(int width, int height)
        {
            _width = width;
            _height = height;

            if (_mipmaps != null && _mipmaps.Length > 0)
            {
                _mipmaps[0] = _mipmaps[0].Resized(_width, _height);

                double wratio = (double)width / _width;
                double hratio = (double)height / _height;

                for (int i = 1; i < _mipmaps.Length; ++i)
                {
                    Bitmap bmp = _mipmaps[i];
                    _mipmaps[i] = bmp.Resized((int)(bmp.Width * wratio), (int)(bmp.Height * wratio));
                }
            }

            PushData();
        }
        protected override int CreateObject()
            => Engine.Renderer.CreateTextures(ETexTarget.Texture2D, 1)[0];
        protected override void OnGenerated()
            => PushData();

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Destroy();
                    if (_mipmaps != null)
                        Array.ForEach(_mipmaps, x => x?.Dispose());
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }
    }
}
