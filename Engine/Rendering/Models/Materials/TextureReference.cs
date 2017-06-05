using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Files;
using CustomEngine.Rendering.Textures;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace CustomEngine.Rendering.Models.Materials
{
    public class TextureReference
    {
        public TextureReference(
            string name,
            int width,
            int height,
            EPixelInternalFormat internalFormat,
            EPixelFormat format,
            EPixelType type)
        {
            _name = name;
            _internalFormat = internalFormat;
            _pixelFormat = format;
            _pixelType = type;
        }
        public TextureReference(string name,
            int width,
            int height,
            EPixelInternalFormat internalFormat,
            EPixelFormat format,
            EPixelType type,
            PixelFormat bitmapFormat) 
            : this(name, width, height, internalFormat, format, type)
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
        public TextureReference()
        {

        }

        public TextureData TextureData
        {
            get => _reference;
            set => _reference = value;
        }

        private SingleFileRef<TextureData> _reference;

        private EPixelInternalFormat _internalFormat = EPixelInternalFormat.Rgb8;
        private EPixelFormat _pixelFormat = EPixelFormat.Rgb;
        private EPixelType _pixelType = EPixelType.UnsignedByte;

        private string _name;
        private MinFilter _minFilter = MinFilter.Linear_Mipmap_Linear;
        private MagFilter _magFilter = MagFilter.Linear;
        private TexCoordWrap _uWrap = TexCoordWrap.Repeat, _vWrap = TexCoordWrap.Repeat;
        private float _lodBias = 0.0f;

        public string FilePath => _reference.FilePath;
        public MagFilter MagFilter { get => _magFilter; set => _magFilter = value; }
        public MinFilter MinFilter { get => _minFilter; set => _minFilter = value; }
        public TexCoordWrap UWrap { get => _uWrap; set => _uWrap = value; }
        public TexCoordWrap VWrap { get => _vWrap; set => _vWrap = value; }
        public float LodBias { get => _lodBias; set => _lodBias = value; }
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
        {
            return new Texture(TextureData, _minFilter, _magFilter, _uWrap, _vWrap, _lodBias, _internalFormat, _pixelFormat, _pixelType);
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
