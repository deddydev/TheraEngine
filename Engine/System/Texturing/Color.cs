using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //[Editor(typeof(PropertyGridColorEditor), typeof(UITypeEditor))]
    public unsafe struct ColorF4 : IUniformable4Float, IBufferable
    {
        public float R, G, B, A;
        
        public ColorF4(float r, float g, float b, float a) { R = r; G = g; B = b; A = a; }

        public float* Data { get { return (float*)Address; } }
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Float; } }
        public int ComponentCount { get { return 4; } }
        bool IBufferable.Normalize { get { return false; } }
        public void Write(VoidPtr address) { this = *(ColorF4*)address; }
        public void Read(VoidPtr address) { *(ColorF4*)address = this; }

        public static implicit operator ColorF4(RGBAPixel p) { return new ColorF4() { A = p.A / 255.0f, B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF4(ARGBPixel p) { return new ColorF4() { A = p.A / 255.0f, B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF4(Color p) { return new ColorF4() { A = p.A / 255.0f, B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF4(Vec3 v) { return new ColorF4(v.X, v.Y, v.Z, 1.0f); }
        public static implicit operator ColorF4(Vec4 v) { return new ColorF4(v.X, v.Y, v.Z, v.W); }
        public static implicit operator Vec4(ColorF4 v) { return new Vec4(v.R, v.G, v.B, v.A); }
        public static implicit operator ColorF4(ColorF3 p) { return new ColorF4(p.R, p.G, p.B, 1.0f); }

        public bool Equals(ColorF4 other, float precision)
        {
            return
                Math.Abs(R - other.R) < precision &&
                Math.Abs(G - other.G) < precision &&
                Math.Abs(B - other.B) < precision &&
                Math.Abs(A - other.A) < precision;
        }

        public override string ToString()
        {
            return String.Format("[R:{0},G:{1},B:{2},A:{3}]", R, G, B, A);
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //[Editor(typeof(PropertyGridColorEditor), typeof(UITypeEditor))]
    public unsafe struct ColorF3 : IUniformable3Float, IBufferable
    {
        public float R, G, B;
        
        public ColorF3(float r, float g, float b) { R = r; G = g; B = b; }

        public float* Data { get { return (float*)Address; } }
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Float; } }
        public int ComponentCount { get { return 3; } }
        bool IBufferable.Normalize { get { return false; } }
        public void Write(VoidPtr address) { this = *(ColorF3*)address; }
        public void Read(VoidPtr address) { *(ColorF3*)address = this; }

        public static implicit operator ColorF3(RGBAPixel p) { return new ColorF3() { B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF3(ARGBPixel p) { return new ColorF3() { B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF3(Color p) { return new ColorF3() { B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF3(Vec3 v) { return new ColorF3(v.X, v.Y, v.Z); }
        public static implicit operator ColorF3(Vec4 v) { return new ColorF3(v.X, v.Y, v.Z); }
        public static implicit operator ColorF3(ColorF4 p) { return new ColorF3(p.R, p.G, p.B); }

        public override string ToString()
        {
            return String.Format("[R:{0},G:{1},B:{2}]", R, G, B);
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct RGBAPixel : IBufferable
    {
        public byte R, G, B, A;
        
        public RGBAPixel(byte r, byte g, byte b, byte a) { R = r; G = g; B = b; A = a; }

        public byte* Data { get { return (byte*)Address; } }
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Byte; } }
        public int ComponentCount { get { return 4; } }
        bool IBufferable.Normalize { get { return true; } }
        public void Write(VoidPtr address) { this = *(RGBAPixel*)address; }
        public void Read(VoidPtr address) { *(RGBAPixel*)address = this; }

        public static implicit operator RGBAPixel(ColorF4 p) { return new RGBAPixel() { A = (byte)(p.A * 255.0f), B = (byte)(p.B * 255.0f), G = (byte)(p.G * 255.0f), R = (byte)(p.R * 255.0f) }; }
        public static implicit operator RGBAPixel(ARGBPixel p) { return new RGBAPixel() { A = p.A, B = p.B, G = p.G, R = p.R }; }
        public static implicit operator RGBAPixel(Color p) { return new RGBAPixel() { A = p.A, B = p.B, G = p.G, R = p.R }; }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct RGBPixel : IBufferable
    {
        public byte R, G, B;

        public RGBPixel(byte r, byte g, byte b) { R = r; G = g; B = b; }

        public byte* Data { get { return (byte*)Address; } }
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Byte; } }
        public int ComponentCount { get { return 3; } }
        bool IBufferable.Normalize { get { return true; } }
        public void Write(VoidPtr address) { this = *(RGBPixel*)address; }
        public void Read(VoidPtr address) { *(RGBPixel*)address = this; }

        public static implicit operator RGBPixel(ColorF4 p) { return new RGBPixel() { B = (byte)(p.B * 255.0f), G = (byte)(p.G * 255.0f), R = (byte)(p.R * 255.0f) }; }
        public static implicit operator RGBPixel(ARGBPixel p) { return new RGBPixel() { B = p.B, G = p.G, R = p.R }; }
        public static implicit operator RGBPixel(Color p) { return new RGBPixel() { B = p.B, G = p.G, R = p.R }; }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ARGBPixel
    {
        public byte A, R, G, B;
        
        public ARGBPixel(byte a, byte r, byte g, byte b) { A = a; R = r; G = g; B = b; }

        public static implicit operator ARGBPixel(ColorF4 p) { return new ARGBPixel() { A = (byte)(p.A * 255.0f), B = (byte)(p.B * 255.0f), G = (byte)(p.G * 255.0f), R = (byte)(p.R * 255.0f) }; }
        public static implicit operator ARGBPixel(RGBAPixel p) { return new ARGBPixel() { A = p.A, B = p.B, G = p.G, R = p.R }; }
        public static implicit operator ARGBPixel(Color p) { return new ARGBPixel() { A = p.A, B = p.B, G = p.G, R = p.R }; }
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
                    case 0:
                        newPixel = new ARGBPixel(255, v, t, p);
                        break;
                    case 1:
                        newPixel = new ARGBPixel(255, q, v, p);
                        break;
                    case 2:
                        newPixel = new ARGBPixel(255, p, v, t);
                        break;
                    case 3:
                        newPixel = new ARGBPixel(255, p, q, v);
                        break;
                    case 4:
                        newPixel = new ARGBPixel(255, t, p, v);
                        break;
                    default:
                        newPixel = new ARGBPixel(255, v, p, q);
                        break;
                }
            }
            return newPixel;
        }
        public static explicit operator Color(HSVPixel p)
        {
            ARGBPixel np = (ARGBPixel)p;
            return Color.FromArgb(*(int*)&np);
        }
        public static explicit operator HSVPixel(Color c)
        {
            return (HSVPixel)(ARGBPixel)c;
        }
    }
}
