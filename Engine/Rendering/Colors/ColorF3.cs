using Extensions;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;

namespace TheraEngine.Rendering.Models.Materials
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ColorF3 : IUniformable3Float, IBufferable, IByteColor, ISerializableString
    {
        public float R, G, B;
        
        [Browsable(false)]
        public string HexCode
        {
            get => R.ToByte().ToString("X2") + G.ToByte().ToString("X2") + B.ToByte().ToString("X2");
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
                        }
                    }
                }
            }
        }

        public ColorF3(float r, float g, float b) { R = r; G = g; B = b; }
        public ColorF3(string s)
        {
            R = G = B = 0.0f;

            char[] delims = new char[] { ',', 'R', 'G', 'B', ':', ' ', '[', ']' };
            string[] arr = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length >= 3)
            {
                float.TryParse(arr[0], NumberStyles.Any, CultureInfo.InvariantCulture, out R);
                float.TryParse(arr[1], NumberStyles.Any, CultureInfo.InvariantCulture, out G);
                float.TryParse(arr[2], NumberStyles.Any, CultureInfo.InvariantCulture, out B);
            }
        }

        [Browsable(false)]
        public float* Data => (float*)Address;
        [Browsable(false)]
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        [Browsable(false)]
        public DataBuffer.EComponentType ComponentType => DataBuffer.EComponentType.Float;
        [Browsable(false)]
        public int ComponentCount => 3;
        [Browsable(false)]
        bool IBufferable.Normalize => false;

        public void Write(VoidPtr address) { *(ColorF3*)address = this; }
        public void Read(VoidPtr address) { this = *(ColorF3*)address; }

        private const float ByteToFloat = 1.0f / 255.0f;

        public static readonly ColorF3 Red = new ColorF3(1.0f, 0.0f, 0.0f);
        public static readonly ColorF3 Green = new ColorF3(0.0f, 1.0f, 0.0f);
        public static readonly ColorF3 Blue = new ColorF3(0.0f, 0.0f, 1.0f);
        public static readonly ColorF3 White = new ColorF3(1.0f, 1.0f, 1.0f);
        public static readonly ColorF3 Black = new ColorF3(0.0f, 0.0f, 0.0f);

        public static implicit operator ColorF3(RGBAPixel p) => new ColorF3(p.R * ByteToFloat, p.G * ByteToFloat, p.B * ByteToFloat);
        public static implicit operator ColorF3(ARGBPixel p) => new ColorF3(p.R * ByteToFloat, p.G * ByteToFloat, p.B * ByteToFloat);
        public static explicit operator ColorF3(Vec4 v) => new ColorF3(v.X, v.Y, v.Z);
        public static explicit operator ColorF3(ColorF4 p) => new ColorF3(p.R, p.G, p.B);

        public static implicit operator ColorF3(Vec3 v) => new ColorF3(v.X, v.Y, v.Z);
        public static implicit operator Vec3(ColorF3 v) => new Vec3(v.R, v.G, v.B);
        public static explicit operator ColorF3(Color p) => new ColorF3(p.R * ByteToFloat, p.G * ByteToFloat, p.B * ByteToFloat);
        public static explicit operator Color(ColorF3 p) => Color.FromArgb(255, p.R.ToByte(), p.G.ToByte(), p.B.ToByte());

        public static ColorF3 operator -(ColorF3 left, ColorF3 right)
            => new ColorF3(
                left.R - right.R,
                left.G - right.G,
                left.B - right.B);
        public static ColorF3 operator +(ColorF3 left, ColorF3 right)
            => new ColorF3(
                left.R + right.R,
                left.G + right.G,
                left.B + right.B);
        public static ColorF3 operator *(ColorF3 left, float right)
            => new ColorF3(
                left.R * right,
                left.G * right,
                left.B * right);
        public static ColorF3 operator *(float left, ColorF3 right)
            => new ColorF3(
                left * right.R,
                left * right.G,
                left * right.B);

        public override string ToString()
        {
            return String.Format("[R:{0},G:{1},B:{2}]", R, G, B);
        }

        public string WriteToString()
        {
            return ToString();
        }

        public void ReadFromString(string str)
        {
            this = new ColorF3(str);
        }

        [Browsable(false)]
        public Color Color { get => (Color)this; set => this = (ColorF3)value; }
    }
}
