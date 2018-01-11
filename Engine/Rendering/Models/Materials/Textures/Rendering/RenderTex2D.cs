using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TheraEngine.Rendering.Models.Materials.Textures
{
    public class RenderTex2DMipmap
    {

    }
    public class RenderTex2D : BaseRenderTexture
    {
        public static RenderTex2D[] GenTextures(int count)
            => Engine.Renderer.CreateObjects<RenderTex2D>(EObjectType.Texture, count);

        public RenderTex2D() : this(null) { }
        public RenderTex2D(int bindingId) : base(bindingId) => Mipmaps = null;
        public RenderTex2D(params Bitmap[] mipmaps) : base() => Mipmaps = mipmaps;
        public RenderTex2D(
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
        public RenderTex2D(int bindingId, params Bitmap[] mipmaps) : base(bindingId) => Mipmaps = mipmaps;
        /// <summary>
        /// Initializes the texture as an unallocated texture to be filled by a framebuffer.
        /// </summary>
        public RenderTex2D(int width, int height, EPixelInternalFormat internalFormat, EPixelFormat pixelFormat, EPixelType pixelType) : this(null)
        {
            _width = width;
            _height = height;
            InternalFormat = internalFormat;
            PixelFormat = pixelFormat;
            PixelType = pixelType;
            _mipmaps = null;
        }

        private int _width, _height;
        private Bitmap[] _mipmaps;

        public override ETexTarget TextureTarget => ETexTarget.Texture2D;
        public int Width => _mipmaps.Length > 0 ? _mipmaps[0].Width : _width;
        public int Height => _mipmaps.Length > 0 ? _mipmaps[0].Height : _height;
        public Bitmap[] Mipmaps
        {
            get => _mipmaps;
            set
            {
                _mipmaps = value;
                //if (_mipmaps != null && _mipmaps.Length > 0)
                //{
                //    Bitmap b = _mipmaps[0];

                //        _width = b.Width;
                //        _height = b.Height;
                    
                //}
                //else
                //{
                //    _width = 0;
                //    _height = 0;
                //}
            }
        }

        public override void PushData()
        {
            if (BaseRenderPanel.NeedsInvoke(PushData, BaseRenderPanel.PanelType.Rendering))
                return;
            
            Bind();
            OnPrePushData();

            if (_mipmaps == null || _mipmaps.Length == 0)
                Engine.Renderer.PushTextureData(TextureTarget, 0, InternalFormat, _width, _height, PixelFormat, PixelType, IntPtr.Zero);
            else
            {
                for (int i = 0; i < _mipmaps.Length; ++i)
                {
                    Bitmap bmp = _mipmaps[i];
                    if (bmp != null)
                    {
                        BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                        Engine.Renderer.PushTextureData(TextureTarget, i, InternalFormat, bmp.Width, bmp.Height, PixelFormat, PixelType, data.Scan0);
                        bmp.UnlockBits(data);
                    }
                    else
                        Engine.Renderer.PushTextureData(TextureTarget, i, InternalFormat, _width, _height, PixelFormat, PixelType, IntPtr.Zero);
                }
                if (_mipmaps.Length == 1)
                    Engine.Renderer.GenerateMipmap(TextureTarget);
            }

            int max = _mipmaps == null || _mipmaps.Length == 0 ? 0 : _mipmaps.Length - 1;
            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureBaseLevel, 0);
            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMaxLevel, max);
            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMinLod, 0);
            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMaxLod, max);

            OnPostPushData();
        }
        public void Resize(int width, int height)
        {
            if (_mipmaps != null && _mipmaps.Length > 0)
            {
                _mipmaps[0] = _mipmaps[0].Resized(width, height);

                double wratio = (double)width / _width;
                double hratio = (double)height / _height;

                for (int i = 1; i < _mipmaps.Length; ++i)
                {
                    Bitmap bmp = _mipmaps[i];
                    _mipmaps[i] = bmp.Resized((int)(bmp.Width * wratio), (int)(bmp.Height * wratio));
                }
            }

            _width = width;
            _height = height;

            PushData();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Destroy();
                }

                if (_mipmaps != null)
                    Array.ForEach(_mipmaps, x => x?.Dispose());

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }
    }
}
