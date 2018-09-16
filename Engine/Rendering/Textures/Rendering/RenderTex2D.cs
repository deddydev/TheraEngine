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
        public bool Resizable { get; set; } = true;
        public int Width => _mipmaps == null ? _width : (_mipmaps.Length > 0 ? _mipmaps[0].Width : _width);
        public int Height => _mipmaps == null ? _height : (_mipmaps.Length > 0 ? _mipmaps[0].Height : _height);
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

        public override int MaxDimension => Math.Max(Width, Height);

        private bool _hasPushed = false;
        private bool _storageSet = false;

        protected override void PostGenerated()
        {
            _hasPushed = false;
            _storageSet = false;
            base.PostGenerated();
        }
        protected override void PostDeleted()
        {
            _hasPushed = false;
            _storageSet = false;
            base.PostDeleted();
        }

        //TODO: use PBO per texture for quick data updates
        public override void PushData()
        {
            if (BaseRenderPanel.ThreadSafeBlockingInvoke((Action)PushData, BaseRenderPanel.PanelType.Rendering))
                return;

            OnPrePushData(out bool shouldPush, out bool allowPostPushCallback);
            if (!shouldPush)
            {
                if (allowPostPushCallback)
                    OnPostPushData();
                return;
            }

            Bind();

            ESizedInternalFormat sizedInternalFormat = (ESizedInternalFormat)(int)InternalFormat;
            
            if (_mipmaps == null || _mipmaps.Length == 0)
            {
                if (!Resizable && !_storageSet)
                {
                    Engine.Renderer.SetTextureStorage(BindingId, 1, sizedInternalFormat, _width, _height);
                    _storageSet = true;
                }
                else if (!_storageSet)
                    Engine.Renderer.PushTextureData(TextureTarget, 0, InternalFormat, _width, _height, PixelFormat, PixelType, IntPtr.Zero);

                if (AutoGenerateMipmaps)
                {
                    SetMipmapGenParams();
                    GenerateMipmaps();
                }
            }
            else
            {
                bool setStorage = !Resizable && !_storageSet;
                if (setStorage)
                {
                    Engine.Renderer.SetTextureStorage(BindingId, _mipmaps.Length, sizedInternalFormat, _mipmaps[0].Width, _mipmaps[0].Height);
                    _storageSet = true;
                }
                
                for (int i = 0; i < _mipmaps.Length; ++i)
                {
                    Bitmap bmp = _mipmaps[i];
                    if (bmp != null)
                    {
                        BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

                        if (_hasPushed || setStorage)
                            Engine.Renderer.PushTextureSubData(TextureTarget, i, 0, 0, bmp.Width, bmp.Height, PixelFormat, PixelType, data.Scan0);
                        else
                            Engine.Renderer.PushTextureData(TextureTarget, i, InternalFormat, bmp.Width, bmp.Height, PixelFormat, PixelType, data.Scan0);

                        bmp.UnlockBits(data);
                    }
                    else if (!(_hasPushed || setStorage))
                        Engine.Renderer.PushTextureData(TextureTarget, i, InternalFormat, _width, _height, PixelFormat, PixelType, IntPtr.Zero);
                }
            }
            _hasPushed = true;

            //int max = _mipmaps == null || _mipmaps.Length == 0 ? 0 : _mipmaps.Length - 1;
            //Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureBaseLevel, 0);
            //Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMaxLevel, max);
            //Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMinLod, 0);
            //Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMaxLod, max);

            if (allowPostPushCallback)
                OnPostPushData();
        }
        public void Resize(int width, int height, int mipLevel = -1)
        {
            if (!Resizable)
            {
                Engine.LogWarning("Tried to resize texture that is immutable (storage size is non-resizable).");
                return;
            }

            _storageSet = false;
            _hasPushed = false;
            _width = width;
            _height = height;

            if (_mipmaps != null)
            {
                if (mipLevel < 0)
                    for (int i = 0; i < _mipmaps.Length; ++i, width /= 2, height /= 2)
                        _mipmaps[i] = _mipmaps[i].Resized(width, height);
                else if (_mipmaps.IndexInArrayRange(mipLevel))
                    _mipmaps[mipLevel] = _mipmaps[mipLevel].Resized(width, height);
            }

            //Destroy();
            //Generate();
            PushData();
        }
        public override void Destroy()
        {
            base.Destroy();
            _hasPushed = false;
            _storageSet = false;
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
                
                _disposedValue = true;
            }
        }
    }
}
