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
        public const string CategoryName = "Texture Reference 2D";

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
            {
                GlobalFileRef<TextureFile2D> tref = new TextureFile2D(width / scale, height / scale, bitmapFormat);
                tref.RegisterLoadEvent(OnMipLoaded);
                tref.RegisterUnloadEvent(OnMipUnloaded);
                _mipmaps[i] = tref;
            }

            DetermineTextureFormat();
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
            TextureFile2D tex = new TextureFile2D(width, height, bitmapFormat);
            GlobalFileRef<TextureFile2D> tref = tex;
            tref.RegisterLoadEvent(OnMipLoaded);
            tref.RegisterUnloadEvent(OnMipUnloaded);
            _mipmaps = new GlobalFileRef<TextureFile2D>[]  { tref };
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
                GlobalFileRef<TextureFile2D> tref = new GlobalFileRef<TextureFile2D>(path);
                tref.RegisterLoadEvent(OnMipLoaded);
                tref.RegisterUnloadEvent(OnMipUnloaded);
                _mipmaps[i] = tref;
            }
        }
        public TexRef2D(string name, params TextureFile2D[] mipmaps)
        {
            _name = name;
            _mipmaps = new GlobalFileRef<TextureFile2D>[mipmaps.Length];
            for (int i = 0; i < mipmaps.Length; ++i)
            {
                TextureFile2D mip = mipmaps[i];
                GlobalFileRef<TextureFile2D> tref = mip;
                tref.RegisterLoadEvent(OnMipLoaded);
                tref.RegisterUnloadEvent(OnMipUnloaded);
                _mipmaps[i] = tref;
            }
            DetermineTextureFormat();
        }
        #endregion

        //Note: one TextureData object may contain all the mips
        public GlobalFileRef<TextureFile2D>[] _mipmaps;

        [TSerialize]
        public GlobalFileRef<TextureFile2D>[] Mipmaps
        {
            get => _mipmaps;
            set
            {
                if (_mipmaps != null)
                {
                    foreach (var fileRef in Mipmaps)
                    {
                        fileRef.UnregisterLoadEvent(OnMipLoaded);
                        fileRef.UnregisterUnloadEvent(OnMipUnloaded);
                    }
                }
                _mipmaps = value;
                if (_mipmaps != null)
                {
                    foreach (var fileRef in Mipmaps)
                    {
                        fileRef.RegisterLoadEvent(OnMipLoaded);
                        fileRef.RegisterUnloadEvent(OnMipUnloaded);
                    }
                }
            }
        }
        
        private void OnMipLoaded(TextureFile2D tex)
        {
            //Engine.PrintLine("Mipmap loaded.");
            if (_texture != null && !_isLoading)
                LoadMipmaps();
        }
        private void OnMipUnloaded(TextureFile2D tex)
        {
            //Engine.PrintLine("Mipmap unloaded.");
            if (_texture != null && !_isLoading)
                LoadMipmaps();
        }

        protected RenderTex2D _texture;

        [TSerialize(nameof(Width))]
        protected int _width;
        [TSerialize(nameof(Height))]
        protected int _height;

        [TSerialize(nameof(PixelFormat))]
        private EPixelFormat _pixelFormat = EPixelFormat.Rgba;
        [TSerialize(nameof(PixelType))]
        private EPixelType _pixelType = EPixelType.UnsignedByte;
        [TSerialize(nameof(InternalFormat))]
        private EPixelInternalFormat _internalFormat = EPixelInternalFormat.Rgba8;
        
        [Category(CategoryName)]
        public EPixelFormat PixelFormat
        {
            get => _pixelFormat;
            set
            {
                _pixelFormat = value;
                if (_texture != null)
                {
                    _texture.PixelFormat = _pixelFormat;
                    _texture.PushData();
                }
            }
        }
        [Category(CategoryName)]
        public EPixelType PixelType
        {
            get => _pixelType;
            set
            {
                _pixelType = value;
                if (_texture != null)
                {
                    _texture.PixelType = _pixelType;
                    _texture.PushData();
                }
            }
        }
        [Category(CategoryName)]
        public EPixelInternalFormat InternalFormat
        {
            get => _internalFormat;
            set
            {
                _internalFormat = value;
                if (_texture != null)
                {
                    _texture.InternalFormat = _internalFormat;
                    _texture.PushData();
                }
            }
        }

        [Category(CategoryName)]
        [TSerialize]
        public EDepthStencilFmt DepthStencilFormat { get; set; } = EDepthStencilFmt.None;
        [Category(CategoryName)]
        [TSerialize]
        public ETexMagFilter MagFilter { get; set; } = ETexMagFilter.Nearest;
        [Category(CategoryName)]
        [TSerialize]
        public ETexMinFilter MinFilter { get; set; } = ETexMinFilter.Nearest;
        [Category(CategoryName)]
        [TSerialize]
        public ETexWrapMode UWrap { get; set; } = ETexWrapMode.Repeat;
        [Category(CategoryName)]
        [TSerialize]
        public ETexWrapMode VWrap { get; set; } = ETexWrapMode.Repeat;
        [Category(CategoryName)]
        [TSerialize]
        public float LodBias { get; set; } = 0.0f;

        [Category(CategoryName)]
        public int Width => _width;
        [Category(CategoryName)]
        public int Height => _height;

        protected virtual void SetParameters()
        {
            if (_texture == null)
                return;

            _texture.Bind();
            int dsmode = DepthStencilFormat == EDepthStencilFmt.Stencil ? 6401 : 6402;
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.DepthStencilTextureMode, dsmode);
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
                await Task.Run(LoadMipmaps);

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
                {
                    GetTextureAsync().ContinueWith(task => _texture = task.Result);
                }
            }

            return _texture;
        }

        public void UpdateTexture() => _texture?.PushData();

        public override BaseRenderTexture GetRenderTextureGeneric(bool loadSynchronously = false) => GetTexture(loadSynchronously);
        public override async Task<BaseRenderTexture> GetTextureGenericAsync() => await GetTextureAsync();

        /// <summary>
        /// If false, calling resize will do nothing.
        /// Useful for repeating textures that must always be a certain size or textures that never need to be dynamically resized during the game.
        /// False by default.
        /// </summary>
        public bool Resizeable { get; set; } = true;

        /// <summary>
        /// Resizes the textures stored in memory.
        /// Does nothing if Resizeable is false.
        /// </summary>
        public void Resize(int width, int height, bool resizeRenderTexture = true)
        {
            if (!Resizeable)
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

            if (resizeRenderTexture)
                _texture?.Resize(width, height);
        }
        /// <summary>
        /// Resizes the allocated render texture stored in video memory, if it exists.
        /// Does not resize the bitmaps stored in RAM.
        /// Does nothing if Resizeable is false.
        /// </summary>
        public void ResizeRenderTexture(int width, int height)
        {
            if (Resizeable)
                _texture?.Resize(width, height);
        }

        public bool IsLoaded => _texture != null;

        /// <summary>
        /// Call if you want to load all mipmap texture files, in a background thread for example.
        /// </summary>
        public void LoadMipmaps()
        {
            _isLoading = true;
            _mipmaps?.ForEach(tex => tex?.GetInstance());
            DetermineTextureFormat(false);
            CreateRenderTexture();
            _isLoading = false;
        }
        /// <summary>
        /// Decides the best internal format, pixel format, and pixel type for the stored mipmaps.
        /// </summary>
        /// <param name="force">If true, sets the formats/type even if the mipmaps are loaded.</param>
        public void DetermineTextureFormat(bool force = true)
        {
            if (_mipmaps != null && _mipmaps.Length > 0)
            {
                var tref = _mipmaps[0];
                //if (!tref.IsLoaded && !force)
                //    return;
                var t = tref.File;
                if (t != null && t.Bitmaps.Length > 0)
                {
                    var b = t.Bitmaps[0];
                    if (b != null)
                    {
                        switch (b.PixelFormat)
                        {
                            case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                                InternalFormat = EPixelInternalFormat.Rgb8;
                                PixelFormat = EPixelFormat.Bgr;
                                PixelType = EPixelType.UnsignedByte;
                                break;
                            case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                                InternalFormat = EPixelInternalFormat.Rgb8;
                                PixelFormat = EPixelFormat.Bgra;
                                PixelType = EPixelType.UnsignedByte;
                                break;
                            case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                            case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                                InternalFormat = EPixelInternalFormat.Rgba8;
                                PixelFormat = EPixelFormat.Bgra;
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
        protected virtual void CreateRenderTexture()
        {
            if (_texture != null)
            {
                _texture.PostPushData -= SetParameters;
                _texture.Destroy();
            }

            if (_mipmaps != null && _mipmaps.Length > 0)
                _texture = new RenderTex2D(InternalFormat, PixelFormat, PixelType,
                    _mipmaps.SelectMany(x => x.File == null || x.File.Bitmaps == null ? new Bitmap[0] : x.File.Bitmaps).ToArray())
                {
                    Resizable = Resizeable,
                };
            else
                _texture = new RenderTex2D(_width, _height, InternalFormat, PixelFormat, PixelType)
                {
                    Resizable = Resizeable
                };

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

        public override void AttachToFBO(EFramebufferTarget target, int mipLevel = 0)
        {
            if (FrameBufferAttachment.HasValue)
                AttachToFBO(target, FrameBufferAttachment.Value, mipLevel);
        }
        public override void DetachFromFBO(EFramebufferTarget target, int mipLevel = 0)
        {
            if (FrameBufferAttachment.HasValue)
                Engine.Renderer.AttachTextureToFrameBuffer(target, FrameBufferAttachment.Value, ETexTarget.Texture2D, 0, mipLevel);
        }
        public override void AttachToFBO(EFramebufferTarget target, EFramebufferAttachment attachment, int mipLevel = 0)
            => Engine.Renderer.AttachTextureToFrameBuffer(target, attachment, ETexTarget.Texture2D, _texture.BindingId, mipLevel);
        public override void DetachFromFBO(EFramebufferTarget target, EFramebufferAttachment attachment, int mipLevel = 0)
            => Engine.Renderer.AttachTextureToFrameBuffer(target, attachment, ETexTarget.Texture2D, 0, mipLevel);
        
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
