using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Files;
using CustomEngine.Rendering.Textures;
using System.IO;
using System.Drawing;

namespace CustomEngine.Rendering.Models.Materials
{
    public class TextureReference
    {
        public TextureReference(string name, int width, int height)
        {
            _name = name;
            _reference = new TextureData(width, height);
        }
        public TextureReference(string path, string name = "")
        {
            if (path.StartsWith("file://"))
                path = path.Substring(7);
            if (string.IsNullOrEmpty(name))
                _name = Path.GetFileNameWithoutExtension(path);
            _reference = new SingleFileRef<TextureData>(path);
        }
        
        private SingleFileRef<TextureData> _reference;

        private string _name;
        private MinFilter _minFilter = MinFilter.Linear_Mipmap_Linear;
        private MagFilter _magFilter = MagFilter.Linear;
        private TexCoordWrap _uWrap = TexCoordWrap.Repeat, _vWrap = TexCoordWrap.Repeat;
        private float _lodBias = 0.0f;

        public string FilePath { get { return _reference.FilePath; } }
        public MagFilter MagFilter { get { return _magFilter; } set { _magFilter = value; } }
        public MinFilter MinFilter { get { return _minFilter; } set { _minFilter = value; } }
        public TexCoordWrap UWrap { get { return _uWrap; } set { _uWrap = value; } }
        public TexCoordWrap VWrap { get { return _vWrap; } set { _vWrap = value; } }
        public float LodBias { get { return _lodBias; } set { _lodBias = value; } }
        public Bitmap Bitmap => _reference.File.Bitmap;

        public Texture GetTexture()
        {
            TextureData data = _reference.File;
            if (data == null)
                return null;
            return new Texture(data, _minFilter, _magFilter, _uWrap, _vWrap, _lodBias);
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
