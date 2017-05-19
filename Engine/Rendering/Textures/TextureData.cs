using CustomEngine.Files;
using FreeImageAPI;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CustomEngine.Rendering.Textures
{
    public class TextureData : FileObject
    {
        public Bitmap Bitmap
        {
            get => _bitmap;
            set => _bitmap = value;
        }

        Bitmap _bitmap;

        public TextureData()
        {
            _bitmap = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }
        public TextureData(string path)
        {
            _bitmap = File.Exists(path) ? new FreeImageBitmap(path).ToBitmap() : null;
        }
        public TextureData(int width, int height, System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        {
            _bitmap = new Bitmap(width, height, format);
        }
    }
}
