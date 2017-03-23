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
        public override ResourceType ResourceType => ResourceType.Texture;
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
        public TextureData(int width, int height, System.Drawing.Imaging.PixelFormat format)
        {
            _bitmap = new Bitmap(width, height, format);
        }

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }
    }
}
