using CustomEngine;
using System.Runtime.InteropServices;
using CustomEngine.Rendering.Models;

namespace System
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BoolVec2 : IUniformable2Bool, IBufferable
    {
        public bool X { get { return _x; } set { _x = value; } }
        public bool Y { get { return _y; } set { _y = value; } }

        private bool _x, _y;
        public bool* Data { get { fixed (void* ptr = &this) return (bool*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Byte; } }
        public int ComponentCount { get { return 2; } }
        public bool Normalize { get { return false; } }
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct IVec2 : IUniformable2Int, IBufferable
    {
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }

        private int _x, _y;
        public int* Data { get { fixed (void* ptr = &this) return (int*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Int; } }
        public int ComponentCount { get { return 2; } }
        public bool Normalize { get { return false; } }
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct UVec2 : IUniformable2UInt, IBufferable
    {
        public uint X { get { return _x; } set { _x = value; } }
        public uint Y { get { return _y; } set { _y = value; } }

        private uint _x, _y;
        public uint* Data { get { fixed (void* ptr = &this) return (uint*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.UInt; } }
        public int ComponentCount { get { return 2; } }
        public bool Normalize { get { return false; } }
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DVec2 : IUniformable2Double, IBufferable
    {
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }

        private double _x, _y;
        public double* Data { get { fixed (void* ptr = &this) return (double*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Double; } }
        public int ComponentCount { get { return 2; } }
        public bool Normalize { get { return false; } }
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BoolVec3 : IUniformable3Bool, IBufferable
    {
        public bool X { get { return _x; } set { _x = value; } }
        public bool Y { get { return _y; } set { _y = value; } }
        public bool Z { get { return _z; } set { _z = value; } }

        private bool _x, _y, _z;
        public bool* Data { get { fixed (void* ptr = &this) return (bool*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Byte; } }
        public int ComponentCount { get { return 4; } }
        public bool Normalize { get { return false; } }
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct IVec3 : IUniformable3Int, IBufferable
    {
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int Z { get { return _z; } set { _z = value; } }

        private int _x, _y, _z;
        public int* Data { get { fixed (void* ptr = &this) return (int*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Int; } }
        public int ComponentCount { get { return 3; } }
        public bool Normalize { get { return false; } }
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct UVec3 : IUniformable3UInt, IBufferable
    {
        public uint X { get { return _x; } set { _x = value; } }
        public uint Y { get { return _y; } set { _y = value; } }
        public uint Z { get { return _z; } set { _z = value; } }

        private uint _x, _y, _z;
        public uint* Data { get { fixed (void* ptr = &this) return (uint*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.UInt; } }
        public int ComponentCount { get { return 3; } }
        public bool Normalize { get { return false; } }
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DVec3 : IUniformable3Double, IBufferable
    {
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }

        private double _x, _y, _z;
        public double* Data { get { fixed (void* ptr = &this) return (double*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Double; } }
        public int ComponentCount { get { return 3; } }
        public bool Normalize { get { return false; } }
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BoolVec4 : IUniformable4Bool, IBufferable
    {
        public bool X { get { return _x; } set { _x = value; } }
        public bool Y { get { return _y; } set { _y = value; } }
        public bool Z { get { return _z; } set { _z = value; } }
        public bool W { get { return _w; } set { _w = value; } }

        private bool _x, _y, _z, _w;
        public bool* Data { get { fixed (void* ptr = &this) return (bool*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Byte; } }
        public int ComponentCount { get { return 4; } }
        public bool Normalize { get { return false; } }
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct IVec4 : IUniformable4Int, IBufferable
    {
        public IVec4(int x, int y, int z, int w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }

        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int Z { get { return _z; } set { _z = value; } }
        public int W { get { return _w; } set { _w = value; } }

        private int _x, _y, _z, _w;
        public int* Data { get { fixed (void* ptr = &this) return (int*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Int; } }
        public int ComponentCount { get { return 4; } }
        public bool Normalize { get { return false; } }
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct UVec4 : IUniformable4UInt, IBufferable
    {
        public uint X { get { return _x; } set { _x = value; } }
        public uint Y { get { return _y; } set { _y = value; } }
        public uint Z { get { return _z; } set { _z = value; } }
        public uint W { get { return _w; } set { _w = value; } }

        private uint _x, _y, _z, _w;
        public uint* Data { get { fixed (void* ptr = &this) return (uint*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.UInt; } }
        public int ComponentCount { get { return 4; } }
        public bool Normalize { get { return false; } }
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DVec4 : IUniformable4Double, IBufferable
    {
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }
        public double W { get { return _w; } set { _w = value; } }

        private double _x, _y, _z, _w;
        public double* Data { get { fixed (void* ptr = &this) return (double*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Double; } }
        public int ComponentCount { get { return 4; } }
        public bool Normalize { get { return false; } }
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
    }
}
