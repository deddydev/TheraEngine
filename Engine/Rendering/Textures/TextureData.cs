using TheraEngine.Files;
using FreeImageAPI;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace TheraEngine.Rendering.Textures
{
    /// <summary>
    /// Wrapper class for a set of bitmaps, optionally (usually) stored in an external texture file such as a PNG or DDS.
    /// </summary>
    [FileClass("", "", true)]
    public class TextureData : FileObject
    {
        private Bitmap[] _bitmaps;
        public Bitmap[] Bitmaps
        {
            get => _bitmaps;
            set => _bitmaps = value;
        }
        public TextureData() : this(1, 1) { }
        public TextureData(int width, int height, PixelFormat format = PixelFormat.Format32bppArgb)
            => _bitmaps = new Bitmap[] { new Bitmap(width, height, format) };
        public TextureData(string path)
            => _bitmaps = TextureConverter.Decode(path);
    }
}
