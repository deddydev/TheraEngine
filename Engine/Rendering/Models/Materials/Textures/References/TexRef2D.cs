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
            _internalFormat = EPixelInternalFormat.Rgba8;
            _pixelFormat = EPixelFormat.Bgra;
            _pixelType = EPixelType.UnsignedByte;
        }
        public TexRef2D(string name, int width, int height,
            PixelFormat bitmapFormat, int mipCount = 1)
            : this(name, width, height)
        {
            _mipmaps = new GlobalFileRef<TextureFile2D>[mipCount];
            for (int i = 0, scale = 1; i < mipCount; scale = 1 << ++i)
                _mipmaps[i] = new TextureFile2D(width / scale, height / scale, bitmapFormat);
        }
        public TexRef2D(string name, int width, int height,
            EPixelInternalFormat internalFormat, EPixelFormat pixelFormat, EPixelType pixelType)
            : this(name, width, height)
        {
            _internalFormat = internalFormat;
            _pixelFormat = pixelFormat;
            _pixelType = pixelType;
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
                _mipmaps = new GlobalFileRef<TextureFile2D>[]
                {
                    new GlobalFileRef<TextureFile2D>(path)
                };
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
        
        private RenderTex2D _texture;

        [TSerialize("Width")]
        private int _width;
        [TSerialize("Height")]
        private int _height;
        
        private ETexWrapMode _uWrapMode = ETexWrapMode.Repeat;
        private ETexWrapMode _vWrapMode = ETexWrapMode.Repeat;
        private ETexMinFilter _minFilter = ETexMinFilter.LinearMipmapLinear;
        private ETexMagFilter _magFilter = ETexMagFilter.Linear;
        private float _lodBias = 0.0f;
        private EPixelInternalFormat _internalFormat = EPixelInternalFormat.Rgba;
        private EPixelFormat _pixelFormat = EPixelFormat.Rgba;
        private EPixelType _pixelType = EPixelType.UnsignedByte;
        
        [TSerialize]
        public ETexMagFilter MagFilter
        {
            get => _magFilter;
            set => _magFilter = value;
        }
        [TSerialize]
        public ETexMinFilter MinFilter
        {
            get => _minFilter;
            set => _minFilter = value;
        }
        [TSerialize]
        public ETexWrapMode UWrap
        {
            get => _uWrapMode;
            set => _uWrapMode = value;
        }
        [TSerialize]
        public ETexWrapMode VWrap
        {
            get => _uWrapMode;
            set => _uWrapMode = value;
        }
        [TSerialize]
        public float LodBias
        {
            get => _lodBias;
            set => _lodBias = value;
        }
        public int Width => _width;
        public int Height => _height;

        private void SetParameters()
        {
            if (_texture == null)
                return;

            _texture.Bind();

            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureLodBias, _lodBias);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureMagFilter, (int)_magFilter);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureMinFilter, (int)_minFilter);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureWrapS, (int)_uWrapMode);
            Engine.Renderer.TexParameter(ETexTarget.Texture2D, ETexParamName.TextureWrapT, (int)_vWrapMode);
            //AttachToFBO();
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
        
        public bool ResizingDisabled { get; internal set; }

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
            
            GetTexture(true)?.Resize(_width, _height);
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
                                case PixelFormat.Format32bppArgb:
                                case PixelFormat.Format32bppPArgb:
                                    _internalFormat = EPixelInternalFormat.Rgba8;
                                    _pixelFormat = EPixelFormat.Bgra;
                                    _pixelType = EPixelType.UnsignedByte;
                                    break;
                                case PixelFormat.Format24bppRgb:
                                    _internalFormat = EPixelInternalFormat.Rgb8;
                                    _pixelFormat = EPixelFormat.Bgr;
                                    _pixelType = EPixelType.UnsignedByte;
                                    break;
                                case PixelFormat.Format64bppArgb:
                                case PixelFormat.Format64bppPArgb:
                                    _internalFormat = EPixelInternalFormat.Rgba16;
                                    _pixelFormat = EPixelFormat.Bgra;
                                    _pixelType = EPixelType.UnsignedShort;
                                    break;
                            }
                        }
                    }

                }
            }
            CreateRenderTexture();
            _isLoading = false;
        }
        private void CreateRenderTexture()
        {
            if (_texture != null)
                _texture.PostPushData -= SetParameters;

            if (_mipmaps != null && _mipmaps.Length > 0)
                _texture = new RenderTex2D(_internalFormat, _pixelFormat, _pixelType, _mipmaps.SelectMany(x => x.File == null || x.File.Bitmaps == null ? new Bitmap[0] : x.File.Bitmaps).ToArray());
            else
                _texture = new RenderTex2D(_width, _height, _internalFormat, _pixelFormat, _pixelType);

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
                Bitmap bmp = new Bitmap(dim, dim, PixelFormat.Format32bppArgb);
                Graphics flagGraphics = Graphics.FromImage(bmp);
                flagGraphics.FillRectangle(Brushes.Red, 0, 0, squareExtent, squareExtent);
                flagGraphics.FillRectangle(Brushes.Red, squareExtent, squareExtent, squareExtent, squareExtent);
                flagGraphics.FillRectangle(Brushes.White, 0, squareExtent, squareExtent, squareExtent);
                flagGraphics.FillRectangle(Brushes.White, squareExtent, 0, squareExtent, squareExtent);
                return bmp;
            }
        }

        internal override void AttachToFBO()
        {
            if (FrameBufferAttachment.HasValue && Material != null && Material.HasAttachment(FrameBufferAttachment.Value))
                Engine.Renderer.AttachTextureToFrameBuffer(EFramebufferTarget.Framebuffer, FrameBufferAttachment.Value, ETexTarget.Texture2D, _texture.BindingId, 0);
        }
    }
}
