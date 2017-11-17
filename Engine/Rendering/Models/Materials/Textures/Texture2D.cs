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
    public abstract class BaseRenderTexture : BaseRenderState, IDisposable
    {
        public event Action PrePushData;
        public event Action PostPushData;

        protected void OnPrePushData() => PrePushData?.Invoke();
        protected void OnPostPushData() => PostPushData?.Invoke();

        public BaseRenderTexture() : base(EObjectType.Texture) { }
        public BaseRenderTexture(int bindingId) : base(EObjectType.Texture, bindingId) { }
        
        public EPixelInternalFormat InternalFormat { get; set; }
        public EPixelFormat PixelFormat { get; set; }
        public EPixelType PixelType { get; set; }

        public abstract ETexTarget TextureTarget { get; }

        public static T[] GenTextures<T>(int count) where T : BaseRenderTexture
            => Engine.Renderer.CreateObjects<T>(EObjectType.Texture, count);

        public void Bind()
            => Engine.Renderer.BindTexture(TextureTarget, BindingId);
        public void Clear(ColorF4 clearColor, int level = 0)
            => Engine.Renderer.ClearTexImage(BindingId, level, clearColor);

        protected override int CreateObject()
            => Engine.Renderer.CreateTextures(TextureTarget, 1)[0];
        protected override void OnGenerated()
            => PushData();

        public abstract void PushData();
    }
    public class Texture2D : BaseRenderTexture
    {
        public static Texture2D[] GenTextures(int count)
            => Engine.Renderer.CreateObjects<Texture2D>(EObjectType.Texture, count);

        public Texture2D() : this(null) { }
        public Texture2D(int bindingId) : base(bindingId) => Init(null);
        public Texture2D(params Bitmap[] mipmaps) : base() => Init(mipmaps);
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
        public Texture2D(int bindingId, params Bitmap[] mipmaps) : base(bindingId) => Init(mipmaps);
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
        private Bitmap[] _mipmaps;

        public override ETexTarget TextureTarget => ETexTarget.Texture2D;
        public int Width => _width;
        public int Height => _height;
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
