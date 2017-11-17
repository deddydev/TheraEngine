using FreeImageAPI;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TheraEngine.Rendering.Textures
{
    public class Texture3D : BaseRenderTexture
    {
        public Texture3D() : this(null) { }
        public Texture3D(int bindingId) : base(EObjectType.Texture, bindingId) => Init(null);
        public Texture3D(params Bitmap[] mipmaps) : base(EObjectType.Texture) => Init(mipmaps);
        public Texture3D(
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
        public Texture3D(int bindingId, params Bitmap[] mipmaps) : base(EObjectType.Texture, bindingId) => Init(mipmaps);
        /// <summary>
        /// Initializes the texture as an unallocated texture to be filled by a framebuffer.
        /// </summary>
        public Texture3D(int width, int height, EPixelInternalFormat internalFormat, EPixelFormat pixelFormat, EPixelType pixelType) : this(null)
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
        
        private int _width, _height, _depth;
        private Bitmap3D[] _mipmaps;

        public override ETexTarget TextureTarget => ETexTarget.Texture3D;

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
        
        public static Texture3D[] GenTextures(int count)
            => Engine.Renderer.CreateObjects<Texture3D>(EObjectType.Texture, count);

        public void AttachToFrameBuffer(int frameBufferBindingId, EFramebufferAttachment attachment, int mipLevel = 0)
        {
            Engine.Renderer.AttachTextureToFrameBuffer(frameBufferBindingId, attachment, BindingId, mipLevel);
        }
        
        public override void PushData()
        {
            if (RenderPanel.NeedsInvoke(PushData, RenderPanel.PanelType.Rendering))
                return;

            Bind();
            OnPrePushData();

            if (_mipmaps == null || _mipmaps.Length == 0)
                Engine.Renderer.PushTextureData(TextureTarget, 0, InternalFormat, _width, _height, PixelFormat, PixelType, IntPtr.Zero);
            else
                for (int i = 0; i < _mipmaps.Length; ++i)
                {
                    Bitmap bmp = _mipmaps[i];
                    if (bmp != null)
                    {
                        BitmapData data = bmp.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                        Engine.Renderer.PushTextureData(TextureTarget, i, InternalFormat, _width, _height, PixelFormat, PixelType, data.Scan0);
                        bmp.UnlockBits(data);
                    }
                    else
                        Engine.Renderer.PushTextureData(TextureTarget, i, InternalFormat, _width, _height, PixelFormat, PixelType, IntPtr.Zero);
                }

            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureBaseLevel, 0);
            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMaxLevel, _mipmaps == null ? 0 : _mipmaps.Length - 1);
            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMinLod, 0);
            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMaxLod, _mipmaps == null ? 0 : _mipmaps.Length - 1);

            OnPostPushData();
        }
        public void Resize(int width, int height, int depth)
        {
            _width = width;
            _height = height;
            _depth = depth;

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
