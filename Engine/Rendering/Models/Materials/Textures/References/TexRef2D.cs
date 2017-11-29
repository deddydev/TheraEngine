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
    [FileClass("TREF2D", "2D Texture Reference")]
    public class TextureReference2D : BaseTextureReference
    {
        #region Constructors
        public TextureReference2D() : this(null, 1, 1) { }
        public TextureReference2D(string name, int width, int height)
        {
            _mipmaps = null;
            _name = name;
            _width = width;
            _height = height;
            _internalFormat = EPixelInternalFormat.Rgba8;
            _pixelFormat = EPixelFormat.Bgra;
            _pixelType = EPixelType.UnsignedByte;
        }
        public TextureReference2D(string name, int width, int height,
            PixelFormat bitmapFormat = PixelFormat.Format32bppArgb, int mipCount = 1)
            : this(name, width, height)
        {
            _mipmaps = new SingleFileRef<TextureFile2D>[mipCount];
            for (int i = 0, scale = 1; i < mipCount; scale = 1 << ++i)
                _mipmaps[i] = new TextureFile2D(width / scale, height / scale, bitmapFormat);
        }
        public TextureReference2D(string name, int width, int height,
            EPixelInternalFormat internalFormat, EPixelFormat pixelFormat, EPixelType pixelType)
            : this(name, width, height)
        {
            _mipmaps = null;
            _internalFormat = internalFormat;
            _pixelFormat = pixelFormat;
            _pixelType = pixelType;
            _width = width;
            _height = height;
        }
        public TextureReference2D(string name, int width, int height,
            EPixelInternalFormat internalFormat, EPixelFormat pixelFormat, EPixelType pixelType, PixelFormat bitmapFormat)
            : this(name, width, height, internalFormat, pixelFormat, pixelType)
        {
            _mipmaps = new SingleFileRef<TextureFile2D>[] { new TextureFile2D(width, height, bitmapFormat) };
        }
        public TextureReference2D(string name, params string[] mipMapPaths)
        {
            _name = name;
            _mipmaps = new SingleFileRef<TextureFile2D>[mipMapPaths.Length];
            for (int i = 0; i < mipMapPaths.Length; ++i)
            {
                string path = mipMapPaths[i];
                if (path.StartsWith("file://"))
                    path = path.Substring(7);
                _mipmaps = new SingleFileRef<TextureFile2D>[]
                {
                    new SingleFileRef<TextureFile2D>(path)
                };
            }
        }
        #endregion

        //Note: one TextureData object may contain all the mips
        public SingleFileRef<TextureFile2D>[] _mipmaps;

        [TSerialize]
        public SingleFileRef<TextureFile2D>[] Mipmaps
        {
            get => _mipmaps;
            set => _mipmaps = value;
        }
        
        private Texture2D _texture;

        [TSerialize("Width")]
        private int _width;
        [TSerialize("Height")]
        private int _height;

        private int _index;
        private ETexWrapMode _uWrapMode = ETexWrapMode.Repeat;
        private ETexWrapMode _vWrapMode = ETexWrapMode.Repeat;
        private ETexMinFilter _minFilter = ETexMinFilter.LinearMipmapLinear;
        private ETexMagFilter _magFilter = ETexMagFilter.Linear;
        private float _lodBias = 0.0f;
        private EPixelInternalFormat _internalFormat;
        private EPixelFormat _pixelFormat;
        private EPixelType _pixelType;
        private EFramebufferAttachment? _frameBufferAttachment;

        [TSerialize]
        public EFramebufferAttachment? FrameBufferAttachment
        {
            get => _frameBufferAttachment;
            set => _frameBufferAttachment = value;
        }
        [TSerialize]
        public int Index
        {
            get => _index;
            set => _index = value;
        }
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
            if (_frameBufferAttachment.HasValue && Material != null && Material.HasAttachment(_frameBufferAttachment.Value))
                Engine.Renderer.AttachTextureToFrameBuffer(EFramebufferTarget.Framebuffer, _frameBufferAttachment.Value, ETexTarget.Texture2D, _texture.BindingId, 0);
        }

        private bool _isLoading = false;
        public async Task<Texture2D> GetTextureAsync()
        {
            if (_texture != null || _isLoading)
                return _texture;

            await Task.Run((Action)LoadMipmaps);
            FinalizeTextureLoaded();

            return _texture;
        }
        public Texture2D GetTexture()
        {
            if (_texture != null || _isLoading)
                return _texture;

            //Texture is null. Load it asynchronously
            GetTextureAsync().ContinueWith(task => TextureLoaded(task));
            
            //Return filler texture
            return GetFillerTexture();
        }

        private static Texture2D _fillerTexture;
        private static Texture2D GetFillerTexture()
        {
            if (_fillerTexture == null)
            {
                TextureFile2D tex = new TextureFile2D(Path.Combine(Engine.Settings.TexturesFolder, "Filler.png"));
                Bitmap b = tex.Bitmaps[0];
                EPixelInternalFormat internalFormat = EPixelInternalFormat.Rgb8;
                EPixelFormat format = EPixelFormat.Rgb;
                EPixelType type = EPixelType.Byte;
                switch (b.PixelFormat)
                {
                    case PixelFormat.Format32bppArgb:
                    case PixelFormat.Format32bppPArgb:
                        internalFormat = EPixelInternalFormat.Rgba8;
                        format = EPixelFormat.Bgra;
                        type = EPixelType.UnsignedByte;
                        break;
                    case PixelFormat.Format24bppRgb:
                        internalFormat = EPixelInternalFormat.Rgb8;
                        format = EPixelFormat.Bgr;
                        type = EPixelType.UnsignedByte;
                        break;
                    case PixelFormat.Format64bppArgb:
                    case PixelFormat.Format64bppPArgb:
                        internalFormat = EPixelInternalFormat.Rgba16;
                        format = EPixelFormat.Bgra;
                        type = EPixelType.UnsignedShort;
                        break;
                }
                _fillerTexture = new Texture2D(b.Width, b.Height, internalFormat, format, type);
            }
            return _fillerTexture;

        }

        private void TextureLoaded(Task<Texture2D> task)
        {
            _texture = task.Result;
        }

        public override BaseRenderTexture GetTextureGeneric() => GetTexture();
        public override async Task<BaseRenderTexture> GetTextureGenericAsync() => await GetTextureAsync();

        private void FinalizeTextureLoaded()
        {
            if (_mipmaps != null && _mipmaps.Length > 0)
                _texture = new Texture2D(_internalFormat, _pixelFormat, _pixelType, _mipmaps.SelectMany(x => x.File == null || x.File.Bitmaps == null ? new Bitmap[0] : x.File.Bitmaps).ToArray());
            else
                _texture = new Texture2D(_width, _height, _internalFormat, _pixelFormat, _pixelType);

            _texture.PostPushData += SetParameters;
        }

        public Material Material { get; internal set; }
        public bool DoNotResize { get; internal set; }

        /// <summary>
        /// Resizes the textures stored in memory.
        /// </summary>
        public async void Resize(int width, int height)
        {
            if (DoNotResize)
                return;

            _width = width;
            _height = height;

            if (_isLoading)
                return;

            Texture2D t = await GetTextureAsync();
            t?.Resize(_width, _height);
        }

        /// <summary>
        /// Call if you want to load all mipmap texture files, in a background thread for example.
        /// </summary>
        public void LoadMipmaps()
        {
            if (_mipmaps == null)
                return;
            _isLoading = true;
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
            _isLoading = false;
        }
    }
}
