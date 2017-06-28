using TheraEngine;
using System.Runtime.InteropServices;
using TheraEngine.Rendering.Models;
using System.Drawing;
using System.ComponentModel;

namespace System
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BoolVec2 : IUniformable2Bool, IBufferable
    {
        public bool X { get => _x; set => _x = value; }
        public bool Y { get => _y; set => _y = value; }

        private bool _x, _y;
        public bool* Data { get { fixed (void* ptr = &this) return (bool*)ptr; } }

        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.Byte;
        [Browsable(false)]
        public int ComponentCount => 2;
        [Browsable(false)]
        public bool Normalize => false;

        public void Write(VoidPtr address)
        {
            byte* dPtr = (byte*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = (byte)(Data[i] ? 1 : 0);
        }
        public void Read(VoidPtr address)
        {
            byte* data = (byte*)address;
            for (int i = 0; i < ComponentCount; ++i)
                Data[i] = *data++ != 0;
        }

        public BoolVec2(bool x, bool y)
        {
            _x = x;
            _y = y;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct IVec2 : IUniformable2Int, IBufferable
    {
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }

        private int _x, _y;
        public int* Data { get { fixed (void* ptr = &this) return (int*)ptr; } }

        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.Int;
        [Browsable(false)]
        public int ComponentCount => 2;
        [Browsable(false)]
        public bool Normalize => false;

        public void Write(VoidPtr address)
        {
            int* dPtr = (int*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }
        public void Read(VoidPtr address)
        {
            int* data = (int*)address;
            for (int i = 0; i < ComponentCount; ++i)
                Data[i] = *data++;
        }

        public IVec2(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public static bool operator ==(IVec2 left, IVec2 right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(IVec2 left, IVec2 right)
        {
            if (ReferenceEquals(left, null))
                return !ReferenceEquals(right, null);
            return !left.Equals(right);
        }

        private static string listSeparator = Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        public override string ToString()
        {
            return String.Format("({0}{2} {1})", X, Y, listSeparator);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Vec2))
                return false;

            return Equals((Vec2)obj);
        }
        public bool Equals(Vec2 other)
        {
            return
                X == other.X &&
                Y == other.Y;
        }
        public static implicit operator Size(IVec2 v) => new Size(v.X, v.Y);
        public static implicit operator IVec2(Size v) => new IVec2(v.Width, v.Height);
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct UVec2 : IUniformable2UInt, IBufferable
    {
        public uint X { get => _x; set => _x = value; }
        public uint Y { get => _y; set => _y = value; }

        private uint _x, _y;
        public uint* Data { get { fixed (void* ptr = &this) return (uint*)ptr; } }

        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.UInt;
        [Browsable(false)]
        public int ComponentCount => 2;
        [Browsable(false)]
        public bool Normalize => false;

        public void Write(VoidPtr address)
        {
            uint* dPtr = (uint*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }
        public void Read(VoidPtr address)
        {
            uint* data = (uint*)address;
            for (int i = 0; i < ComponentCount; ++i)
                Data[i] = *data++;
        }

        public UVec2(uint x, uint y)
        {
            _x = x;
            _y = y;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DVec2 : IUniformable2Double, IBufferable
    {
        public double X { get => _x; set => _x = value; }
        public double Y { get => _y; set => _y = value; }

        private double _x, _y;
        public double* Data { get { fixed (void* ptr = &this) return (double*)ptr; } }

        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.Double;
        [Browsable(false)]
        public int ComponentCount => 2;
        [Browsable(false)]
        public bool Normalize => false;

        public void Write(VoidPtr address)
        {
            double* dPtr = (double*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }
        public void Read(VoidPtr address)
        {
            double* data = (double*)address;
            for (int i = 0; i < ComponentCount; ++i)
                Data[i] = *data++;
        }

        public DVec2(double x, double y)
        {
            _x = x;
            _y = y;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BoolVec3 : IUniformable3Bool, IBufferable
    {
        public bool X { get => _x; set => _x = value; }
        public bool Y { get => _y; set => _y = value; }
        public bool Z { get => _z; set => _z = value; }

        private bool _x, _y, _z;
        public bool* Data { get { fixed (void* ptr = &this) return (bool*)ptr; } }

        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.Byte;
        [Browsable(false)]
        public int ComponentCount => 3;
        [Browsable(false)]
        public bool Normalize => false;

        public void Write(VoidPtr address)
        {
            byte* dPtr = (byte*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = (byte)(Data[i] ? 1 : 0);
        }
        public void Read(VoidPtr address)
        {
            byte* data = (byte*)address;
            for (int i = 0; i < ComponentCount; ++i)
                Data[i] = *data++ != 0;
        }

        public BoolVec3(bool x, bool y, bool z)
        {
            _x = x;
            _y = y;
            _z = z;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct IVec3 : IUniformable3Int, IBufferable
    {
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }
        public int Z { get => _z; set => _z = value; }

        private int _x, _y, _z;
        public int* Data { get { fixed (void* ptr = &this) return (int*)ptr; } }

        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.Int;
        [Browsable(false)]
        public int ComponentCount => 3;
        [Browsable(false)]
        public bool Normalize => false;

        public void Write(VoidPtr address)
        {
            int* dPtr = (int*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }
        public void Read(VoidPtr address)
        {
            int* data = (int*)address;
            for (int i = 0; i < ComponentCount; ++i)
                Data[i] = *data++;
        }

        public IVec3(int x, int y, int z)
        {
            _x = x;
            _y = y;
            _z = z;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct UVec3 : IUniformable3UInt, IBufferable
    {
        public uint X { get => _x; set => _x = value; }
        public uint Y { get => _y; set => _y = value; }
        public uint Z { get => _z; set => _z = value; }

        private uint _x, _y, _z;
        public uint* Data { get { fixed (void* ptr = &this) return (uint*)ptr; } }

        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.UInt;
        [Browsable(false)]
        public int ComponentCount => 3;
        [Browsable(false)]
        public bool Normalize => false;

        public void Write(VoidPtr address)
        {
            uint* dPtr = (uint*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }
        public void Read(VoidPtr address)
        {
            uint* data = (uint*)address;
            for (int i = 0; i < ComponentCount; ++i)
                Data[i] = *data++;
        }

        public UVec3(uint x, uint y, uint z)
        {
            _x = x;
            _y = y;
            _z = z;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DVec3 : IUniformable3Double, IBufferable
    {
        public double X { get => _x; set => _x = value; }
        public double Y { get => _y; set => _y = value; }
        public double Z { get => _z; set => _z = value; }

        private double _x, _y, _z;
        public double* Data { get { fixed (void* ptr = &this) return (double*)ptr; } }

        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.Double;
        [Browsable(false)]
        public int ComponentCount => 3;
        [Browsable(false)]
        public bool Normalize => false;

        public void Write(VoidPtr address)
        {
            double* dPtr = (double*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }
        public void Read(VoidPtr address)
        {
            double* data = (double*)address;
            for (int i = 0; i < ComponentCount; ++i)
                Data[i] = *data++;
        }

        public DVec3(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BoolVec4 : IUniformable4Bool, IBufferable
    {
        public bool X { get => _x; set => _x = value; }
        public bool Y { get => _y; set => _y = value; }
        public bool Z { get => _z; set => _z = value; }
        public bool W { get => _w; set => _w = value; }

        private bool _x, _y, _z, _w;
        public bool* Data { get { fixed (void* ptr = &this) return (bool*)ptr; } }

        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.Byte;
        [Browsable(false)]
        public int ComponentCount => 4;
        [Browsable(false)]
        public bool Normalize => false;

        public void Write(VoidPtr address)
        {
            byte* dPtr = (byte*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = (byte)(Data[i] ? 1 : 0);
        }
        public void Read(VoidPtr address)
        {
            byte* data = (byte*)address;
            for (int i = 0; i < ComponentCount; ++i)
                Data[i] = *data++ != 0;
        }

        public bool this[int index]
        {
            get
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                return Data[index];
            }
            set
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                Data[index] = value;
            }
        }

        public BoolVec4(bool x, bool y, bool z, bool w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct IVec4 : IUniformable4Int, IBufferable
    {
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }
        public int Z { get => _z; set => _z = value; }
        public int W { get => _w; set => _w = value; }

        private int _x, _y, _z, _w;
        public int* Data { get { fixed (void* ptr = &this) return (int*)ptr; } }

        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.Int;
        [Browsable(false)]
        public int ComponentCount => 4;
        [Browsable(false)]
        public bool Normalize => false;

        public void Write(VoidPtr address)
        {
            int* dPtr = (int*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }
        public void Read(VoidPtr address)
        {
            int* data = (int*)address;
            for (int i = 0; i < ComponentCount; ++i)
                Data[i] = *data++;
        }

        public int this[int index]
        {
            get
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                return Data[index];
            }
            set
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                Data[index] = value;
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", X, Y, Z, W);
        }

        public IVec4(int x, int y, int z, int w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct UVec4 : IUniformable4UInt, IBufferable
    {
        public uint X { get => _x; set => _x = value; }
        public uint Y { get => _y; set => _y = value; }
        public uint Z { get => _z; set => _z = value; }
        public uint W { get => _w; set => _w = value; }

        private uint _x, _y, _z, _w;
        public uint* Data { get { fixed (void* ptr = &this) return (uint*)ptr; } }

        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.UInt;
        [Browsable(false)]
        public int ComponentCount => 4;
        [Browsable(false)]
        public bool Normalize => false;

        public void Write(VoidPtr address)
        {
            uint* dPtr = (uint*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }
        public void Read(VoidPtr address)
        {
            uint* data = (uint*)address;
            for (int i = 0; i < ComponentCount; ++i)
                Data[i] = *data++;
        }

        public uint this[int index]
        {
            get
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                return Data[index];
            }
            set
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                Data[index] = value;
            }
        }

        public UVec4(uint x, uint y, uint z, uint w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DVec4 : IUniformable4Double, IBufferable
    {
        public double X { get => _x; set => _x = value; }
        public double Y { get => _y; set => _y = value; }
        public double Z { get => _z; set => _z = value; }
        public double W { get => _w; set => _w = value; }

        private double _x, _y, _z, _w;
        public double* Data { get { fixed (void* ptr = &this) return (double*)ptr; } }

        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.Double;
        [Browsable(false)]
        public int ComponentCount => 4;
        [Browsable(false)]
        public bool Normalize => false;

        public void Write(VoidPtr address)
        {
            double* dPtr = (double*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }
        public void Read(VoidPtr address)
        {
            double* data = (double*)address;
            for (int i = 0; i < ComponentCount; ++i)
                Data[i] = *data++;
        }

        public double this[int index]
        {
            get
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                return Data[index];
            }
            set
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                Data[index] = value;
            }
        }

        public DVec4(double x, double y, double z, double w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }
    }
}
