using FreeImageAPI;
using System;
using System.IO;

namespace CustomEngine.Rendering.Textures
{
    public class Texture : BaseRenderState
    {
        private FreeImageBitmap _bitmap;
        public FreeImageBitmap Bitmap
        {
            get { return _bitmap; }
            set
            {
                Delete();
                _bitmap = value;
            }
        }

        public Texture(FreeImageBitmap bitmap) : base(GenType.Texture) { _bitmap = bitmap; }

        protected override void OnGenerated()
        {
            base.OnGenerated();
        }

        protected override void OnDeleted()
        {
            base.OnDeleted();
        }

        //public new static Texture FromFile(string filename) { return FreeImageBitmap.FromFile(filename) as Texture; }
        //public new static Texture FromFile(string filename, bool useEmbeddedColorManagement) { return FreeImageBitmap.FromFile(filename, useEmbeddedColorManagement) as Texture; }
        //public new static Texture FromHbitmap(IntPtr hbitmap) { return FreeImageBitmap.FromHbitmap(hbitmap) as Texture; }
        //public new static Texture FromHbitmap(IntPtr hbitmap, IntPtr hpalette) { return FreeImageBitmap.FromHbitmap(hbitmap, hpalette) as Texture; }
        //public new static Texture FromHicon(IntPtr hicon) { return FreeImageBitmap.FromHicon(hicon) as Texture; }
        //public new static Texture FromResource(IntPtr hinstance, string bitmapName) { return FreeImageBitmap.FromResource(hinstance, bitmapName) as Texture; }
        //public new static Texture FromStream(Stream stream) { return FreeImageBitmap.FromStream(stream) as Texture; }
        //public new static Texture FromStream(Stream stream, bool useEmbeddedColorManagement) { return FreeImageBitmap.FromStream(stream, useEmbeddedColorManagement) as Texture; }
        //public new static Texture FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData) { return FreeImageBitmap.FromStream(stream, useEmbeddedColorManagement, validateImageData) as Texture; }
    }
}
