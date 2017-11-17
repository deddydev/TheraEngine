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

        Format8bppIntensityAlpha,
        Format16bppIntensityAlphai,
        Format16bppIntensityAlphaf,
        Format32bppIntensityAlphai,
        Format32bppIntensityAlphaf,

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
    }
    [FileClass("bmp3D", "3D Bitmap")]
    public class TBitmap3D : FileObject, IDisposable
    {
        public TBitmap3D() : this(1, 1, 1) { }
        public TBitmap3D(int width, int height, int depth)
        {
            _format = TPixelFormat.Format8bppRGBA;
            _pixelSize = GetPixelSize();
            _wh = width * height;
            _width = width;
            _height = height;
            _depth = depth;
            _data = new DataSource(_wh * _depth * _pixelSize);
        }
        public TBitmap3D(int width, int height, int depth, TPixelFormat format)
        {
            _format = format;
            _pixelSize = GetPixelSize();
            _wh = width * height;
            _width = width;
            _height = height;
            _depth = depth;
            _data = new DataSource(_wh * _depth * _pixelSize);
        }
        public TBitmap3D(int width, int height, int depth, TPixelFormat format, IntPtr scan0)
        {
            _format = format;
            _pixelSize = GetPixelSize();
            _wh = width * height;
            _width = width;
            _height = height;
            _depth = depth;
            _data = new DataSource(_wh * _depth * _pixelSize);
            Memory.Move(_data.Address, scan0, (uint)_data.Length);
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

                case TPixelFormat.Format8bppIntensityAlpha: return 2;
                case TPixelFormat.Format16bppIntensityAlphai: return 4;
                case TPixelFormat.Format16bppIntensityAlphaf: return 4;
                case TPixelFormat.Format32bppIntensityAlphai: return 8;
                case TPixelFormat.Format32bppIntensityAlphaf: return 8;

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
            SetPixel(_data.Address[index, _pixelSize], color);
        }

        internal TBitmap3D Resized(int width, int height, int depth)
        {
            throw new NotImplementedException();
        }

        private unsafe void SetPixel(VoidPtr addr, ColorF4 color)
        {
            switch (_format)
            {
                case TPixelFormat.Format8bIntensity:
                    *(byte*)addr = color.R.ToByte();
                    break;
                case TPixelFormat.Format16bIntensityi:
                    *(ushort*)addr = color.R.ToUShort();
                    break;
                case TPixelFormat.Format16bIntensityf:
                    *(Half*)addr = (Half)color.R;
                    break;
                case TPixelFormat.Format32bIntensityi:
                    *(uint*)addr = color.R.ToUInt();
                    break;
                case TPixelFormat.Format32bIntensityf:
                    *(float*)addr = color.R;
                    break;

                case TPixelFormat.Format8bppIntensityAlpha:
                    *(byte*)addr = color.R.ToByte();
                    *(byte*)(addr + 1) = color.A.ToByte();
                    break;
                case TPixelFormat.Format16bppIntensityAlphai:
                    *(ushort*)addr = color.R.ToUShort();
                    *(ushort*)(addr + 2) = color.A.ToUShort();
                    break;
                case TPixelFormat.Format16bppIntensityAlphaf:
                    *(Half*)addr = (Half)color.R;
                    *(Half*)(addr + 2) = (Half)color.A;
                    break;
                case TPixelFormat.Format32bppIntensityAlphai:
                    *(uint*)addr = color.R.ToUInt();
                    *(uint*)(addr + 4) = color.A.ToUInt();
                    break;
                case TPixelFormat.Format32bppIntensityAlphaf:
                    *(float*)addr = color.R;
                    *(float*)(addr + 4) = color.A;
                    break;

                case TPixelFormat.Format8bppRGB:
                    *(byte*)addr = color.R.ToByte();
                    *(byte*)(addr + 1) = color.G.ToByte();
                    *(byte*)(addr + 2) = color.B.ToByte();
                    break;
                case TPixelFormat.Format8bppRGBA:
                    *(byte*)addr = color.R.ToByte();
                    *(byte*)(addr + 1) = color.G.ToByte();
                    *(byte*)(addr + 2) = color.B.ToByte();
                    *(byte*)(addr + 3) = color.A.ToByte();
                    break;

                case TPixelFormat.Format16bppRGBi:
                    *(ushort*)addr = color.R.ToUShort();
                    *(ushort*)(addr + 2) = color.G.ToUShort();
                    *(ushort*)(addr + 4) = color.B.ToUShort();
                    break;
                case TPixelFormat.Format16bppRGBAi:
                    *(ushort*)addr = color.R.ToUShort();
                    *(ushort*)(addr + 2) = color.G.ToUShort();
                    *(ushort*)(addr + 4) = color.B.ToUShort();
                    *(ushort*)(addr + 6) = color.A.ToUShort();
                    break;

                case TPixelFormat.Format16bppRGBf:
                    *(Half*)addr = (Half)color.R;
                    *(Half*)(addr + 2) = (Half)color.G;
                    *(Half*)(addr + 4) = (Half)color.B;
                    break;
                case TPixelFormat.Format16bppRGBAf:
                    *(Half*)addr = (Half)color.R;
                    *(Half*)(addr + 2) = (Half)color.G;
                    *(Half*)(addr + 4) = (Half)color.B;
                    *(Half*)(addr + 6) = (Half)color.A;
                    break;

                case TPixelFormat.Format32bppRGBi:
                    *(uint*)addr = color.R.ToUInt();
                    *(uint*)(addr + 4) = color.G.ToUInt();
                    *(uint*)(addr + 8) = color.B.ToUInt();
                    break;
                case TPixelFormat.Format32bppRGBAi:
                    *(uint*)addr = color.R.ToUInt();
                    *(uint*)(addr + 4) = color.G.ToUInt();
                    *(uint*)(addr + 8) = color.B.ToUInt();
                    *(uint*)(addr + 12) = color.A.ToUInt();
                    break;

                case TPixelFormat.Format32bppRGBf:
                    *(float*)addr = color.R;
                    *(float*)(addr + 4) = color.G;
                    *(float*)(addr + 8) = color.B;
                    break;
                case TPixelFormat.Format32bppRGBAf:
                    *(float*)addr = color.R;
                    *(float*)(addr + 4) = color.G;
                    *(float*)(addr + 8) = color.B;
                    *(float*)(addr + 12) = color.A;
                    break;
            }
        }
        private unsafe ColorF4 GetPixel(VoidPtr addr)
        {
            switch (_format)
            {
                case TPixelFormat.Format8bIntensity: return new ColorF4(*(byte*)addr / (float)byte.MaxValue);
                case TPixelFormat.Format16bIntensityi: return new ColorF4(*(ushort*)addr / (float)ushort.MaxValue);
                case TPixelFormat.Format16bIntensityf: return new ColorF4(*(Half*)addr);
                case TPixelFormat.Format32bIntensityi: return new ColorF4(*(uint*)addr / (float)uint.MaxValue);
                case TPixelFormat.Format32bIntensityf: return new ColorF4(*(float*)addr);
                
                case TPixelFormat.Format8bppIntensityAlpha: return new ColorF4(*(byte*)addr / (float)byte.MaxValue, *(byte*)(addr + 1) / (float)byte.MaxValue);
                case TPixelFormat.Format16bppIntensityAlphai: return new ColorF4(*(ushort*)addr / (float)ushort.MaxValue, *(ushort*)(addr + 2) / (float)ushort.MaxValue);
                case TPixelFormat.Format16bppIntensityAlphaf: return new ColorF4(*(Half*)addr, *(Half*)(addr + 2));
                case TPixelFormat.Format32bppIntensityAlphai: return new ColorF4(*(uint*)addr / (float)uint.MaxValue, *(uint*)(addr + 4) / (float)uint.MaxValue);
                case TPixelFormat.Format32bppIntensityAlphaf: return new ColorF4(*(float*)addr, *(float*)(addr + 4));
                
                case TPixelFormat.Format8bppRGB: return new ColorF4(*(byte*)(addr + 0) / (float)byte.MaxValue, *(byte*)(addr + 1) / (float)byte.MaxValue, *(byte*)(addr + 2) / (float)byte.MaxValue);
                case TPixelFormat.Format8bppRGBA: return new ColorF4(*(byte*)(addr + 0) / (float)byte.MaxValue, *(byte*)(addr + 1) / (float)byte.MaxValue, *(byte*)(addr + 2) / (float)byte.MaxValue, *(byte*)(addr + 3) / (float)byte.MaxValue);

                case TPixelFormat.Format16bppRGBi: return new ColorF4(*(ushort*)(addr + 0) / (float)ushort.MaxValue, *(ushort*)(addr + 2) / (float)ushort.MaxValue, *(ushort*)(addr + 4) / (float)ushort.MaxValue);
                case TPixelFormat.Format16bppRGBAi: return new ColorF4(*(ushort*)(addr + 0) / (float)ushort.MaxValue, *(ushort*)(addr + 2) / (float)ushort.MaxValue, *(ushort*)(addr + 4) / (float)ushort.MaxValue, *(ushort*)(addr + 6) / (float)ushort.MaxValue);

                case TPixelFormat.Format16bppRGBf: return new ColorF4(*(Half*)(addr + 0), *(Half*)(addr + 2), *(Half*)(addr + 4));
                case TPixelFormat.Format16bppRGBAf: return new ColorF4(*(Half*)(addr + 0), *(Half*)(addr + 2), *(Half*)(addr + 4), *(Half*)(addr + 6));

                case TPixelFormat.Format32bppRGBi: return new ColorF4(*(uint*)(addr + 0) / (float)uint.MaxValue, *(uint*)(addr + 4) / (float)uint.MaxValue, *(uint*)(addr + 8) / (float)uint.MaxValue);
                case TPixelFormat.Format32bppRGBAi: return new ColorF4(*(uint*)(addr + 0) / (float)uint.MaxValue, *(uint*)(addr + 4) / (float)uint.MaxValue, *(uint*)(addr + 8) / (float)uint.MaxValue, *(uint*)(addr + 12) / (float)uint.MaxValue);

                case TPixelFormat.Format32bppRGBf: return new ColorF4(*(float*)(addr + 0), *(float*)(addr + 4), *(float*)(addr + 8));
                case TPixelFormat.Format32bppRGBAf: return new ColorF4(*(float*)(addr + 0), *(float*)(addr + 4), *(float*)(addr + 8), *(float*)(addr + 12));
            }
            return new ColorF4();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                _data?.Dispose();
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
         ~TBitmap3D()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
