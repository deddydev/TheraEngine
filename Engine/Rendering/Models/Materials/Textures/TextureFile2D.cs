using TheraEngine.Files;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System;

namespace TheraEngine.Rendering.Textures
{
    /// <summary>
    /// Wrapper class for a set of bitmaps, optionally (usually) stored in an external texture file such as a PNG or DDS.
    /// </summary>
    [FileClass("", "", IsSpecialDeserialize = true)]
    public class TextureFile2D : FileObject
    {
        private Bitmap[] _bitmaps = null;
        public Bitmap[] Bitmaps
        {
            get => _bitmaps;
            set => _bitmaps = value;
        }
        public TextureFile2D() : this(1, 1) { }
        public TextureFile2D(int width, int height, PixelFormat format = PixelFormat.Format32bppArgb)
            => _bitmaps = new Bitmap[] { new Bitmap(width, height, format) };
        public TextureFile2D(string path)
        {
            Engine.PrintLine("Loading texture from " + path);
            _bitmaps = /*Task.Run(() => */TextureConverter.Decode(path)/*).ContinueWith(t => _bitmaps = t.Result)*/;
        }
        public TextureFile2D(string path, Action<TextureFile2D> onFinishedAsync)
        {
            Engine.PrintLine("Loading texture async from " + path);
            Task.Run(() => TextureConverter.Decode(path)).ContinueWith(t => { _bitmaps = t.Result; onFinishedAsync?.Invoke(this); });
        }
    }
}
