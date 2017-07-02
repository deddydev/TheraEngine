using TheraEngine;
using TheraEngine.Rendering.Models;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Globalization;

namespace System
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //[Editor(typeof(PropertyGridColorEditor), typeof(UITypeEditor))]
    public unsafe struct ColorF4 : IUniformable4Float, IBufferable
    {
        public float R, G, B, A;

#if EDITOR
        /// <summary>
        /// For editor use.
        /// </summary>
        public float Red { get => R; set => R = value; }
        /// <summary>
        /// For editor use.
        /// </summary>
        public float Green { get => G; set => G = value; }
        /// <summary>
        /// For editor use.
        /// </summary>
        public float Blue { get => B; set => B = value; }
        /// <summary>
        /// For editor use.
        /// </summary>
        public float Alpha { get => A; set => A = value; }
#endif

        public string HexCode
        {
            get => R.ToByte().ToString("X") + G.ToByte().ToString("X") + B.ToByte().ToString("X") + A.ToByte().ToString("X");
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

        public ColorF4(float r, float g, float b, float a) { R = r; G = g; B = b; A = a; }

        public float* Data { get { return (float*)Address; } }
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Float; } }
        public int ComponentCount { get { return 4; } }
        bool IBufferable.Normalize { get { return false; } }
        public void Write(VoidPtr address) { this = *(ColorF4*)address; }
        public void Read(VoidPtr address) { *(ColorF4*)address = this; }

        private const float ByteToFloat = 1.0f / 255.0f;

        public static implicit operator ColorF4(RGBAPixel p)
            => new ColorF4(p.R * ByteToFloat, p.G * ByteToFloat, p.B * ByteToFloat, p.A * ByteToFloat);
        public static implicit operator ColorF4(ARGBPixel p)
            => new ColorF4(p.R * ByteToFloat, p.G * ByteToFloat, p.B * ByteToFloat, p.A * ByteToFloat);
        public static implicit operator ColorF4(Color p)
            => new ColorF4(p.R * ByteToFloat, p.G * ByteToFloat, p.B * ByteToFloat, p.A * ByteToFloat);
        public static implicit operator Color(ColorF4 p)
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

    public class EventColorF3
    {
        public event Action RedChanged;
        public event Action GreenChanged;
        public event Action BlueChanged;
        public event FloatChange RedValueChanged;
        public event FloatChange GreenValueChanged;
        public event FloatChange BlueValueChanged;
        public event Action Changed;
        
        ColorF3 _raw;
        private float _oldR, _oldG, _oldB;

        public EventColorF3(ColorF3 rgb)
        {
            _raw = rgb;
        }

        public void SetRawNoUpdate(ColorF3 raw)
        {
            _raw = raw;
        }

        public ColorF3 Raw
        {
            get => _raw;
            set => _raw = value;
        }

        private void BeginUpdate()
        {
            _oldR = R;
            _oldG = G;
            _oldB = B;
        }
        private void EndUpdate()
        {
            bool changed = false;
            if (R != _oldR)
            {
                changed = true;
                RedChanged?.Invoke();
                RedValueChanged?.Invoke(R, _oldR);
            }
            if (G != _oldG)
            {
                changed = true;
                GreenChanged?.Invoke();
                GreenValueChanged?.Invoke(G, _oldG);
            }
            if (B != _oldB)
            {
                changed = true;
                BlueChanged?.Invoke();
                BlueValueChanged?.Invoke(B, _oldB);
            }
            if (changed)
                Changed?.Invoke();
        }

        public float R
        {
            get => _raw.R;
            set
            {
                BeginUpdate();
                _raw.R = value;
                EndUpdate();
            }
        }
        public float G
        {
            get => _raw.G;
            set
            {
                BeginUpdate();
                _raw.G = value;
                EndUpdate();
            }
        }
        public float B
        {
            get => _raw.B;
            set
            {
                BeginUpdate();
                _raw.B = value;
                EndUpdate();
            }
        }
        public string HexCode
        {
            get => _raw.HexCode;
            set
            {
                BeginUpdate();
                _raw.HexCode = value;
                EndUpdate();
            }
        }
        
        public static implicit operator ColorF3(EventColorF3 v) { return v._raw; }
        public static implicit operator EventColorF3(ColorF3 v) { return new EventColorF3(v); }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //[Editor(typeof(PropertyGridColorEditor), typeof(UITypeEditor))]
    public unsafe struct ColorF3 : IUniformable3Float, IBufferable
    {
        public float R, G, B;
        
        [Browsable(false)]
        public string HexCode
        {
            get => R.ToByte().ToString("X") + G.ToByte().ToString("X") + B.ToByte().ToString("X");
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

        [Browsable(false)]
        public float* Data => (float*)Address;
        [Browsable(false)]
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.Float;
        [Browsable(false)]
        public int ComponentCount => 3;
        [Browsable(false)]
        bool IBufferable.Normalize => false;

        public void Write(VoidPtr address) { this = *(ColorF3*)address; }
        public void Read(VoidPtr address) { *(ColorF3*)address = this; }

        public static implicit operator ColorF3(RGBAPixel p) { return new ColorF3() { B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF3(ARGBPixel p) { return new ColorF3() { B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF3(Color p) { return new ColorF3() { B = p.B / 255.0f, G = p.G / 255.0f, R = p.R / 255.0f }; }
        public static implicit operator ColorF3(Vec3 v) { return new ColorF3(v.X, v.Y, v.Z); }
        public static implicit operator Vec3(ColorF3 v) { return new Vec3(v.R, v.G, v.B); }
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
