using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.Models.Materials
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct RGBAPixel : IBufferable, IByteColor
    {
        public byte R, G, B, A;
        
        public RGBAPixel(byte r, byte g, byte b, byte a) { R = r; G = g; B = b; A = a; }

        public byte* Data => (byte*)Address;
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        public DataBuffer.EComponentType ComponentType => DataBuffer.EComponentType.Byte;
        public int ComponentCount => 4;
        bool IBufferable.Normalize => true;
        public void Write(VoidPtr address) { this = *(RGBAPixel*)address; }
        public void Read(VoidPtr address) { *(RGBAPixel*)address = this; }

        public static explicit operator RGBAPixel(ColorF4 p) => new RGBAPixel(p.R.ToByte(), p.G.ToByte(), p.B.ToByte(), p.A.ToByte());
        public static implicit operator RGBAPixel(ARGBPixel p) => new RGBAPixel(p.R, p.G, p.B, p.A);
        public static implicit operator RGBAPixel(Color p) => new RGBAPixel(p.R, p.G, p.B, p.A);
        public static implicit operator Color(RGBAPixel p) => Color.FromArgb(p.A, p.R, p.G, p.B);

        [Browsable(false)]
        public Color Color { get => this; set => this = value; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct RGBPixel : IBufferable, IByteColor
    {
        public byte R, G, B;

        public RGBPixel(byte r, byte g, byte b) { R = r; G = g; B = b; }

        public byte* Data { get { return (byte*)Address; } }
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        public DataBuffer.EComponentType ComponentType { get { return DataBuffer.EComponentType.Byte; } }
        public int ComponentCount { get { return 3; } }
        bool IBufferable.Normalize { get { return true; } }
        public void Write(VoidPtr address) { this = *(RGBPixel*)address; }
        public void Read(VoidPtr address) { *(RGBPixel*)address = this; }

        public static explicit operator RGBPixel(ColorF3 p) => new RGBPixel(p.R.ToByte(), p.G.ToByte(), p.B.ToByte());
        public static explicit operator RGBPixel(ColorF4 p) => new RGBPixel(p.R.ToByte(), p.G.ToByte(), p.B.ToByte());
        public static explicit operator RGBPixel(ARGBPixel p) => new RGBPixel(p.R, p.G, p.B);
        public static explicit operator RGBPixel(Color p) => new RGBPixel(p.R, p.G, p.B);
        public static implicit operator Color(RGBPixel p) => Color.FromArgb(255, p.R, p.G, p.B);
        
        [Browsable(false)]
        public Color Color { get => this; set => this = (RGBPixel)value; }
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ARGBPixel
    {
        private const float ColorFactor = 1.0f / 255.0f;

        [Browsable(false)]
        public Color Color { get => this; set => this = value; }

        public byte A, R, G, B;

        public ARGBPixel(int argb)
        {
            A = (byte)((argb >> 24) & 0xFF);
            R = (byte)((argb >> 16) & 0xFF);
            G = (byte)((argb >> 8) & 0xFF);
            B = (byte)((argb) & 0xFF);
        }
        public ARGBPixel(uint argb)
        {
            A = (byte)((argb >> 24) & 0xFF);
            R = (byte)((argb >> 16) & 0xFF);
            G = (byte)((argb >> 8) & 0xFF);
            B = (byte)((argb) & 0xFF);
        }

        public ARGBPixel(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public int DistanceTo(ARGBPixel p)
        {
            int val = A - p.A;
            int dist = val * val;
            val = R - p.R;
            dist += val * val;
            val = G - p.G;
            dist += val * val;
            val = B - p.B;
            return dist + val;
        }

        public float Luminance()
            => (0.299f * R) + (0.587f * G) + (0.114f * B);
        public bool IsGreyscale()
            => R == G && G == B;
        public int Greyscale()
            => (R + G + B) / 3;

        public static implicit operator ARGBPixel(int val) => new ARGBPixel(val);
        public static implicit operator int(ARGBPixel p) => *((bint*)&p);
        public static implicit operator ARGBPixel(uint val) => new ARGBPixel(val);
        public static implicit operator uint(ARGBPixel p) => *((buint*)&p);
        public static implicit operator ARGBPixel(Color val) => val.ToArgb();
        public static implicit operator Color(ARGBPixel p) => Color.FromArgb(p);
        public static explicit operator Vec3(ARGBPixel p) => new Vec3(p.R * ColorFactor, p.G * ColorFactor, p.B * ColorFactor);
        public static implicit operator Vec4(ARGBPixel p) => new Vec4(p.R * ColorFactor, p.G * ColorFactor, p.B * ColorFactor, p.A * ColorFactor);
        public static explicit operator ARGBPixel(ColorF4 p) => new ARGBPixel(p.A.ToByte(), p.R.ToByte(), p.G.ToByte(), p.B.ToByte());
        public static implicit operator ARGBPixel(RGBAPixel p) => new ARGBPixel(p.A, p.R, p.G, p.B);

        public ARGBPixel Min(ARGBPixel p) { return new ARGBPixel(Math.Min(A, p.A), Math.Min(R, p.R), Math.Min(G, p.G), Math.Min(B, p.B)); }
        public ARGBPixel Max(ARGBPixel p) { return new ARGBPixel(Math.Max(A, p.A), Math.Max(R, p.R), Math.Max(G, p.G), Math.Max(B, p.B)); }

        public static bool operator ==(ARGBPixel p1, ARGBPixel p2) { return *((uint*)(&p1)) == *((uint*)(&p2)); }
        public static bool operator !=(ARGBPixel p1, ARGBPixel p2) { return *((uint*)(&p1)) != *((uint*)(&p2)); }

        public override string ToString()
        {
            return String.Format("A:{0} R:{1} G:{2} B:{3}", A, R, G, B);
        }
        public string ToHexString()
        {
            return String.Format("A:{0:X2} R:{1:X2} G:{2:X2} B:{3:X2}", A, R, G, B);
        }
        public string ToPaddedString()
        {
            return String.Format("A:{0,3} R:{1,3} G:{2,3} B:{3,3}", A, R, G, B);
        }
        public string ToARGBColorCode()
        {
            return String.Format("{0:X2}{1:X2}{2:X2}{3:X2}", A, R, G, B);
        }
        public string ToRGBAColorCode()
        {
            return String.Format("{0:X2}{1:X2}{2:X2}{3:X2}", R, G, B, A);
        }
        public override int GetHashCode() { return (int)this; }
        public override bool Equals(object obj)
        {
            if (obj is ARGBPixel) return (ARGBPixel)obj == this;
            return false;
        }

        public unsafe ARGBPixel Inverse()
        {
            return new ARGBPixel(A, (byte)(255 - R), (byte)(255 - G), (byte)(255 - B));
        }
        public unsafe ARGBPixel Lighten(int amount)
        {
            return new ARGBPixel(A, (byte)Math.Min(R + amount, 255), (byte)Math.Min(G + amount, 255), (byte)Math.Min(B + amount, 255));
        }
        public unsafe ARGBPixel Darken(int amount)
        {
            return new ARGBPixel(A, (byte)Math.Max(R - amount, 0), (byte)Math.Max(G - amount, 0), (byte)Math.Max(B - amount, 0));
        }
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct HSVPixel
    {
        public ushort H;
        public byte S, V;

        public HSVPixel(ushort h, byte s, byte v) { H = h; S = s; V = v; }

        public static explicit operator HSVPixel(ARGBPixel p)
        {
            HSVPixel outp;

            int min = Math.Min(Math.Min(p.R, p.G), p.B);
            int max = Math.Max(Math.Max(p.R, p.G), p.B);
            int diff = max - min;

            if (diff == 0)
            {
                outp.H = 0;
                outp.S = 0;
            }
            else
            {
                if (max == p.R)
                    outp.H = (ushort)((60 * ((float)(p.G - p.B) / diff) + 360) % 360);
                else if (max == p.G)
                    outp.H = (ushort)(60 * ((float)(p.B - p.R) / diff) + 120);
                else
                    outp.H = (ushort)(60 * ((float)(p.R - p.G) / diff) + 240);

                if (max == 0)
                    outp.S = 0;
                else
                    outp.S = (byte)(diff * 100 / max);
            }

            outp.V = (byte)(max * 100 / 255);

            return outp;
        }
        public static explicit operator ARGBPixel(HSVPixel pixel)
        {
            ARGBPixel newPixel;

            byte v = (byte)(pixel.V * 255 / 100);
            if (pixel.S == 0)
                newPixel = new ARGBPixel(255, v, v, v);
            else
            {
                int h = (pixel.H / 60) % 6;
                float f = (pixel.H / 60.0f) - (pixel.H / 60);

                byte p = (byte)(pixel.V * (100 - pixel.S) * 255 / 10000);
                byte q = (byte)(pixel.V * (100 - (int)(f * pixel.S)) * 255 / 10000);
                byte t = (byte)(pixel.V * (100 - (int)((1.0f - f) * pixel.S)) * 255 / 10000);

                switch (h)
                {
                    case 0: newPixel = new ARGBPixel(255, v, t, p); break;
                    case 1: newPixel = new ARGBPixel(255, q, v, p); break;
                    case 2: newPixel = new ARGBPixel(255, p, v, t); break;
                    case 3: newPixel = new ARGBPixel(255, p, q, v); break;
                    case 4: newPixel = new ARGBPixel(255, t, p, v); break;
                    default: newPixel = new ARGBPixel(255, v, p, q); break;
                }
            }
            return newPixel;
        }
        public static explicit operator Color(HSVPixel p)
        {
            ARGBPixel np = (ARGBPixel)p;
            return Color.FromArgb(np);
        }
        public static explicit operator HSVPixel(Color c)
        {
            return (HSVPixel)(ARGBPixel)c;
        }
    }
}
