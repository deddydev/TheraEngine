﻿using TheraEngine.Files;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Textures
{
    /// <summary>
    /// Wrapper class for a set of bitmaps, optionally (usually) stored in an external texture file such as a PNG or DDS.
    /// </summary>
    [File3rdParty("png", "jpg", "jpeg", "tiff", "gif", "dds", "tga")]
    [FileExt("tex2d")]
    [FileDef("Texture File 2D")]
    public class TextureFile2D : TFileObject
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
        public TextureFile2D(string path) => Read3rdParty(path);
        public TextureFile2D(string path, Action<TextureFile2D> onFinishedAsync)
        {
            Engine.PrintLine("Loading texture async from " + path);
            Task.Run(() => TextureConverter.Decode(path)).ContinueWith(t => { _bitmaps = t.Result; onFinishedAsync?.Invoke(this); });
        }
        public override void Read3rdParty(string filePath)
        {
            Engine.PrintLine("Loading texture from " + filePath);
            _bitmaps = TextureConverter.Decode(filePath);
        }
    }
}
