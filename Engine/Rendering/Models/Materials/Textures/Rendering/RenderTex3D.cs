using System;

namespace TheraEngine.Rendering.Models.Materials.Textures
{
    public class RenderTex3D : BaseRenderTexture
    {
        public RenderTex3D() : this(null) { }
        public RenderTex3D(int bindingId) : base(bindingId) => Init(null);
        public RenderTex3D(params TBitmap3D[] mipmaps) : base() => Init(mipmaps);
        public RenderTex3D(
            EPixelInternalFormat internalFormat,
            EPixelFormat pixelFormat,
            EPixelType pixelType,
            params TBitmap3D[] mipmaps)
            : this(mipmaps)
        {
            InternalFormat = internalFormat;
            PixelFormat = pixelFormat;
            PixelType = pixelType;
        }
        public RenderTex3D(int bindingId, params TBitmap3D[] mipmaps) : base(bindingId) => Init(mipmaps);
        /// <summary>
        /// Initializes the texture as an unallocated texture to be filled by a framebuffer.
        /// </summary>
        public RenderTex3D(int width, int height, int depth, EPixelInternalFormat internalFormat, EPixelFormat pixelFormat, EPixelType pixelType) : this(null)
        {
            _width = width;
            _height = height;
            _depth = depth;
            InternalFormat = internalFormat;
            PixelFormat = pixelFormat;
            PixelType = pixelType;
            _mipmaps = null;
        }
        private void Init(params TBitmap3D[] mipmaps)
        {
            _mipmaps = mipmaps;
            _width = mipmaps != null && mipmaps.Length > 0 ? mipmaps[0].Width : 1;
            _height = mipmaps != null && mipmaps.Length > 0 ? mipmaps[0].Height : 1;
        }
        
        private int _width, _height, _depth;
        private TBitmap3D[] _mipmaps;

        public override ETexTarget TextureTarget => ETexTarget.Texture3D;

        public TBitmap3D[] Mipmaps
        {
            get => _mipmaps;
            set
            {
                _mipmaps = value;
                _width = _mipmaps != null && _mipmaps.Length > 0 ? _mipmaps[0].Width : 1;
                _height = _mipmaps != null && _mipmaps.Length > 0 ? _mipmaps[0].Height : 1;
                _depth = _mipmaps != null && _mipmaps.Length > 0 ? _mipmaps[0].Depth : 1;
            }
        }

        public int Width => _width;
        public int Height => _height;
        public int Depth => _depth;

        public override int MaxDimension => TMath.Max(Width, Height, Depth);

        public static RenderTex3D[] GenTextures(int count)
            => Engine.Renderer.CreateObjects<RenderTex3D>(EObjectType.Texture, count);
        
        public override void PushData()
        {
            if (BaseRenderPanel.NeedsInvoke(PushData, BaseRenderPanel.PanelType.Rendering))
                return;
            
            Bind();
            OnPrePushData(out bool shouldPush, out bool allowPostPushCallback);

            if (_mipmaps == null || _mipmaps.Length == 0)
                Engine.Renderer.PushTextureData(TextureTarget, 0, InternalFormat, _width, _height, PixelFormat, PixelType, IntPtr.Zero);
            else
                for (int i = 0; i < _mipmaps.Length; ++i)
                {
                    TBitmap3D bmp = _mipmaps[i];
                    if (bmp != null)
                    {
                        //BitmapData data = bmp.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                        Engine.Renderer.PushTextureData(TextureTarget, i, InternalFormat, _width, _height, PixelFormat, PixelType, bmp.Scan0);
                        //bmp.UnlockBits(data);
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
            if (_mipmaps != null && _mipmaps.Length > 0)
            {
                _mipmaps[0] = _mipmaps[0].Resized(width, height, depth);

                double wratio = (double)width / _width;
                double hratio = (double)height / _height;
                double dratio = (double)depth / _depth;

                for (int i = 1; i < _mipmaps.Length; ++i)
                {
                    TBitmap3D bmp = _mipmaps[i];
                    _mipmaps[i] = bmp.Resized((int)(bmp.Width * wratio), (int)(bmp.Height * wratio), (int)(bmp.Depth * dratio));
                }
            }

            _width = width;
            _height = height;
            _depth = depth;

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
