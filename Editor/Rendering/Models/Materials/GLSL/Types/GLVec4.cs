using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLBVec4 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._bvec4; } }
        public BVec4 Value { get { return _value; } set { _value = value; } }
        
        private BVec4 _value;

        public GLBVec4(BVec4 defaultValue, string name, IGLVarOwner owner) 
            : base(name, owner) { _value = defaultValue; }
    }
    public class GLVec4 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._vec4; } }
        public Vec4 Value { get { return _value; } set { _value = value; } }

        private Vec4 _value;

        public GLVec4(Vec4 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner) { _value = defaultValue; }
    }
    public class GLDVec4 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._dvec4; } }
        public DVec4 Value { get { return _value; } set { _value = value; } }

        private DVec4 _value;

        public GLDVec4(DVec4 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner) { _value = defaultValue; }
    }
    public class GLIVec4 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._ivec4; } }
        public IVec4 Value { get { return _value; } set { _value = value; } }

        private IVec4 _value;

        public GLIVec4(IVec4 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner) { _value = defaultValue; }
    }
    public class GLUVec4 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._uvec4; } }
        public UVec4 Value { get { return _value; } set { _value = value; } }

        private UVec4 _value;

        public GLUVec4(UVec4 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner) { _value = defaultValue; }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BVec4 : IUniformable4Bool
    {
        public bool X { get { return _x; } set { _x = value; } }
        public bool Y { get { return _y; } set { _y = value; } }
        public bool Z { get { return _z; } set { _z = value; } }
        public bool W { get { return _w; } set { _w = value; } }

        private bool _x, _y, _z, _w;
        public unsafe bool* Data { get { fixed (void* ptr = &this) return (bool*)ptr; } }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IVec4 : IUniformable4Int
    {
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int Z { get { return _z; } set { _z = value; } }
        public int W { get { return _w; } set { _w = value; } }

        private int _x, _y, _z, _w;
        public unsafe int* Data { get { fixed (void* ptr = &this) return (int*)ptr; } }
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
    }
}
