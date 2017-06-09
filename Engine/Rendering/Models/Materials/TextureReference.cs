﻿using CustomEngine.Files;
using CustomEngine.Rendering.Textures;
using System.IO;
using System.Drawing.Imaging;
using FreeImageAPI;
using System.Drawing;

namespace CustomEngine.Rendering.Models.Materials
{
    public class TextureReference
    {
        public TextureReference() : this(null, 1, 1) { }
        public TextureReference(
            string name,
            int width,
            int height)
        {
            _name = name;
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
        }
        public TextureReference(string path, string name = "")
        {
            if (path.StartsWith("file://"))
                path = path.Substring(7);
            if (string.IsNullOrEmpty(name))
                _name = Path.GetFileNameWithoutExtension(path);
            _reference = new SingleFileRef<TextureData>(path);
        }

        public TextureData TextureData
        {
            get => _reference;
            set => _reference = value;
        }

        private SingleFileRef<TextureData> _reference;
        
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

        public Texture GetTexture()
            => new Texture(TextureData, _minFilter, _magFilter, _uWrap, _vWrap, _lodBias, _internalFormat, _pixelFormat, _pixelType);
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
