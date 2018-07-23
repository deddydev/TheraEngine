using TheraEngine.Files;
using System.ComponentModel;
using System.Threading.Tasks;
using System;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Rendering.Models.Materials
{
    [FileExt("tref3d")]
    [FileDef("3D Texture Reference")]
    public class TexRef3D : BaseTexRef
    {
        public const string CategoryName = "Texture Reference 2D";

        #region Constructors
        public TexRef3D() : this(null, 1, 1, 1) { }
        public TexRef3D(string name, int width, int height, int depth)
        {
            _mipmaps = null;
            _name = name;
            _width = width;
            _height = height;
            _internalFormat = EPixelInternalFormat.Rgba8;
            _pixelFormat = EPixelFormat.Bgra;
            _pixelType = EPixelType.UnsignedByte;
        }
        public TexRef3D(string name, int width, int height, int depth,
            ETPixelCompFmt bitmapFormat = ETPixelCompFmt.F16, int mipCount = 1)
            : this(name, width, height, depth)
        {
            _mipmaps = new GlobalFileRef<TBitmap3D>[mipCount];
            for (int i = 0, scale = 1; i < mipCount; scale = 1 << ++i)
            {
                GlobalFileRef<TBitmap3D> tref = new TBitmap3D(width / scale, height / scale, depth / scale, ETPixelType.Basic);
                tref.RegisterLoadEvent(OnMipLoaded);
                tref.RegisterUnloadEvent(OnMipUnloaded);
                _mipmaps[i] = tref;
            }

            DetermineTextureFormat();
        }
        public TexRef3D(string name, int width, int height, int depth,
            EPixelInternalFormat internalFormat, EPixelFormat pixelFormat, EPixelType pixelType)
            : this(name, width, height, depth)
        {
            _mipmaps = null;
            _internalFormat = internalFormat;
            _pixelFormat = pixelFormat;
            _pixelType = pixelType;
            _width = width;
            _height = height;
        }
        //public TexRef3D(string name, int width, int height, int depth,
        //    EPixelInternalFormat internalFormat, EPixelFormat pixelFormat, EPixelType pixelType)
        //    : this(name, width, height, depth, internalFormat, pixelFormat, pixelType)
        //{
        //    _mipmaps = new GlobalFileRef<TBitmap3D>[] { new TBitmap3D(width, height, depth, pixelType) };
        //}
        public TexRef3D(string name, params string[] mipMapPaths)
        {
            _name = name;
            _mipmaps = new GlobalFileRef<TBitmap3D>[mipMapPaths.Length];
            for (int i = 0; i < mipMapPaths.Length; ++i)
            {
                string path = mipMapPaths[i];
                if (path.StartsWith("file://"))
                    path = path.Substring(7);
                _mipmaps = new GlobalFileRef<TBitmap3D>[]
                {
                    new GlobalFileRef<TBitmap3D>(path)
                };
            }
        }
        #endregion

        //Note: one TextureData object may contain all the mips
        public GlobalFileRef<TBitmap3D>[] _mipmaps;

        [Browsable(false)]
        [Category(CategoryName)]
        [TSerialize]
        public GlobalFileRef<TBitmap3D>[] Mipmaps
        {
            get => _mipmaps;
            set
            {
                if (_mipmaps != null)
                {
                    for (int i = 0; i < Mipmaps.Length; ++i)
                    {
                        var fileRef = Mipmaps[i];
                        if (fileRef == null)
                            Mipmaps[i] = new GlobalFileRef<TBitmap3D>();
                        {
                            fileRef.UnregisterLoadEvent(OnMipLoaded);
                            fileRef.UnregisterUnloadEvent(OnMipUnloaded);
                        }
                    }
                }
                _mipmaps = value;
                if (_mipmaps != null)
                {
                    foreach (var fileRef in Mipmaps)
                    {
                        if (fileRef != null)
                        {
                            fileRef.RegisterLoadEvent(OnMipLoaded);
                            fileRef.RegisterUnloadEvent(OnMipUnloaded);
                        }
                    }
                }
            }
        }

        private void OnMipLoaded(TBitmap3D tex)
        {
            //Engine.PrintLine("Mipmap loaded.");
            if (_texture != null && !_isLoading)
                LoadMipmaps();
        }
        private void OnMipUnloaded(TBitmap3D tex)
        {
            //Engine.PrintLine("Mipmap unloaded.");
            if (_texture != null && !_isLoading)
                LoadMipmaps();
        }

        protected RenderTex3D _texture;

        [TSerialize(nameof(Width))]
        protected int _width;
        [TSerialize(nameof(Height))]
        protected int _height;
        [TSerialize(nameof(Depth))]
        protected int _depth;

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

        /// <summary>
        /// If false, calling resize will do nothing.
        /// Useful for repeating textures that must always be a certain size or textures that never need to be dynamically resized during the game.
        /// False by default.
        /// </summary>
        [Category(CategoryName)]
        public bool Resizable { get; set; } = true;

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
        public ETexWrapMode WWrap { get; set; } = ETexWrapMode.Repeat;
        [Category(CategoryName)]
        [TSerialize]
        public float LodBias { get; set; } = 0.0f;

        [Category(CategoryName)]
        public int Width => _width;
        [Category(CategoryName)]
        public int Height => _height;
        [Category(CategoryName)]
        public int Depth => _depth;

        private void SetParameters()
        {
            if (_texture == null)
                return;

            _texture.Bind();

            Engine.Renderer.TexParameter(ETexTarget.Texture3D, ETexParamName.TextureLodBias, LodBias);
            Engine.Renderer.TexParameter(ETexTarget.Texture3D, ETexParamName.TextureMagFilter, (int)MagFilter);
            Engine.Renderer.TexParameter(ETexTarget.Texture3D, ETexParamName.TextureMinFilter, (int)MinFilter);
            Engine.Renderer.TexParameter(ETexTarget.Texture3D, ETexParamName.TextureWrapS, (int)UWrap);
            Engine.Renderer.TexParameter(ETexTarget.Texture3D, ETexParamName.TextureWrapT, (int)VWrap);
            Engine.Renderer.TexParameter(ETexTarget.Texture3D, ETexParamName.TextureWrapR, (int)WWrap);
        }

        private bool _isLoading = false;
        public async Task<RenderTex3D> GetTextureAsync()
        {
            if (_texture != null)
                return _texture;

            if (!_isLoading)
                await Task.Run(LoadMipmaps);

            return _texture;
        }
        public RenderTex3D GetTexture(bool loadSynchronously = false)
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
        /// Resizes the textures stored in memory.
        /// Does nothing if Resizeable is false.
        /// </summary>
        public void Resize(int width, int height, int depth, bool resizeRenderTexture = true)
        {
            if (!Resizable)
                return;

            _width = width;
            _height = height;
            _depth = depth;

            if (_isLoading)
                return;

            _mipmaps?.ForEach(x => x.File?.Resize(width, height, depth));

            if (resizeRenderTexture)
                _texture?.Resize(width, height, depth);
        }
        /// <summary>
        /// Resizes the allocated render texture stored in video memory, if it exists.
        /// Does not resize the bitmaps stored in RAM.
        /// Does nothing if Resizeable is false.
        /// </summary>
        public void ResizeRenderTexture(int width, int height, int depth, bool doNotLoad = false)
        {
            if (!Resizable)
                return;

            _width = width;
            _height = height;
            _depth = depth;

            if (_isLoading)
                return;

            if (doNotLoad && _texture == null)
                return;

            RenderTex3D t = GetTexture(true);
            t?.Resize(_width, _height, _depth);
        }
        
        [Browsable(false)]
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
                if (t != null)
                {
                    //switch (t.Format)
                    //{
                    //    case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    //        InternalFormat = EPixelInternalFormat.Rgb8;
                    //        PixelFormat = EPixelFormat.Bgr;
                    //        PixelType = EPixelType.UnsignedByte;
                    //        break;
                    //    case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    //        InternalFormat = EPixelInternalFormat.Rgb8;
                    //        PixelFormat = EPixelFormat.Bgra;
                    //        PixelType = EPixelType.UnsignedByte;
                    //        break;
                    //    case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    //    case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    //        InternalFormat = EPixelInternalFormat.Rgba8;
                    //        PixelFormat = EPixelFormat.Bgra;
                    //        PixelType = EPixelType.UnsignedByte;
                    //        break;
                    //    case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                    //    case System.Drawing.Imaging.PixelFormat.Format64bppPArgb:
                    //        InternalFormat = EPixelInternalFormat.Rgba16;
                    //        PixelFormat = EPixelFormat.Bgra;
                    //        PixelType = EPixelType.UnsignedShort;
                    //        break;
                    //}
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
                _texture = new RenderTex3D(InternalFormat, PixelFormat, PixelType,
                    _mipmaps.SelectMany(x => x.File == null || x.File.Bitmaps == null ? new Bitmap[0] : x.File.Bitmaps).ToArray())
                {
                    Resizable = Resizable,
                };
            else
                _texture = new RenderTex3D(_width, _height, InternalFormat, PixelFormat, PixelType)
                {
                    Resizable = Resizable
                };

            _texture.PostPushData += SetParameters;
        }
        
        public override void AttachToFBO(EFramebufferTarget target, int mipLevel = 0)
        {
            if (FrameBufferAttachment.HasValue)
                AttachToFBO(target, FrameBufferAttachment.Value, mipLevel);
        }
        public override void DetachFromFBO(EFramebufferTarget target, int mipLevel = 0)
        {
            if (FrameBufferAttachment.HasValue)
                Engine.Renderer.AttachTextureToFrameBuffer(EFramebufferTarget.Framebuffer, FrameBufferAttachment.Value, 0, mipLevel);
        }
        public override void AttachToFBO(EFramebufferTarget target, EFramebufferAttachment attachment, int mipLevel = 0)
            => Engine.Renderer.AttachTextureToFrameBuffer(EFramebufferTarget.Framebuffer, attachment, _texture.BindingId, mipLevel);
        public override void DetachFromFBO(EFramebufferTarget target, EFramebufferAttachment attachment, int mipLevel = 0)
            => Engine.Renderer.AttachTextureToFrameBuffer(target, attachment, ETexTarget.Texture3D, 0, mipLevel);
    }
}
