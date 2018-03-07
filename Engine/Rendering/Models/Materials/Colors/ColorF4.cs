using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Globalization;
using System;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.Models.Materials
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ColorF4 : IUniformable4Float, IBufferable, IByteColor
    {
        public float R, G, B, A;
        
        [Browsable(false)]
        public string HexCode
        {
            get => R.ToByte().ToString("X2") + G.ToByte().ToString("X2") + B.ToByte().ToString("X2") + A.ToByte().ToString("X2");
            set
            {
                R = 0.0f;
                G = 0.0f;
                B = 0.0f;

                if (value.StartsWith("#"))
                    value = value.Substring(1);
                if (value.StartsWith("0x"))
                    value = value.Substring(2);

                if (value.Length >= 2)
                {
                    string r = value.Substring(0, 2);
                    byte.TryParse(r, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte rb);
                    R = rb / 255.0f;
                    if (value.Length >= 4)
                    {
                        string g = value.Substring(2, 2);
                        byte.TryParse(g, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte gb);
                        G = gb / 255.0f;
                        if (value.Length >= 6)
                        {
                            string b = value.Substring(4, 2);
                            byte.TryParse(b, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte bb);
                            B = bb / 255.0f;
                            if (value.Length >= 8)
                            {
                                string a = value.Substring(6, 2);
                                byte.TryParse(a, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte ab);
                                A = ab / 255.0f;
                            }
                        }

                    }
                }
            }
        }

        public ColorF4(float intensity) { R = G = B = intensity;  A = 1.0f; }
        public ColorF4(float intensity, float alpha) { R = G = B = intensity; A = alpha; }
        public ColorF4(float r, float g, float b) { R = r; G = g; B = b; A = 1.0f; }
        public ColorF4(float r, float g, float b, float a) { R = r; G = g; B = b; A = a; }
        public ColorF4(string s)
        {
            R = G = B = 0.0f;
            A = 1.0f;

            char[] delims = new char[] { ',', 'R', 'G', 'B', 'A', ':', ' ', '[', ']' };
            string[] arr = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length >= 4)
            {
                float.TryParse(arr[0], NumberStyles.Any, CultureInfo.InvariantCulture, out R);
                float.TryParse(arr[1], NumberStyles.Any, CultureInfo.InvariantCulture, out G);
                float.TryParse(arr[2], NumberStyles.Any, CultureInfo.InvariantCulture, out B);
                float.TryParse(arr[3], NumberStyles.Any, CultureInfo.InvariantCulture, out A);
            }
        }

        [Browsable(false)]
        public float* Data => (float*)Address;
        [Browsable(false)]
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.Float;
        [Browsable(false)]
        public int ComponentCount => 4;
        [Browsable(false)]
        bool IBufferable.Normalize => false;

        [Browsable(false)]
        public Color Color { get => (Color)this; set => this = value; }

        public void Write(VoidPtr address) { this = *(ColorF4*)address; }
        public void Read(VoidPtr address) { *(ColorF4*)address = this; }

        private const float ByteToFloat = 1.0f / 255.0f;

        public static implicit operator ColorF4(RGBAPixel p)
            => new ColorF4(p.R * ByteToFloat, p.G * ByteToFloat, p.B * ByteToFloat, p.A * ByteToFloat);
        public static implicit operator ColorF4(ARGBPixel p)
            => new ColorF4(p.R * ByteToFloat, p.G * ByteToFloat, p.B * ByteToFloat, p.A * ByteToFloat);
        public static implicit operator ColorF4(Color p)
            => new ColorF4(p.R * ByteToFloat, p.G * ByteToFloat, p.B * ByteToFloat, p.A * ByteToFloat);
        public static explicit operator Color(ColorF4 p)
            => Color.FromArgb(p.A.ToByte(), p.R.ToByte(), p.G.ToByte(), p.B.ToByte());
        public static implicit operator ColorF4(Vec3 v)
            => new ColorF4(v.X, v.Y, v.Z, 1.0f);
        public static implicit operator ColorF4(Vec4 v)
            => new ColorF4(v.X, v.Y, v.Z, v.W);
        public static implicit operator Vec4(ColorF4 v)
            => new Vec4(v.R, v.G, v.B, v.A);
        public static implicit operator ColorF4(ColorF3 p) 
            => new ColorF4(p.R, p.G, p.B, 1.0f);

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
}
