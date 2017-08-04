﻿using TheraEngine.Files;
using TheraEngine.Rendering.Textures;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System;
using System.ComponentModel;
using System.Linq;

namespace TheraEngine.Rendering.Models.Materials
{
    [FileClass("TREF", "Texture Reference")]
    public class TextureReference : FileObject
    {
        #region Constructors
        public TextureReference() : this(null, 1, 1) { }
        public TextureReference(string name, int width, int height)
        {
            _mipmaps = null;
            _name = name;
            _width = width;
            _height = height;
            _internalFormat = EPixelInternalFormat.Rgba8;
            _pixelFormat = EPixelFormat.Bgra;
            _pixelType = EPixelType.UnsignedByte;
        }
        public TextureReference(string name, int width, int height,
            PixelFormat bitmapFormat = PixelFormat.Format32bppArgb, int mipCount = 1)
            : this(name, width, height)
        {
            _mipmaps = new SingleFileRef<TextureFile>[mipCount];
            for (int i = 0, scale = 1; i < mipCount; scale = 1 << ++i)
                _mipmaps[i] = new TextureFile(width / scale, height / scale, bitmapFormat);
        }
        public TextureReference(string name, int width, int height,
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
        public TextureReference(string name, int width, int height,
            EPixelInternalFormat internalFormat, EPixelFormat pixelFormat, EPixelType pixelType, PixelFormat bitmapFormat)
            : this(name, width, height, internalFormat, pixelFormat, pixelType)
        {
            _mipmaps = new SingleFileRef<TextureFile>[] { new TextureFile(width, height, bitmapFormat) };
        }
        public TextureReference(string name, params string[] mipMapPaths)
        {
            _name = name;
            _mipmaps = new SingleFileRef<TextureFile>[mipMapPaths.Length];
            for (int i = 0; i < mipMapPaths.Length; ++i)
            {
                string path = mipMapPaths[i];
                if (path.StartsWith("file://"))
                    path = path.Substring(7);
                _mipmaps = new SingleFileRef<TextureFile>[]
                {
                    new SingleFileRef<TextureFile>(path)
                };
            }
        }
        #endregion

        //Note: one TextureData object may contain all the mips
        public SingleFileRef<TextureFile>[] _mipmaps;
        public SingleFileRef<TextureFile>[] Mipmaps
        {
            get => _mipmaps;
            set => _mipmaps = value;
        }

        /// <summary>
        /// Call if you want to load all mipmap texture files, in a background thread for example.
        /// </summary>
        public void LoadMipmaps()
        {
            if (_mipmaps == null)
                return;
            foreach (var tref in _mipmaps)
                tref.GetInstance();
            if (_mipmaps.Length > 0)
            {
                var tref = _mipmaps[0];
                if (tref.File != null)
                {
                    var t = tref.File;
                    if (t.Bitmaps.Length > 0)
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
        }

        private Texture2D _texture;

        private int _width, _height, _index;
        private ETexWrapMode _uWrapMode = ETexWrapMode.Repeat;
        private ETexWrapMode _vWrapMode = ETexWrapMode.Repeat;
        private ETexMinFilter _minFilter = ETexMinFilter.LinearMipmapLinear;
        private ETexMagFilter _magFilter = ETexMagFilter.Linear;
        private float _lodBias = 0.0f;
        private EPixelInternalFormat _internalFormat;
        private EPixelFormat _pixelFormat;
        private EPixelType _pixelType;
        private EFramebufferAttachment? _frameBufferAttachment;

        public EFramebufferAttachment? FrameBufferAttachment
        {
            get => _frameBufferAttachment;
            set => _frameBufferAttachment = value;
        }
        public int Index
        {
            get => _index;
            set => _index = value;
        }
        public ETexMagFilter MagFilter
        {
            get => _magFilter;
            set => _magFilter = value;
        }
        public ETexMinFilter MinFilter
        {
            get => _minFilter;
            set => _minFilter = value;
        }
        public ETexWrapMode UWrap
        {
            get => _uWrapMode;
            set => _uWrapMode = value;
        }
        public ETexWrapMode VWrap
        {
            get => _uWrapMode;
            set => _uWrapMode = value;
        }
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

        public Texture2D Texture
        {
            get
            {
                if (_texture != null)
                    return _texture;
                LoadMipmaps();
                if (_mipmaps != null && _mipmaps.Length > 0)
                    _texture = new Texture2D(_internalFormat, _pixelFormat, _pixelType, _mipmaps.SelectMany(x => x.File == null || x.File.Bitmaps == null ? new Bitmap[0] : x.File.Bitmaps).ToArray());
                else
                    _texture = new Texture2D(_width, _height, _internalFormat, _pixelFormat, _pixelType);
                _texture.PostPushData += SetParameters;
                return _texture;
            }
        }

        public Material Material { get; internal set; }
        public bool DoNotResize { get; internal set; }

        /// <summary>
        /// Resizes the textures stored in memory.
        /// </summary>
        public void Resize(int width, int height)
        {
            if (DoNotResize)
                return;
            _width = width;
            _height = height;
            Texture?.Resize(width, height);
        }
    }
}
