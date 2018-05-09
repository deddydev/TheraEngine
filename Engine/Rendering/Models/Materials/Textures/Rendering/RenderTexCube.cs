using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TheraEngine.Rendering.Models.Materials.Textures
{
    public class RenderTexCube : BaseRenderTexture
    {
        public RenderTexCube() : this(null) { }
        public RenderTexCube(int bindingId) : base(bindingId) => Mipmaps = null;
        public RenderTexCube(params RenderCubeMipmap[] mipmaps) : base() => Mipmaps = mipmaps;
        public RenderTexCube(
            EPixelInternalFormat internalFormat,
            EPixelFormat pixelFormat,
            EPixelType pixelType,
            params RenderCubeMipmap[] mipmaps)
            : this(mipmaps)
        {
            InternalFormat = internalFormat;
            PixelFormat = pixelFormat;
            PixelType = pixelType;
        }
        public RenderTexCube(int bindingId, params RenderCubeMipmap[] mipmaps) 
            : base(bindingId) => Mipmaps = mipmaps;
        /// <summary>
        /// Initializes the texture as an unallocated texture to be filled by a framebuffer.
        /// </summary>
        public RenderTexCube(
            int cubeExtent,
            EPixelInternalFormat internalFormat,
            EPixelFormat pixelFormat,
            EPixelType pixelType) 
            : this(null)
        {
            InternalFormat = internalFormat;
            PixelFormat = pixelFormat;
            PixelType = pixelType;
            _mipmaps = null;
            _dimension = cubeExtent;
        }

        private int _dimension;
        private RenderCubeMipmap[] _mipmaps;

        private bool _hasPushed = false;
        private bool _storageSet = false;

        public override ETexTarget TextureTarget => ETexTarget.TextureCubeMap;
        public bool Resizable { get; set; } = true;

        public RenderCubeMipmap[] Mipmaps
        {
            get => _mipmaps;
            set => _mipmaps = value;
        }
        public int Dimension => _mipmaps == null ? _dimension : (_mipmaps.Length > 0 ? _mipmaps[0].Sides[0].Width : _dimension);

        public override int MaxDimension => Dimension;

        public static RenderTexCube[] GenTextures(int count)
            => Engine.Renderer.CreateObjects<RenderTexCube>(EObjectType.Texture, count);
        
        public override void PushData()
        {
            if (BaseRenderPanel.NeedsInvoke(PushData, BaseRenderPanel.PanelType.Rendering))
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
                int mipCount = SmallestMipmapLevel + 1;
                if (!Resizable && !_storageSet)
                {
                    Engine.Renderer.SetTextureStorage(BindingId, mipCount, sizedInternalFormat, _dimension, _dimension);
                    _storageSet = true;
                }
                else if (!_storageSet)
                {
                    int dim = _dimension;
                    for (int i = 0; i < mipCount; ++i)
                    {
                        for (int x = 0; x < 6; ++x)
                        {
                            ETexTarget target = ETexTarget.TextureCubeMapPositiveX + x;
                            Engine.Renderer.PushTextureData(target, i, InternalFormat, dim, dim, PixelFormat, PixelType, IntPtr.Zero);
                        }
                        dim /= 2;
                    }
                }

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
                    Engine.Renderer.SetTextureStorage(BindingId, _mipmaps.Length, sizedInternalFormat, _mipmaps[0].Sides[0].Width, _mipmaps[0].Sides[0].Height);
                    _storageSet = true;
                }

                for (int i = 0; i < _mipmaps.Length; ++i)
                {
                    RenderCubeMipmap mip = _mipmaps[i];
                    for (int x = 0; x < mip.Sides.Length; ++x)
                    {
                        RenderCubeSide side = mip.Sides[x];
                        Bitmap bmp = side.Map;

                        ETexTarget target = ETexTarget.TextureCubeMapPositiveX + x;
                        if (bmp != null)
                        {
                            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

                            if (_hasPushed || setStorage)
                                Engine.Renderer.PushTextureSubData(target, i, 0, 0, bmp.Width, bmp.Height, side.PixelFormat, side.PixelType, data.Scan0);
                            else
                                Engine.Renderer.PushTextureData(target, i, side.InternalFormat, bmp.Width, bmp.Height, side.PixelFormat, side.PixelType, data.Scan0);

                            bmp.UnlockBits(data);
                        }
                        else if (!(_hasPushed || setStorage))
                            Engine.Renderer.PushTextureData(target, i, side.InternalFormat, side.Width, side.Height, side.PixelFormat, side.PixelType, IntPtr.Zero);
                    }
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

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Destroy();
                }

                if (_mipmaps != null)
                    Array.ForEach(_mipmaps, x => x.Dispose());

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }
    }
}
