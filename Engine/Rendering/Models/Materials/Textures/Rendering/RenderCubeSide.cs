using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials.Textures
{
    public class RenderCubeSide : IDisposable
    {
        public RenderCubeSide(Bitmap map)
        {
            Map = map;
            Width = map.Width;
            Height = map.Height;
            switch (map.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    InternalFormat = EPixelInternalFormat.Rgba8;
                    PixelFormat = EPixelFormat.Bgra;
                    PixelType = EPixelType.UnsignedByte;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    InternalFormat = EPixelInternalFormat.Rgb8;
                    PixelFormat = EPixelFormat.Bgr;
                    PixelType = EPixelType.UnsignedByte;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format64bppPArgb:
                    InternalFormat = EPixelInternalFormat.Rgba16;
                    PixelFormat = EPixelFormat.Bgra;
                    PixelType = EPixelType.UnsignedShort;
                    break;
            }
        }
        public RenderCubeSide(int width, int height, EPixelInternalFormat internalFormat, EPixelFormat format, EPixelType type)
        {
            InternalFormat = internalFormat;
            PixelFormat = format;
            PixelType = type;
            Width = width;
            Height = height;
            Map = null;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Bitmap Map { get; private set; }
        public EPixelFormat PixelFormat { get; private set; }
        public EPixelType PixelType { get; private set; }
        public EPixelInternalFormat InternalFormat { get; private set; }

        public static implicit operator RenderCubeSide(Bitmap bitmap) => new RenderCubeSide(bitmap);

        public void PushData(int sideIndex, int mipIndex)
        {
            ETexTarget target = ETexTarget.TextureCubeMapPositiveX + sideIndex;
            if (Map == null)
                Engine.Renderer.PushTextureData(target, mipIndex, InternalFormat, Width, Height, PixelFormat, PixelType, IntPtr.Zero);
            else
            {
                BitmapData data = Map.LockBits(new Rectangle(0, 0, Map.Width, Map.Height), ImageLockMode.ReadOnly, Map.PixelFormat);
                Engine.Renderer.PushTextureData(target, mipIndex, InternalFormat, Map.Width, Map.Height, PixelFormat, PixelType, data.Scan0);
                Map.UnlockBits(data);
            }
        }

        public void Dispose()
        {
            Map?.Dispose();
        }
    }
}
