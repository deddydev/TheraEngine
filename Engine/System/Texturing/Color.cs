﻿using System.Drawing;
using System.Runtime.InteropServices;

namespace System
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //[Editor(typeof(PropertyGridColorEditor), typeof(UITypeEditor))]
    public class ColorF4
    {
        public float R, G, B, A;

        public ColorF4() { }
        public ColorF4(float r, float g, float b, float a) { R = r; G = g; B = b; A = a; }

        public static implicit operator ColorF4(RGBAPixel p) { return new ColorF4() { A = p.A / 255.0f, B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF4(ARGBPixel p) { return new ColorF4() { A = p.A / 255.0f, B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF4(Color p) { return new ColorF4() { A = p.A / 255.0f, B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF4(Vector3 v) { return new ColorF4(v.X, v.Y, v.Z, 1.0f); }
        public static implicit operator ColorF4(Vector4 v) { return new ColorF4(v.X, v.Y, v.Z, v.W); }
        public static implicit operator ColorF4(ColorF3 p) { return new ColorF4(p.R, p.G, p.B, 1.0f); }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //[Editor(typeof(PropertyGridColorEditor), typeof(UITypeEditor))]
    public class ColorF3
    {
        public float R, G, B;

        public ColorF3() { }
        public ColorF3(float r, float g, float b) { R = r; G = g; B = b; }

        public static implicit operator ColorF3(RGBAPixel p) { return new ColorF3() { B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF3(ARGBPixel p) { return new ColorF3() { B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF3(Color p) { return new ColorF3() { B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF3(Vector3 v) { return new ColorF3(v.X, v.Y, v.Z); }
        public static implicit operator ColorF3(Vector4 v) { return new ColorF3(v.X, v.Y, v.Z); }
        public static implicit operator ColorF3(ColorF4 p) { return new ColorF3(p.R, p.G, p.B); }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGBAPixel
    {
        public byte R, G, B, A;
        
        public RGBAPixel(byte r, byte g, byte b, byte a) { R = r; G = g; B = b; A = a; }

        public static implicit operator RGBAPixel(ColorF4 p) { return new RGBAPixel() { A = (byte)(p.A * 255.0f), B = (byte)(p.B * 255.0f), G = (byte)(p.G * 255.0f), R = (byte)(p.R * 255.0f) }; }
        public static implicit operator RGBAPixel(ARGBPixel p) { return new RGBAPixel() { A = p.A, B = p.B, G = p.G, R = p.R }; }
        public static implicit operator RGBAPixel(Color p) { return new RGBAPixel() { A = p.A, B = p.B, G = p.G, R = p.R }; }
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
