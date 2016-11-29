using CustomEngine;
using System.Runtime.InteropServices;
using CustomEngine.Rendering.Models;

namespace System
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BoolVec2 : IUniformable2Bool
    {
        public bool X { get { return _x; } set { _x = value; } }
        public bool Y { get { return _y; } set { _y = value; } }

        private bool _x, _y;
        public unsafe bool* Data { get { fixed (void* ptr = &this) return (bool*)ptr; } }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IVec2 : IUniformable2Int
    {
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }

        private int _x, _y;
        public unsafe int* Data { get { fixed (void* ptr = &this) return (int*)ptr; } }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UVec2 : IUniformable2UInt
    {
        public uint X { get { return _x; } set { _x = value; } }
        public uint Y { get { return _y; } set { _y = value; } }

        private uint _x, _y;
        public unsafe uint* Data { get { fixed (void* ptr = &this) return (uint*)ptr; } }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DVec2 : IUniformable2Double
    {
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }

        private double _x, _y;
        public unsafe double* Data { get { fixed (void* ptr = &this) return (double*)ptr; } }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BoolVec3 : IUniformable3Bool
    {
        public bool X { get { return _x; } set { _x = value; } }
        public bool Y { get { return _y; } set { _y = value; } }
        public bool Z { get { return _z; } set { _z = value; } }

        private bool _x, _y, _z;
        public unsafe bool* Data { get { fixed (void* ptr = &this) return (bool*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Int; } }
        public int ComponentCount { get { return 4; } }
        public bool Normalize { get { return false; } }
        public unsafe void Write(VoidPtr address)
        {
            int* dPtr = (int*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i] ? 1 : 0;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IVec3 : IUniformable3Int
    {
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int Z { get { return _z; } set { _z = value; } }

        private int _x, _y, _z;
        public unsafe int* Data { get { fixed (void* ptr = &this) return (int*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Int; } }
        public int ComponentCount { get { return 3; } }
        public bool Normalize { get { return false; } }
        public unsafe void Write(VoidPtr address)
        {
            int* dPtr = (int*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UVec3 : IUniformable3UInt
    {
        public uint X { get { return _x; } set { _x = value; } }
        public uint Y { get { return _y; } set { _y = value; } }
        public uint Z { get { return _z; } set { _z = value; } }

        private uint _x, _y, _z;
        public unsafe uint* Data { get { fixed (void* ptr = &this) return (uint*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.UInt; } }
        public int ComponentCount { get { return 3; } }
        public bool Normalize { get { return false; } }
        public unsafe void Write(VoidPtr address)
        {
            uint* dPtr = (uint*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DVec3 : IUniformable3Double
    {
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }

        private double _x, _y, _z;
        public unsafe double* Data { get { fixed (void* ptr = &this) return (double*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Double; } }
        public int ComponentCount { get { return 3; } }
        public bool Normalize { get { return false; } }
        public unsafe void Write(VoidPtr address)
        {
            double* dPtr = (double*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BoolVec4 : IUniformable4Bool
    {
        public bool X { get { return _x; } set { _x = value; } }
        public bool Y { get { return _y; } set { _y = value; } }
        public bool Z { get { return _z; } set { _z = value; } }
        public bool W { get { return _w; } set { _w = value; } }

        private bool _x, _y, _z, _w;
        public unsafe bool* Data { get { fixed (void* ptr = &this) return (bool*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Int; } }
        public int ComponentCount { get { return 4; } }
        public bool Normalize { get { return false; } }
        public unsafe void Write(VoidPtr address)
        {
            int* dPtr = (int*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i] ? 1 : 0;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IVec4 : IUniformable4Int, IBufferable
    {
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int Z { get { return _z; } set { _z = value; } }
        public int W { get { return _w; } set { _w = value; } }

        private int _x, _y, _z, _w;
        public unsafe int* Data { get { fixed (void* ptr = &this) return (int*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Int; } }
        public int ComponentCount { get { return 4; } }
        public bool Normalize { get { return false; } }
        public unsafe void Write(VoidPtr address)
        {
            int* dPtr = (int*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }

        public unsafe int this[int index]
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UVec4 : IUniformable4UInt
    {
        public uint X { get { return _x; } set { _x = value; } }
        public uint Y { get { return _y; } set { _y = value; } }
        public uint Z { get { return _z; } set { _z = value; } }
        public uint W { get { return _w; } set { _w = value; } }

        private uint _x, _y, _z, _w;
        public unsafe uint* Data { get { fixed (void* ptr = &this) return (uint*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.UInt; } }
        public int ComponentCount { get { return 4; } }
        public bool Normalize { get { return false; } }
        public unsafe void Write(VoidPtr address)
        {
            uint* dPtr = (uint*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DVec4 : IUniformable4Double
    {
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }
        public double W { get { return _w; } set { _w = value; } }

        private double _x, _y, _z, _w;
        public unsafe double* Data { get { fixed (void* ptr = &this) return (double*)ptr; } }

        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Double; } }
        public int ComponentCount { get { return 4; } }
        public bool Normalize { get { return false; } }
        public unsafe void Write(VoidPtr address)
        {
            double* dPtr = (double*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *dPtr++ = Data[i];
        }
    }
}
