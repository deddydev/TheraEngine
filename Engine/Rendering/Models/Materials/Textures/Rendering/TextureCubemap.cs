using FreeImageAPI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Rendering.Models.Materials.Textures
{
    public class RenderCubeMipMap
    {
        public Bitmap[] Sides { get; private set; }
        
        public RenderCubeMipMap(Bitmap cubeCrossBmp)
        {
            int w = cubeCrossBmp.Width;
            int h = cubeCrossBmp.Height;
            if (w % 4 == 0 && w / 4 * 3 == h)
            {
                //Cross is on its side.
                //     __
                //  __|__|__ __        +Y
                // |__|__|__|__|   -X, -Z, +X, +Z
                //    |__|             -Y

                int dim = w / 4;
                Sides = new Bitmap[]
                {
                    cubeCrossBmp.Clone(new Rectangle(dim * 2, dim, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(0, dim, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim, 0, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim, dim * 2, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim * 3, dim, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim, dim, dim, dim), cubeCrossBmp.PixelFormat),
                };
            }
            else if (h % 4 == 0 && h / 4 * 3 == w)
            {
                //Cross is standing up.
                //     __
                //  __|__|__        +Y
                // |__|__|__|   -X, -Z, +X
                //    |__|          -Y
                //    |__|          +Z

                int dim = h / 4;
                Sides = new Bitmap[]
                {
                    cubeCrossBmp.Clone(new Rectangle(dim * 2, dim, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(0, dim, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim, 0, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim, dim * 2, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim, dim * 3, dim, dim), cubeCrossBmp.PixelFormat),
                    cubeCrossBmp.Clone(new Rectangle(dim, dim, dim, dim), cubeCrossBmp.PixelFormat),
                };
            }
            else
            {
                throw new InvalidOperationException("Cubemap cross dimensions are invalid.");
            }
        }

        public RenderCubeMipMap(
            Bitmap posX,
            Bitmap negX,
            Bitmap posY,
            Bitmap negY,
            Bitmap posZ,
            Bitmap negZ)
        {
            Sides = new Bitmap[6] { posX, negX, posY, negY, posZ, negZ };
        }

        public RenderCubeMipMap(int dim, PixelFormat bitmapFormat)
        {
            Sides = new Bitmap[6];
            Sides.FillWith(i => new Bitmap(dim, dim, bitmapFormat));
        }

        public void PushData(int i)
        {

        }
    }
    public class TextureCubemap : BaseRenderTexture
    {
        public TextureCubemap() : this(null) { }
        public TextureCubemap(int bindingId) : base(bindingId) => Mipmaps = null;
        public TextureCubemap(params RenderCubeMipMap[] mipmaps) : base() => Mipmaps = mipmaps;
        public TextureCubemap(
            EPixelInternalFormat internalFormat,
            EPixelFormat pixelFormat,
            EPixelType pixelType,
            params RenderCubeMipMap[] mipmaps)
            : this(mipmaps)
        {
            InternalFormat = internalFormat;
            PixelFormat = pixelFormat;
            PixelType = pixelType;
        }
        public TextureCubemap(int bindingId, params RenderCubeMipMap[] mipmaps) 
            : base(bindingId) => Mipmaps = mipmaps;
        /// <summary>
        /// Initializes the texture as an unallocated texture to be filled by a framebuffer.
        /// </summary>
        public TextureCubemap(
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
        }
        
        private RenderCubeMipMap[] _mipmaps;

        public override ETexTarget TextureTarget => ETexTarget.TextureCubeMap;

        public RenderCubeMipMap[] Mipmaps
        {
            get => _mipmaps;
            set => _mipmaps = value;
        }
        
        public int NullCubeExtent => _cubeExtent;

        public static TextureCubemap[] GenTextures(int count)
            => Engine.Renderer.CreateObjects<TextureCubemap>(EObjectType.Texture, count);
        
        public override void PushData()
        {
            if (BaseRenderPanel.NeedsInvoke(PushData, BaseRenderPanel.PanelType.Rendering))
                return;
            
            Bind();
            OnPrePushData();

            if (_mipmaps == null || _mipmaps.Length == 0)
                Engine.Renderer.PushTextureData(TextureTarget, 0, InternalFormat, 1, 1, PixelFormat, PixelType, IntPtr.Zero);
            else
                for (int i = 0; i < _mipmaps.Length; ++i)
                {
                    RenderCubeMipMap mip = _mipmaps[i];
                    mip.PushData(i);
                }

            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureBaseLevel, 0);
            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMaxLevel, _mipmaps == null ? 0 : _mipmaps.Length - 1);
            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMinLod, 0);
            Engine.Renderer.TexParameter(TextureTarget, ETexParamName.TextureMaxLod, _mipmaps == null ? 0 : _mipmaps.Length - 1);

            OnPostPushData();
        }
        public void Resize(int cubeExtent)
        {
            if (_mipmaps != null && _mipmaps.Length > 0)
            {
                _mipmaps[0] = _mipmaps[0].Resized(cubeExtent, cubeExtent);

                double wratio = (double)cubeExtent / _width;
                double hratio = (double)height / _height;

                for (int i = 1; i < _mipmaps.Length; ++i)
                {
                    Bitmap bmp = _mipmaps[i];
                    _mipmaps[i] = bmp.Resized((int)(bmp.Width * wratio), (int)(bmp.Height * wratio));
                }
            }

            _width = cubeExtent;
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
