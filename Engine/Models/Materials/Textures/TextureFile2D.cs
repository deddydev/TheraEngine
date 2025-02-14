﻿using TheraEngine.Core.Files;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.ComponentModel;

namespace TheraEngine.Rendering.Textures
{
    public interface ITextureFile : IFileObject
    {

    }
    /// <summary>
    /// Wrapper class for a set of bitmaps, optionally (usually) stored in an external texture file such as a PNG or DDS.
    /// </summary>
    [TFile3rdPartyExt("png", "jpg", "jpeg", "tiff", "gif", "dds", "tga")]
    [TFileExt("tex2d", "png", "jpg", "jpeg", "tiff", "gif", "dds", "tga")]
    [TFileDef("Texture File 2D")]
    public class TextureFile2D : TFileObject, ITextureFile
    {
        public Bitmap[] Bitmaps { get; set; } = null;

        public Bitmap GetBitmap(int index = 0) => 
            Bitmaps != null && 
            index < Bitmaps.Length && 
            index >= 0 
                ? Bitmaps[0]
                : null;

        public TextureFile2D() 
            : this(1, 1) { }
        public TextureFile2D(int width, int height, PixelFormat format = PixelFormat.Format32bppArgb)
            => Bitmaps = new Bitmap[] { new Bitmap(width, height, format) };
        public TextureFile2D(string path)
            => ManualRead3rdParty(path);
        public TextureFile2D(string path, Action<TextureFile2D> onFinishedAsync)
        {
            Engine.Out("Loading texture async from " + path);
            Task.Run(() => TextureConverter.Decode(path)).ContinueWith(t => { Bitmaps = t.Result; onFinishedAsync?.Invoke(this); });
        }
        public override void ManualRead3rdParty(string filePath)
        {
            Engine.Out("Loading texture from " + filePath);
            Bitmaps = TextureConverter.Decode(filePath);
        }
    }
}
