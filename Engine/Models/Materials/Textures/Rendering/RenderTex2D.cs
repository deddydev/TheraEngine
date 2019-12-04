using Extensions;
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
            Mipmaps = null;
        }

        private int _width, _height;

        public override ETexTarget TextureTarget => ETexTarget.Texture2D;
        public bool Resizable { get; set; } = true;
        public int Width => Mipmaps is null ? _width : (Mipmaps.Length > 0 ? Mipmaps[0].Width : _width);
        public int Height => Mipmaps is null ? _height : (Mipmaps.Length > 0 ? Mipmaps[0].Height : _height);
        public Bitmap[] Mipmaps { get; set; }

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
        internal override void PushData()
        {
            Bitmap bmp = null;
            BitmapData data = null;
            try
            {
                OnPrePushData(out bool shouldPush, out bool allowPostPushCallback);
                if (!shouldPush)
                {
                    if (allowPostPushCallback)
                        OnPostPushData();
                    return;
                }

                Bind();

                ESizedInternalFormat sizedInternalFormat = (ESizedInternalFormat)(int)InternalFormat;

                if (Mipmaps is null || Mipmaps.Length == 0)
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
                        Engine.Renderer.SetTextureStorage(BindingId, Mipmaps.Length, sizedInternalFormat, Mipmaps[0].Width, Mipmaps[0].Height);
                        _storageSet = true;
                    }

                    for (int i = 0; i < Mipmaps.Length; ++i)
                    {
                        bmp = Mipmaps[i];
                        if (bmp != null)
                        {
                            data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

                            if (_hasPushed || setStorage)
                                Engine.Renderer.PushTextureSubData(TextureTarget, i, 0, 0, bmp.Width, bmp.Height, PixelFormat, PixelType, data.Scan0);
                            else
                                Engine.Renderer.PushTextureData(TextureTarget, i, InternalFormat, bmp.Width, bmp.Height, PixelFormat, PixelType, data.Scan0);

                            bmp.UnlockBits(data);
                            data = null;
                        }
                        else if (!(_hasPushed || setStorage))
                            Engine.Renderer.PushTextureData(TextureTarget, i, InternalFormat, _width, _height, PixelFormat, PixelType, IntPtr.Zero);
                    }
                }
                _hasPushed = true;

                //int max = _mipmaps is null || _mipmaps.Length == 0 ? 0 : _mipmaps.Length - 1;
                //Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureBaseLevel, 0);
                //Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMaxLevel, max);
                //Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMinLod, 0);
                //Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMaxLod, max);

                if (allowPostPushCallback)
                    OnPostPushData();
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
            finally
            {
                if (bmp != null && data != null)
                    bmp.UnlockBits(data);
            }
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

            if (Mipmaps != null)
            {
                if (mipLevel < 0)
                    for (int i = 0; i < Mipmaps.Length; ++i, width /= 2, height /= 2)
                        Mipmaps[i] = Mipmaps[i].Resized(width, height);
                else if (Mipmaps.IndexInRange(mipLevel))
                    Mipmaps[mipLevel] = Mipmaps[mipLevel].Resized(width, height);
            }

            //Destroy();
            //Generate();
            QueueRedraw();
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

                if (Mipmaps != null)
                    Array.ForEach(Mipmaps, x => x?.Dispose());
                
                _disposedValue = true;
            }
        }
    }
}
