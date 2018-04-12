using TheraEngine.Files;
using TheraEngine.Rendering.Textures;
using System.Drawing.Imaging;
using System.Drawing;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System;
using TheraEngine.Rendering.Models.Materials.Textures;
using System.IO;

namespace TheraEngine.Rendering.Models.Materials
{
    public enum EDepthStencilFmt
    {
        None,
        Depth,
        Stencil,
    }
    [FileExt("tref2d")]
    [FileDef("2D Texture Reference")]
    public class TexRef2D : BaseTexRef
    {
        #region Constructors
        public TexRef2D() : this(null, 1, 1) { }
        public TexRef2D(string name, int width, int height)
        {
            _mipmaps = null;
            _name = name;
            _width = width;
            _height = height;
            InternalFormat = EPixelInternalFormat.Rgba8;
            PixelFormat = EPixelFormat.Bgra;
            PixelType = EPixelType.UnsignedByte;
        }
        public TexRef2D(string name, int width, int height,
            PixelFormat bitmapFormat, int mipCount = 1)
            : this(name, width, height)
        {
            _mipmaps = new GlobalFileRef<TextureFile2D>[mipCount];
            for (int i = 0, scale = 1; i < mipCount; scale = 1 << ++i)
                _mipmaps[i] = new TextureFile2D(width / scale, height / scale, bitmapFormat);

            switch (bitmapFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    InternalFormat = EPixelInternalFormat.Rgba8;
                    PixelFormat = EPixelFormat.Bgra;
                    PixelType = EPixelType.UnsignedByte;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    InternalFormat = EPixelInternalFormat.Rgb8;
                    PixelFormat = EPixelFormat.Bgr;
                    PixelType = EPixelType.UnsignedByte;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format64bppPArgb:
                    InternalFormat = EPixelInternalFormat.Rgba16;
                    PixelFormat = EPixelFormat.Bgra;
                    PixelType = EPixelType.UnsignedShort;
                    break;
            }
        }
        public TexRef2D(string name, int width, int height,
            EPixelInternalFormat internalFormat, EPixelFormat pixelFormat, EPixelType pixelType)
            : this(name, width, height)
        {
            InternalFormat = internalFormat;
            PixelFormat = pixelFormat;
            PixelType = pixelType;
        }
        public TexRef2D(string name, int width, int height,
            EPixelInternalFormat internalFormat, EPixelFormat pixelFormat, EPixelType pixelType, PixelFormat bitmapFormat)
            : this(name, width, height, internalFormat, pixelFormat, pixelType)
        {
            _mipmaps = new GlobalFileRef<TextureFile2D>[] { new TextureFile2D(width, height, bitmapFormat) };
        }
        public TexRef2D(string name, params string[] mipMapPaths)
        {
            _name = name;
            _mipmaps = new GlobalFileRef<TextureFile2D>[mipMapPaths.Length];
            for (int i = 0; i < mipMapPaths.Length; ++i)
            {
                string path = mipMapPaths[i];
                if (path.StartsWith("file://"))
                    path = path.Substring(7);
                _mipmaps[i] = new GlobalFileRef<TextureFile2D>(path);
            }
        }
        public TexRef2D(string name, params TextureFile2D[] mipmaps)
        {
            _name = name;
            _mipmaps = new GlobalFileRef<TextureFile2D>[mipmaps.Length];
            for (int i = 0; i < mipmaps.Length; ++i)
            {
                TextureFile2D mip = mipmaps[i];
                _mipmaps[i] = new GlobalFileRef<TextureFile2D>(mip);
            }
        }
        #endregion

        //Note: one TextureData object may contain all the mips
        public GlobalFileRef<TextureFile2D>[] _mipmaps;

        [TSerialize]
        public GlobalFileRef<TextureFile2D>[] Mipmaps
        {
            get => _mipmaps;
            set => _mipmaps = value;
        }
        
        protected RenderTex2D _texture;

        [TSerialize(nameof(Width))]
        protected int _width;
        [TSerialize(nameof(Height))]
        protected int _height;

        [TSerialize]
        public EPixelFormat PixelFormat { get; set; } = EPixelFormat.Rgba;
        [TSerialize]
        public EPixelType PixelType { get; set; } = EPixelType.UnsignedByte;
        [TSerialize]
        public EDepthStencilFmt DepthStencilFormat { get; set; } = EDepthStencilFmt.None;
        [TSerialize]
        public EPixelInternalFormat InternalFormat { get; set; } = EPixelInternalFormat.Rgba;
        [TSerialize]
        public ETexMagFilter MagFilter { get; set; } = ETexMagFilter.Nearest;
        [TSerialize]
        public ETexMinFilter MinFilter { get; set; } = ETexMinFilter.Nearest;
        [TSerialize]
        public ETexWrapMode UWrap { get; set; } = ETexWrapMode.Repeat;
        [TSerialize]
        public ETexWrapMode VWrap { get; set; } = ETexWrapMode.Repeat;
        [TSerialize]
        public float LodBias { get; set; } = 0.0f;

        public int Width => _width;
        public int Height => _height;

        protected virtual void SetParameters()
        {
            if (_texture == null)
                return;

            _texture.Bind();

            if (DepthStencilFormat != EDepthStencilFmt.None)
            {
                int u = DepthStencilFormat == EDepthStencilFmt.Stencil ? 
                    (int)OpenTK.Graphics.OpenGL.All.StencilIndex :
                    (int)OpenTK.Graphics.OpenGL.All.DepthComponent;
                int id = _texture.BindingId;
                OpenTK.Graphics.OpenGL.GL.TextureParameterI(id, OpenTK.Graphics.OpenGL.All.DepthStencilTextureMode, ref u);
            }
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureLodBias, LodBias);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureMagFilter, (int)MagFilter);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureMinFilter, (int)MinFilter);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureWrapS, (int)UWrap);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureWrapT, (int)VWrap);
        }

        private bool _isLoading = false;
        public async Task<RenderTex2D> GetTextureAsync()
        {
            if (_texture != null)
                return _texture;

            if (!_isLoading)
                await Task.Run((Action)LoadMipmaps);

            return _texture;
        }
        public RenderTex2D GetTexture(bool loadSynchronously = false)
        {
            if (_texture != null)
                return _texture;

            if (!_isLoading)
            {
                if (loadSynchronously)
                {
                    LoadMipmaps();
                    return _texture;
                }
                else
                    GetTextureAsync().ContinueWith(task => _texture = task.Result);
            }

            return _texture;
        }

        public override BaseRenderTexture GetTextureGeneric(bool loadSynchronously = false) => GetTexture(loadSynchronously);
        public override async Task<BaseRenderTexture> GetTextureGenericAsync() => await GetTextureAsync();
        
        /// <summary>
        /// If true, calling resize will do nothing.
        /// Useful for repeating textures that must be a certain size.
        /// </summary>
        public bool ResizingDisabled { get; set; }

        /// <summary>
        /// Resizes the textures stored in memory.
        /// </summary>
        public void Resize(int width, int height)
        {
            if (ResizingDisabled)
                return;

            _width = width;
            _height = height;

            if (_isLoading)
                return;

            if (_mipmaps != null)
                foreach (TextureFile2D tex in _mipmaps)
                {
                    int w = width, h = height;
                    for (int i = 0; i < tex.Bitmaps.Length; ++i)
                    {
                        tex.Bitmaps[i] = tex.Bitmaps[i].Resized(w, h);
                        w /= 2;
                        h /= 2;
                    }
                }

            _texture?.Resize(width, height);
        }

        public bool IsLoaded => _texture != null;

        /// <summary>
        /// Call if you want to load all mipmap texture files, in a background thread for example.
        /// </summary>
        public void LoadMipmaps()
        {
            _isLoading = true;
            if (_mipmaps != null)
            {
                if (_mipmaps.Length > 0)
                {
                    var tref = _mipmaps[0];
                    var t = tref.File;
                    if (t != null && t.Bitmaps.Length > 0)
                    {
                        var b = t.Bitmaps[0];
                        if (b != null)
                        {
                            switch (b.PixelFormat)
                            {
                                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                                    InternalFormat = EPixelInternalFormat.Rgba8;
                                    PixelFormat = EPixelFormat.Bgra;
                                    PixelType = EPixelType.UnsignedByte;
                                    break;
                                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                                    InternalFormat = EPixelInternalFormat.Rgb8;
                                    PixelFormat = EPixelFormat.Bgr;
                                    PixelType = EPixelType.UnsignedByte;
                                    break;
                                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                                case System.Drawing.Imaging.PixelFormat.Format64bppPArgb:
                                    InternalFormat = EPixelInternalFormat.Rgba16;
                                    PixelFormat = EPixelFormat.Bgra;
                                    PixelType = EPixelType.UnsignedShort;
                                    break;
                            }
                        }
                    }

                }
            }
            CreateRenderTexture();
            _isLoading = false;
        }
        protected virtual void CreateRenderTexture()
        {
            if (_texture != null)
                _texture.PostPushData -= SetParameters;

            if (_mipmaps != null && _mipmaps.Length > 0)
                _texture = new RenderTex2D(InternalFormat, PixelFormat, PixelType, _mipmaps.SelectMany(x => x.File == null || x.File.Bitmaps == null ? new Bitmap[0] : x.File.Bitmaps).ToArray());
            else
                _texture = new RenderTex2D(_width, _height, InternalFormat, PixelFormat, PixelType);

            _texture.PostPushData += SetParameters;
        }

        public static Bitmap FillerBitmap => _fillerBitmap.Value;

        private static Lazy<Bitmap> _fillerBitmap = new Lazy<Bitmap>(() => GetFillerBitmap());
        private unsafe static Bitmap GetFillerBitmap()
        {
            TextureFile2D tex = new TextureFile2D(Path.Combine(Engine.Settings.TexturesFolder, "Filler.png"));
            if (tex?.Bitmaps != null && tex.Bitmaps.Length > 0 && tex.Bitmaps[0] != null)
                return tex.Bitmaps[0];
            else
            {
                int squareExtent = 4;
                int dim = squareExtent * 2;
                Bitmap bmp = new Bitmap(dim, dim, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics flagGraphics = Graphics.FromImage(bmp);
                flagGraphics.FillRectangle(Brushes.Red, 0, 0, squareExtent, squareExtent);
                flagGraphics.FillRectangle(Brushes.Red, squareExtent, squareExtent, squareExtent, squareExtent);
                flagGraphics.FillRectangle(Brushes.White, 0, squareExtent, squareExtent, squareExtent);
                flagGraphics.FillRectangle(Brushes.White, squareExtent, 0, squareExtent, squareExtent);
                return bmp;
            }
        }

        internal override void AttachToFBO(int mipLevel = 0)
        {
            if (FrameBufferAttachment.HasValue)
                AttachToFBO(FrameBufferAttachment.Value, mipLevel);
        }
        public override void AttachToFBO(EFramebufferAttachment attachment, int mipLevel = 0)
        {
            Engine.Renderer.AttachTextureToFrameBuffer(EFramebufferTarget.Framebuffer, attachment, ETexTarget.Texture2D, _texture.BindingId, mipLevel);
        }
        internal override void DetachFromFBO(int mipLevel = 0)
        {
            if (FrameBufferAttachment.HasValue)
                Engine.Renderer.AttachTextureToFrameBuffer(EFramebufferTarget.Framebuffer, FrameBufferAttachment.Value, ETexTarget.Texture2D, 0, mipLevel);
        }

        public static TexRef2D CreateFrameBufferTexture(string name, int width, int height,
            EPixelInternalFormat internalFmt, EPixelFormat fmt, EPixelType pixelType, EFramebufferAttachment bufAttach)
        {
            return new TexRef2D(name, width, height, internalFmt, fmt, pixelType)
            {
                MinFilter = ETexMinFilter.Linear,
                MagFilter = ETexMagFilter.Linear,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
                FrameBufferAttachment = bufAttach,
            };
        }
        public static TexRef2D CreateFrameBufferTexture(string name, int width, int height,
            EPixelInternalFormat internalFmt, EPixelFormat fmt, EPixelType pixelType)
        {
            return new TexRef2D(name, width, height, internalFmt, fmt, pixelType)
            {
                MinFilter = ETexMinFilter.Linear,
                MagFilter = ETexMagFilter.Linear,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
            };
        }
    }
}
