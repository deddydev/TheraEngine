using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Files;
using CustomEngine.Rendering.Textures;

namespace CustomEngine.Rendering.Models.Materials
{
    public class TextureReference
    {
        public TextureReference(Material m, string path)
        {
            _owningMaterial = m;
            _reference = new SingleFileRef<TextureData>(path);
        }

        private Material _owningMaterial;
        private SingleFileRef<TextureData> _reference;

        private TextureMinFilter _minFilter;
        private TextureMagFilter _magFilter;
        private TexCoordWrap _uWrap, _vWrap;

        public string Path { get { return _reference.FilePathAbsolute; } }
        public TextureMagFilter MagFilter { get { return _magFilter; } set { _magFilter = value; } }
        public TextureMinFilter MinFilter { get { return _minFilter; } set { _minFilter = value; } }
        public TexCoordWrap UWrap { get { return _uWrap; } set { _uWrap = value; } }
        public TexCoordWrap VWrap { get { return _vWrap; } set { _vWrap = value; } }

        public Texture GetTexture()
        {
            TextureData data = _reference.File;
            if (data == null)
                return null;
            return new Texture(data);
        }
    }
    public enum TexCoordWrap
    {
        Clamp,
        Repeat,
        Mirror
    }
    public enum TextureMinFilter : uint
    {
        Nearest,
        Linear,
        Nearest_Mipmap_Nearest,
        Linear_Mipmap_Nearest,
        Nearest_Mipmap_Linear,
        Linear_Mipmap_Linear
    }
    public enum TextureMagFilter : uint
    {
        Nearest,
        Linear,
    }
}
