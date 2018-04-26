using System;
using System.Drawing;
using System.Runtime.InteropServices;
using TheraEngine.Core.Memory;

namespace TheraEngine.Rendering.Models.Materials
{
    public enum ETPixelComps
    {
        R,
        RG,
        RGB,
        RGBA,
    }
    public enum ETPixelCompFmt
    {
        U8,
        S8,
        U16,
        S16,
        U24,
        S24,
        U32,
        S32,
        F16,
        F32,
    }
    public enum ETPixelType
    {
        /// <summary>
        /// Each component has its own spot in memory (no optimizations).
        /// </summary>
        Basic,
        /// <summary>
        /// Each component has its own bit count set in the texture's header.
        /// </summary>
        Specific,
        /// <summary>
        /// Each component of each pixel is multiplied by a respective component scale value in the texture's header.
        /// </summary>
        IndivOverallQuantized,
        /// <summary>
        /// Each component of each pixel is multiplied by the same scale value in the texture's header.
        /// </summary>
        SharedOverallQuantized,
        /// <summary>
        /// The texture has only an R component.
        /// Each individual pixel has a quantization scale.
        /// 2 components total.
        /// </summary>
        RIndivQuantized,
        /// <summary>
        /// The texture has only an R component.
        /// Each individual pixel has a quantization scale.
        /// 3 components total.
        /// </summary>
        RGSharedQuantized,
        /// <summary>
        /// The texture has only an R component.
        /// Each individual pixel has a quantization scale.
        /// 4 components total.
        /// </summary>
        RGIndivQuantized,
        /// <summary>
        /// The texture has only an R component.
        /// Each individual pixel has a quantization scale.
        /// 4 components total.
        /// </summary>
        RGBSharedQuantized,
        
        /// <summary>
        /// R has 5 bits, G has 6 bits, and B has 5 bits.
        /// 16 bits total for optimized r/w speed.
        /// </summary>
        RGB565,
        /// <summary>
        /// R, G, B and A all have their own 4 bits.
        /// 16 bits total for optimized r/w speed.
        /// </summary>
        RGBA4444,
        /// <summary>
        /// R, G and B have 5 bits if A is 1.0f.
        /// Otherwise, R, G and B have 4 bits and A has 3.
        /// 16 bits total for optimized r/w speed.
        /// </summary>
        RGBA5553,

        /// <summary>
        /// R, G, B and A all have their own 6 bits.
        /// 24 bits total for optimized r/w speed.
        /// </summary>
        RGBA6666,
    }
    /// <summary>
    /// The alpha component is unused and exists purely for padding purposes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGBXPixel
    {
        public byte R, G, B, X;

        public static explicit operator RGBXPixel(ARGBPixel p) { return new RGBXPixel() { R = p.R, G = p.G, B = p.B, X = 0 }; }
        public static explicit operator ARGBPixel(RGBXPixel p) { return new ARGBPixel() { A = 0xFF, R = p.R, G = p.G, B = p.B }; }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RQuantizedPixel
    {
        public byte R, Scale;

        public static explicit operator RQuantizedPixel(float p)
        {
            float s = 65025.0f / float.MaxValue * p;
            return new RQuantizedPixel() { };
        }
        public static explicit operator float(RQuantizedPixel p)
            => p.R * p.Scale;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGBA4Pixel
    {
        //1111 0000 0000 0000 half R
        //0000 1111 0000 0000 half G       
        //0000 0000 1111 0000 half B
        //0000 0000 0000 1111 half A

        //Conversion: C26F -> CC2266FF (RGBA)

        Bin16 _data;

        public byte this[int index]
        {
            get
            {
                ushort val = _data[(3 - index) * 4, 4];
                return (byte)(val | (val << 4));
            }
            set => _data[(3 - index) * 4, 4] = (ushort)((value & 0xFF) >> 4);
        }
        public byte R
        {
            get => this[0];
            set => this[0] = value;
        }
        public byte G
        {
            get => this[1];
            set => this[1] = value;
        }
        public byte B
        {
            get => this[2];
            set => this[2] = value;
        }
        public byte A
        {
            get => this[3];
            set => this[3] = value;
        }

        public static explicit operator ARGBPixel(RGBA4Pixel p)
        {
            int val = p._data;
            int r = val & 0xF000;
            r = (r >> 8) | (r >> 12);
            int g = val & 0x0F00;
            g = (g >> 4) | (g >> 8);
            int b = val & 0x00F0;
            b |= (b >> 4);
            int a = val & 0x000F;
            a |= (a << 4);
            return new ARGBPixel((byte)a, (byte)r, (byte)g, (byte)b);
        }
    }
    /// <summary>
    /// Each component uses 6 bits.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGBA6Pixel
    {
        byte _b1, _b2, _b3;

        public static explicit operator ARGBPixel(RGBA6Pixel p)
        {
            int val = (p._b1 << 16) | (p._b2 << 8) | p._b3;
            int r = val & 0xFC0000;
            r = (r >> 16) | (r >> 22);
            int g = val & 0x3F000;
            g = (g >> 10) | (g >> 16);
            int b = val & 0xFC0;
            b = (b >> 4) | (b >> 10);
            int a = val & 0x3F;
            a = (a << 2) | (a >> 4);
            return new ARGBPixel((byte)a, (byte)r, (byte)g, (byte)b);
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGB565Pixel
    {
        public bushort _data;

        public RGB565Pixel(bushort data) { _data = data; }
        public RGB565Pixel(int r, int g, int b)
        {
            r = Convert.ToInt32(r * (31.0 / 255.0));
            g = Convert.ToInt32(g * (63.0 / 255.0));
            b = Convert.ToInt32(b * (31.0 / 255.0));
            _data = (ushort)((r << 11) | (g << 5) | b);
        }

        public static bool operator >(RGB565Pixel p1, RGB565Pixel p2) { return (ushort)p1._data > (ushort)p2._data; }
        public static bool operator <(RGB565Pixel p1, RGB565Pixel p2) { return (ushort)p1._data < (ushort)p2._data; }
        public static bool operator >=(RGB565Pixel p1, RGB565Pixel p2) { return (ushort)p1._data >= (ushort)p2._data; }
        public static bool operator <=(RGB565Pixel p1, RGB565Pixel p2) { return (ushort)p1._data <= (ushort)p2._data; }
        public static bool operator ==(RGB565Pixel p1, RGB565Pixel p2) { return p1.Equals(p2); }
        public static bool operator !=(RGB565Pixel p1, RGB565Pixel p2) { return !p1.Equals(p2); }

        public static explicit operator ARGBPixel(RGB565Pixel p)
        {
            int r, g, b;
            ushort val = p._data;
            r = (val >> 11) & 0x1F;
            r = Convert.ToInt32(r * (255.0 / 31.0));
            g = (val >> 5) & 0x3F;
            g = Convert.ToInt32(g * (255.0 / 63.0));
            b = val & 0x1F;
            b = Convert.ToInt32(b * (255.0 / 31.0));
            return new ARGBPixel(0xFF, (byte)r, (byte)g, (byte)b);
        }
        public static explicit operator RGBPixel(RGB565Pixel p)
        {
            int r, g, b;
            ushort val = p._data;
            r = (val >> 11) & 0x1F;
            r = Convert.ToInt32(r * (255.0 / 31.0));
            g = (val >> 5) & 0x3F;
            g = Convert.ToInt32(g * (255.0 / 63.0));
            b = val & 0x1F;
            b = Convert.ToInt32(b * (255.0 / 31.0));
            return new RGBPixel() { R = (byte)r, G = (byte)g, B = (byte)b };
        }
        public static explicit operator Color(RGB565Pixel p)
        {
            int r, g, b;
            ushort val = p._data;
            r = (val >> 11) & 0x1F;
            r = Convert.ToInt32(r * (255.0 / 31.0));
            g = (val >> 5) & 0x3F;
            g = Convert.ToInt32(g * (255.0 / 63.0));
            b = val & 0x1F;
            b = Convert.ToInt32(b * (255.0 / 31.0));
            return Color.FromArgb(0xFF, r, g, b);
        }
        public static explicit operator RGB565Pixel(ARGBPixel p) { return new RGB565Pixel(p.R, p.G, p.B); }
        public static explicit operator RGB565Pixel(RGBPixel p) { return new RGB565Pixel(p.R, p.G, p.B); }
        public static explicit operator RGB565Pixel(Color p) { return new RGB565Pixel(p.R, p.G, p.B); }

        public static explicit operator RGB565Pixel(Vec3 v)
        {
            int r = Math.Max(Math.Min(Convert.ToInt32(v.X * 31.0f), 31), 0);
            int g = Math.Max(Math.Min(Convert.ToInt32(v.Y * 63.0f), 63), 0);
            int b = Math.Max(Math.Min(Convert.ToInt32(v.Z * 31.0f), 31), 0);
            return new RGB565Pixel((ushort)((r << 11) | (g << 5) | b));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RGB565Pixel))
                return false;
            RGB565Pixel p = (RGB565Pixel)obj;
            return _data._data == p._data._data;
        }

        public override int GetHashCode()
        {
            return _data;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGB5A3Pixel
    {
        public bushort _data;

        public RGB5A3Pixel(int a, int r, int g, int b)
        {
            a = Convert.ToInt32(a * (7.0 / 255.0));
            if (a == 7)
            {
                r = Convert.ToInt32(r * (31.0 / 255.0));
                g = Convert.ToInt32(g * (31.0 / 255.0));
                b = Convert.ToInt32(b * (31.0 / 255.0));
                _data = (ushort)((1 << 15) | (r << 10) | (g << 5) | b);
            }
            else
            {
                r = Convert.ToInt32(r * (15.0 / 255.0));
                g = Convert.ToInt32(g * (15.0 / 255.0));
                b = Convert.ToInt32(b * (15.0 / 255.0));
                _data = (ushort)((a << 12) | (r << 8) | (g << 4) | b);
            }
        }

        public static explicit operator ARGBPixel(RGB5A3Pixel p)
        {
            int a, r, g, b;
            ushort val = p._data;
            if ((val & 0x8000) != 0)
            {
                a = 0xFF;
                r = (val >> 10) & 0x1F;
                r = Convert.ToInt32(r * (255.0 / 31.0));
                g = (val >> 5) & 0x1F;
                g = Convert.ToInt32(g * (255.0 / 31.0));
                b = val & 0x1F;
                b = Convert.ToInt32(b * (255.0 / 31.0));
            }
            else
            {
                a = (val >> 12) & 0x07;
                a = Convert.ToInt32(a * (255.0 / 7.0));
                r = val & 0xF00;
                r = (r >> 4) | (r >> 8);
                g = val & 0xF0;
                g |= (g >> 4);
                b = val & 0x0F;
                b |= (b << 4);
            }
            return new ARGBPixel() { A = (byte)a, R = (byte)r, G = (byte)g, B = (byte)b };
        }
        public static explicit operator RGB5A3Pixel(ARGBPixel p)
        {
            return new RGB5A3Pixel(p.A, p.R, p.G, p.B);
        }

        public static explicit operator Color(RGB5A3Pixel p)
        {
            int a, r, g, b;
            ushort val = p._data;
            if ((val & 0x8000) != 0)
            {
                a = 0xFF;
                r = (val >> 10) & 0x1F;
                r = Convert.ToInt32(r * (255.0 / 31.0));
                g = (val >> 5) & 0x1F;
                g = Convert.ToInt32(g * (255.0 / 31.0));
                b = val & 0x1F;
                b = Convert.ToInt32(b * (255.0 / 31.0));
            }
            else
            {
                a = (val >> 12) & 0x07;
                a = Convert.ToInt32(a * (255.0 / 7.0));
                r = val & 0xF00;
                r = (r >> 4) | (r >> 8);
                g = val & 0xF0;
                g |= (g >> 4);
                b = val & 0x0F;
                b |= (b << 4);
            }
            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }
        public static explicit operator RGB5A3Pixel(Color p)
        {
            return new RGB5A3Pixel(p.A, p.R, p.G, p.B);
        }
    }
}
