﻿using TheraEngine.Files;
using TheraEngine.Rendering.Textures;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System;

namespace TheraEngine.Rendering.Models.Materials
{
    /// <summary>
    /// Used by materials to refer to either an external texture or an internal bitmap.
    /// </summary>
    public class TextureReference
    {
        public TextureReference() : this(null, 1, 1) { }
        public TextureReference(
            string name,
            int width,
            int height)
        {
            _name = name;
            _width = width;
            _height = height;
            _internalFormat = EPixelInternalFormat.Four;
            _pixelFormat = EPixelFormat.Bgra;
            _pixelType = EPixelType.UnsignedByte;
        }
        public TextureReference(
            string name,
            int width,
            int height,
            PixelFormat bitmapFormat) 
            : this(name, width, height)
        {
            TextureData = new TextureData(width, height, bitmapFormat);
            _width = width;
            _height = height;
        }
        public TextureReference(
            string name,
            int width,
            int height,
            EPixelInternalFormat internalFormat,
            EPixelFormat pixelFormat,
            EPixelType pixelType)
            : this(name, width, height)
        {
            TextureData = null;
            _internalFormat = internalFormat;
            _pixelFormat = pixelFormat;
            _pixelType = pixelType;
            _width = width;
            _height = height;
        }
        public TextureReference(
            string name,
            int width,
            int height,
            EPixelInternalFormat internalFormat,
            EPixelFormat pixelFormat,
            EPixelType pixelType,
            PixelFormat bitmapFormat)
            : this(name, width, height, internalFormat, pixelFormat, pixelType)
        {
            TextureData = new TextureData(width, height, bitmapFormat);
            _width = width;
            _height = height;
        }
        public TextureReference(string path, string name = "")
        {
            if (path.StartsWith("file://"))
                path = path.Substring(7);
            if (string.IsNullOrEmpty(name))
                _name = Path.GetFileNameWithoutExtension(path);
            else
                _name = name;
            _reference = new SingleFileRef<TextureData>(path);
            PixelFormat p = (TextureData == null || TextureData.Bitmap == null) ? PixelFormat.Format32bppArgb : TextureData.Bitmap.PixelFormat;
            switch (p)
            {
                case PixelFormat.Format32bppArgb:
                    _internalFormat = EPixelInternalFormat.Rgba8;
                    _pixelFormat = EPixelFormat.Bgra;
                    _pixelType = EPixelType.UnsignedByte;
                    break;
                case PixelFormat.Format24bppRgb:
                    _internalFormat = EPixelInternalFormat.Rgb8;
                    _pixelFormat = EPixelFormat.Bgr;
                    _pixelType = EPixelType.UnsignedByte;
                    break;
                default:
                    throw new Exception();
            }
        }

        public TextureData TextureData
        {
            get => _reference;
            set => _reference = value;
        }

        private SingleFileRef<TextureData> _reference;

        private int _width, _height;
        private string _name;
        private MinFilter _minFilter = MinFilter.Linear_Mipmap_Linear;
        private MagFilter _magFilter = MagFilter.Linear;
        private TexCoordWrap _uWrap = TexCoordWrap.Repeat, _vWrap = TexCoordWrap.Repeat;
        private float _lodBias = 0.0f;
        private EPixelInternalFormat _internalFormat;
        private EPixelFormat _pixelFormat;
        private EPixelType _pixelType;

        public string FilePath => _reference.FilePath;
        public MagFilter MagFilter
        {
            get => _magFilter;
            set => _magFilter = value;
        }
        public MinFilter MinFilter
        {
            get => _minFilter;
            set => _minFilter = value;
        }
        public TexCoordWrap UWrap
        {
            get => _uWrap;
            set => _uWrap = value;
        }
        public TexCoordWrap VWrap
        {
            get => _vWrap;
            set => _vWrap = value;
        }
        public float LodBias
        {
            get => _lodBias;
            set => _lodBias = value;
        }
        public Bitmap Bitmap
        {
            get => TextureData?.Bitmap;
            set
            {
                if (TextureData != null)
                    TextureData.Bitmap = value;
            }
        }

        public int Width => (TextureData != null && TextureData.Bitmap != null) ? TextureData.Bitmap.Width : _width;
        public int Height => (TextureData != null && TextureData.Bitmap != null) ? TextureData.Bitmap.Height : _height;

        public Texture2D GetTexture()
        {
            if (TextureData != null)
                return new Texture2D(TextureData, _minFilter, _magFilter, _uWrap, _vWrap, _lodBias, _internalFormat, _pixelFormat, _pixelType);
            else
                return new Texture2D(_width, _height, _minFilter, _magFilter, _uWrap, _vWrap, _lodBias, _internalFormat, _pixelFormat, _pixelType);
        }
    }
    public enum TexCoordWrap
    {
        Clamp,
        Repeat,
        Mirror
    }
    public enum MinFilter : uint
    {
        Nearest,
        Linear,
        Nearest_Mipmap_Nearest,
        Linear_Mipmap_Nearest,
        Nearest_Mipmap_Linear,
        Linear_Mipmap_Linear
    }
    public enum MagFilter : uint
    {
        Nearest,
        Linear,
    }
}
