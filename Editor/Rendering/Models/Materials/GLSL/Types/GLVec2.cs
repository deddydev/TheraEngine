using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLBVec2 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._bvec2; } }
        public BVec2 Value { get { return _value; } set { _value = value; } }
        
        private BVec2 _value;

        public GLBVec2(BVec2 defaultValue, string name, IGLVarOwner owner) 
            : base(name, owner) { _value = defaultValue; }
    }
    public class GLVec2 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._vec2; } }
        public Vec2 Value { get { return _value; } set { _value = value; } }

        private Vec2 _value;

        public GLVec2(Vec2 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner) { _value = defaultValue; }
    }
    public class GLDVec2 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._dvec2; } }
        public DVec2 Value { get { return _value; } set { _value = value; } }

        private DVec2 _value;

        public GLDVec2(DVec2 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner) { _value = defaultValue; }
    }
    public class GLIVec2 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._ivec2; } }
        public IVec2 Value { get { return _value; } set { _value = value; } }

        private IVec2 _value;

        public GLIVec2(IVec2 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner) { _value = defaultValue; }
    }
    public class GLUVec2 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._uvec2; } }
        public UVec2 Value { get { return _value; } set { _value = value; } }

        private UVec2 _value;

        public GLUVec2(UVec2 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner) { _value = defaultValue; }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BVec2 : IUniformable2Bool
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
}
