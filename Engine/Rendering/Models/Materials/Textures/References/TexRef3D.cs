﻿using TheraEngine.Files;
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
                _mipmaps[i] = new TBitmap3D(width / scale, height / scale, depth / scale, ETPixelType.Basic);
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

        [TSerialize]
        public GlobalFileRef<TBitmap3D>[] Mipmaps
        {
            get => _mipmaps;
            set => _mipmaps = value;
        }
        
        private RenderTex3D _texture;

        [TSerialize("Width")]
        private int _width;
        [TSerialize("Height")]
        private int _height;
        [TSerialize("Depth")]
        private int _depth;

        private int _index;
        private ETexWrapMode _uWrapMode = ETexWrapMode.Repeat;
        private ETexWrapMode _vWrapMode = ETexWrapMode.Repeat;
        private ETexWrapMode _wWrapMode = ETexWrapMode.Repeat;
        private ETexMinFilter _minFilter = ETexMinFilter.LinearMipmapLinear;
        private ETexMagFilter _magFilter = ETexMagFilter.Linear;
        private float _lodBias = 0.0f;
        private EPixelInternalFormat _internalFormat;
        private EPixelFormat _pixelFormat;
        private EPixelType _pixelType;
        
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
        public ETexWrapMode WWrap
        {
            get => _wWrapMode;
            set => _wWrapMode = value;
        }
        [TSerialize]
        public float LodBias
        {
            get => _lodBias;
            set => _lodBias = value;
        }

        public int Width => _width;
        public int Height => _height;
        public int Depth => _depth;

        private void SetParameters()
        {
            if (_texture == null)
                return;

            _texture.Bind();

            Engine.Renderer.TexParameter(ETexTarget.Texture3D, ETexParamName.TextureLodBias, _lodBias);
            Engine.Renderer.TexParameter(ETexTarget.Texture3D, ETexParamName.TextureMagFilter, (int)_magFilter);
            Engine.Renderer.TexParameter(ETexTarget.Texture3D, ETexParamName.TextureMinFilter, (int)_minFilter);
            Engine.Renderer.TexParameter(ETexTarget.Texture3D, ETexParamName.TextureWrapS, (int)_uWrapMode);
            Engine.Renderer.TexParameter(ETexTarget.Texture3D, ETexParamName.TextureWrapT, (int)_vWrapMode);
            Engine.Renderer.TexParameter(ETexTarget.Texture3D, ETexParamName.TextureWrapR, (int)_wWrapMode);
        }

        private bool _isLoading = false;
        public async Task<RenderTex3D> GetTextureAsync()
        {
            if (_texture != null || _isLoading)
                return _texture;

            await Task.Run((Action)LoadMipmaps);
            FinalizeTextureLoaded();

            return _texture;
        }
        public RenderTex3D GetTexture(bool loadSynchronously = false)
        {
            if (_texture != null || _isLoading)
                return _texture;

            LoadMipmaps();
            FinalizeTextureLoaded();

            return _texture;
        }

        public override BaseRenderTexture GetTextureGeneric(bool loadSynchronously = false) => GetTexture(loadSynchronously);
        public override async Task<BaseRenderTexture> GetTextureGenericAsync() => await GetTextureAsync();

        private void FinalizeTextureLoaded()
        {
            //if (_mipmaps != null && _mipmaps.Length > 0)
            //    _texture = new Texture3D(_internalFormat, _pixelFormat, _pixelType, _mipmaps.SelectMany(x => x.File == null || x.File.Bitmaps == null ? new Bitmap[0] : x.File.Bitmaps).ToArray());
            //else
            //    _texture = new Texture3D(_width, _height, _depth, _internalFormat, _pixelFormat, _pixelType);

            _texture.PostPushData += SetParameters;
        }
        
        public bool DoNotResize { get; internal set; }

        /// <summary>
        /// Resizes the textures stored in memory.
        /// </summary>
        public void Resize(int width, int height, int depth)
        {
            if (DoNotResize)
                return;

            _width = width;
            _height = height;
            _depth = depth;

            if (_isLoading)
                return;

            RenderTex3D t = GetTexture();
            t?.Resize(_width, _height, _depth);
        }

        /// <summary>
        /// Call if you want to load all mipmap texture files, in a background thread for example.
        /// </summary>
        public void LoadMipmaps()
        {
            if (_mipmaps == null)
                return;
            _isLoading = true;
            foreach (var tref in _mipmaps)
                tref.GetInstance();
            if (_mipmaps.Length > 0)
            {
                var tref = _mipmaps[0];
                if (tref.File != null)
                {
                    var t = tref.File;
                    //if (t.Bitmaps.Length > 0)
                    //{
                    //    var b = t.Bitmaps[0];
                    //    if (b != null)
                    //    {
                    //        switch (b.PixelFormat)
                    //        {
                    //            case PixelFormat.Format32bppArgb:
                    //            case PixelFormat.Format32bppPArgb:
                    //                _internalFormat = EPixelInternalFormat.Rgba8;
                    //                _pixelFormat = EPixelFormat.Bgra;
                    //                _pixelType = EPixelType.UnsignedByte;
                    //                break;
                    //            case PixelFormat.Format24bppRgb:
                    //                _internalFormat = EPixelInternalFormat.Rgb8;
                    //                _pixelFormat = EPixelFormat.Bgr;
                    //                _pixelType = EPixelType.UnsignedByte;
                    //                break;
                    //            case PixelFormat.Format64bppArgb:
                    //            case PixelFormat.Format64bppPArgb:
                    //                _internalFormat = EPixelInternalFormat.Rgba16;
                    //                _pixelFormat = EPixelFormat.Bgra;
                    //                _pixelType = EPixelType.UnsignedShort;
                    //                break;
                    //        }
                    //    }
                    //}
                }
            }
            _isLoading = false;
        }

        public override void AttachToFBO(int mipLevel = 0)
        {
            if (FrameBufferAttachment.HasValue)
                AttachToFBO(FrameBufferAttachment.Value, mipLevel);
        }
        public override void AttachToFBO(EFramebufferAttachment attachment, int mipLevel = 0)
        {
            Engine.Renderer.AttachTextureToFrameBuffer(EFramebufferTarget.Framebuffer, attachment, _texture.BindingId, mipLevel);
        }
        public override void DetachFromFBO(int mipLevel = 0)
        {
            if (FrameBufferAttachment.HasValue)
                Engine.Renderer.AttachTextureToFrameBuffer(EFramebufferTarget.Framebuffer, FrameBufferAttachment.Value, 0, mipLevel);
        }

        public override void DetachFromFBO(EFramebufferAttachment attachment, int mipLevel = 0)
        {

        }
    }
}
