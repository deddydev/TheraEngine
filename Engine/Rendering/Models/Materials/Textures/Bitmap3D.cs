using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using TheraEngine.Files;
using System.ComponentModel;
using OpenTK;

namespace TheraEngine.Rendering.Models.Materials.Textures
{
    public enum TPixelFormat
    {
        Format8bIntensity,
        Format16bIntensityi,
        Format16bIntensityf,
        Format32bIntensityi,
        Format32bIntensityf,
        Format64bIntensityi,
        Format64bIntensityf,

        Format8bppIntensityAlpha,
        Format16bppIntensityAlphai,
        Format16bppIntensityAlphaf,
        Format32bppIntensityAlphai,
        Format32bppIntensityAlphaf,
        Format64bppIntensityAlphai,
        Format64bppIntensityAlphaf,

        Format8bppRGB,
        Format8bppRGBA,

        Format16bppRGBi,
        Format16bppRGBAi,

        Format16bppRGBf,
        Format16bppRGBAf,

        Format32bppRGBi,
        Format32bppRGBAi,

        Format32bppRGBf,
        Format32bppRGBAf,

        Format64bppRGBi,
        Format64bppRGBAi,

        Format64bppRGBf,
        Format64bppRGBAf,
    }
    [FileClass("tex3D", "3D Texture")]
    public class TBitmap3D : FileObject
    {
        public TBitmap3D()
        {

        }
        public TBitmap3D(int width, int height, int depth)
        {
            _pixelSize = GetPixelSize();
            _wh = width * height;
            _width = width;
            _height = height;
            _depth = depth;

            _data = new DataSource(_wh * _depth * _pixelSize);
        }
        public TBitmap3D(int width, int height, int depth, TPixelFormat format) : this(width, height, depth)
        {

        }
        public TBitmap3D(int width, int height,  int depth,int stride, TPixelFormat format, IntPtr scan0)
        {

        }

        private TPixelFormat _format;
        private DataSource _data;
        private int _width, _height, _depth, _wh, _pixelSize;

        public VoidPtr Scan0 => _data.Address;
        public TPixelFormat Format => _format;

        public int Width => _width; 
        public int Height => _height;
        public int Depth => _depth; 

        public int GetPixelSize()
        {
            switch (_format)
            {
                case TPixelFormat.Format8bIntensity: return 1;
                case TPixelFormat.Format16bIntensityi: return 2;
                case TPixelFormat.Format16bIntensityf: return 2;
                case TPixelFormat.Format32bIntensityi: return 4;
                case TPixelFormat.Format32bIntensityf: return 4;
                case TPixelFormat.Format64bIntensityi: return 8;
                case TPixelFormat.Format64bIntensityf: return 8;

                case TPixelFormat.Format8bppIntensityAlpha: return 2;
                case TPixelFormat.Format16bppIntensityAlphai: return 4;
                case TPixelFormat.Format16bppIntensityAlphaf: return 4;
                case TPixelFormat.Format32bppIntensityAlphai: return 8;
                case TPixelFormat.Format32bppIntensityAlphaf: return 8;
                case TPixelFormat.Format64bppIntensityAlphai: return 16;
                case TPixelFormat.Format64bppIntensityAlphaf: return 16;

                case TPixelFormat.Format8bppRGB: return 3;
                case TPixelFormat.Format8bppRGBA: return 4;

                case TPixelFormat.Format16bppRGBi: return 6;
                case TPixelFormat.Format16bppRGBAi: return 8;

                case TPixelFormat.Format16bppRGBf: return 6;
                case TPixelFormat.Format16bppRGBAf: return 8;

                case TPixelFormat.Format32bppRGBi: return 12;
                case TPixelFormat.Format32bppRGBAi: return 16;

                case TPixelFormat.Format32bppRGBf: return 12;
                case TPixelFormat.Format32bppRGBAf: return 16;

                case TPixelFormat.Format64bppRGBi: return 24;
                case TPixelFormat.Format64bppRGBAi: return 32;

                case TPixelFormat.Format64bppRGBf: return 24;
                case TPixelFormat.Format64bppRGBAf: return 32;
            }
            return 0;
        }
        public ColorF4 GetPixel(int x, int y, int z)
        {
            int index = x * (y * _width) * (z * _wh);
            return GetPixel(_data.Address[index, _pixelSize]);
        }
        public void SetPixel(int x, int y, int z, ColorF4 color)
        {
            int index = x * (y * _width) * (z * _wh);
            SetPixel(_data.Address[index, _pixelSize]);
        }
        private void SetPixel(VoidPtr addr)
        {
            
        }
        private unsafe ColorF4 GetPixel(VoidPtr addr)
        {
            switch (_format)
            {
                case TPixelFormat.Format8bIntensity: return new ColorF4(*(byte*)addr);
                case TPixelFormat.Format16bIntensityi: return new ColorF4(*(ushort*)addr);
                case TPixelFormat.Format16bIntensityf: return new ColorF4(*(Half*)addr);
                case TPixelFormat.Format32bIntensityi: return new ColorF4(*(uint*)addr);
                case TPixelFormat.Format32bIntensityf: return new ColorF4(*(float*)addr);
                case TPixelFormat.Format64bIntensityi: return new ColorF4(*(ulong*)addr);
                case TPixelFormat.Format64bIntensityf: return new ColorF4((float)*(double*)addr);

                case TPixelFormat.Format8bppIntensityAlpha: return new ColorF4(*(byte*)addr, *(byte*)(addr + 1));
                case TPixelFormat.Format16bppIntensityAlphai: return new ColorF4(*(ushort*)addr, *(ushort*)(addr + 2));
                case TPixelFormat.Format16bppIntensityAlphaf: return new ColorF4(*(Half*)addr, *(Half*)(addr + 2));
                case TPixelFormat.Format32bppIntensityAlphai: return new ColorF4(*(uint*)addr, *(uint*)(addr + 4));
                case TPixelFormat.Format32bppIntensityAlphaf: return new ColorF4(*(float*)addr, *(float*)(addr + 4));
                case TPixelFormat.Format64bppIntensityAlphai: return new ColorF4(*(ulong*)addr, *(ulong*)(addr + 8));
                case TPixelFormat.Format64bppIntensityAlphaf: return new ColorF4((float)*(double*)addr, (float)*(double*)(addr + 8));

                case TPixelFormat.Format8bppRGB: return new ColorF4(*(byte*)(addr + 0), *(byte*)(addr + 1), *(byte*)(addr + 2));
                case TPixelFormat.Format8bppRGBA: return new ColorF4(*(byte*)(addr + 0), *(byte*)(addr + 1), *(byte*)(addr + 2), *(byte*)(addr + 3));

                case TPixelFormat.Format16bppRGBi: return new ColorF4(*(ushort*)(addr + 0), *(ushort*)(addr + 2), *(ushort*)(addr + 4));
                case TPixelFormat.Format16bppRGBAi: return new ColorF4(*(ushort*)(addr + 0), *(ushort*)(addr + 2), *(ushort*)(addr + 4), *(ushort*)(addr + 6));

                case TPixelFormat.Format16bppRGBf: return new ColorF4(*(Half*)(addr + 0), *(Half*)(addr + 2), *(Half*)(addr + 4));
                case TPixelFormat.Format16bppRGBAf: return new ColorF4(*(Half*)(addr + 0), *(Half*)(addr + 2), *(Half*)(addr + 4), *(Half*)(addr + 6));

                case TPixelFormat.Format32bppRGBi: return new ColorF4(*(uint*)(addr + 0), *(uint*)(addr + 4), *(uint*)(addr + 8));
                case TPixelFormat.Format32bppRGBAi: return new ColorF4();

                case TPixelFormat.Format32bppRGBf: return new ColorF4(*(float*)(addr + 0), *(float*)(addr + 4), *(float*)(addr + 8));
                case TPixelFormat.Format32bppRGBAf: return new ColorF4();

                case TPixelFormat.Format64bppRGBi: return new ColorF4(*(ulong*)(addr + 0), *(ulong*)(addr + 8), *(ulong*)(addr + 16));
                case TPixelFormat.Format64bppRGBAi: return new ColorF4();

                case TPixelFormat.Format64bppRGBf: return new ColorF4((float)*(double*)(addr + 0), (float)*(double*)(addr + 8), (float)*(double*)(addr + 16));
                case TPixelFormat.Format64bppRGBAf: return new ColorF4();
            }
            return new ColorF4();
        }
    }
}
