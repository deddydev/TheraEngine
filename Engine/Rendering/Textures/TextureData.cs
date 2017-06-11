using TheraEngine.Files;
using FreeImageAPI;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TheraEngine.Rendering.Textures
{
    [FileClass("", "", true)]
    public class TextureData : FileObject
    {
        public Bitmap Bitmap
        {
            get => _bitmap;
            set => _bitmap = value;
        }

        Bitmap _bitmap;

        public TextureData() : this(1, 1) { }
        public TextureData(int width, int height, System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        {
            _bitmap = new Bitmap(width, height, format);
        }
        public TextureData(string path)
        {
            _bitmap = File.Exists(path) ? new FreeImageBitmap(path).ToBitmap() : null;
        }
    }
}
