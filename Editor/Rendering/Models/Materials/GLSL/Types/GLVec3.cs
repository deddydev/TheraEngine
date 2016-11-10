using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLBVec3 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._bvec3; } }
        public BVec3 Value { get { return _value; } set { _value = value; } }
        public override IUniformable UniformValue { get { return _value; } }

        private BVec3 _value;

        public GLBVec3(BVec3 defaultValue, string name, IGLVarOwner owner) 
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLBool(defaultValue.X, "X", this));
            _fields.Add("y", new GLBool(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLBool(defaultValue.Z, "Z", this));
        }
    }
    public class GLVec3 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._vec3; } }
        public Vec3 Value { get { return _value; } set { _value = value; } }
        public override IUniformable UniformValue { get { return _value; } }

        private Vec3 _value;

        public GLVec3(Vec3 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLFloat(defaultValue.X, "X", this));
            _fields.Add("y", new GLFloat(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLFloat(defaultValue.Z, "Z", this));
        }
    }
    public class GLDVec3 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._dvec3; } }
        public DVec3 Value { get { return _value; } set { _value = value; } }
        public override IUniformable UniformValue { get { return _value; } }

        private DVec3 _value;

        public GLDVec3(DVec3 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLDouble(defaultValue.X, "X", this));
            _fields.Add("y", new GLDouble(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLDouble(defaultValue.Z, "Z", this));
        }
    }
    public class GLIVec3 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._ivec3; } }
        public IVec3 Value { get { return _value; } set { _value = value; } }
        public override IUniformable UniformValue { get { return _value; } }

        private IVec3 _value;

        public GLIVec3(IVec3 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLDouble(defaultValue.X, "X", this));
            _fields.Add("y", new GLDouble(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLDouble(defaultValue.Z, "Z", this));
        }
    }
    public class GLUVec3 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._uvec3; } }
        public UVec3 Value { get { return _value; } set { _value = value; } }
        public override IUniformable UniformValue { get { return _value; } }

        private UVec3 _value;

        public GLUVec3(UVec3 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLUInt(defaultValue.X, "X", this));
            _fields.Add("y", new GLUInt(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLUInt(defaultValue.Z, "Z", this));
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BVec3 : IUniformable3Bool
    {
        public bool X { get { return _x; } set { _x = value; } }
        public bool Y { get { return _y; } set { _y = value; } }
        public bool Z { get { return _z; } set { _z = value; } }

        private bool _x, _y, _z;
        public unsafe bool* Data { get { fixed (void* ptr = &this) return (bool*)ptr; } }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IVec3 : IUniformable3Int
    {
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int Z { get { return _z; } set { _z = value; } }

        private int _x, _y, _z;
        public unsafe int* Data { get { fixed (void* ptr = &this) return (int*)ptr; } }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UVec3 : IUniformable3UInt
    {
        public uint X { get { return _x; } set { _x = value; } }
        public uint Y { get { return _y; } set { _y = value; } }
        public uint Z { get { return _z; } set { _z = value; } }

        private uint _x, _y, _z;
        public unsafe uint* Data { get { fixed (void* ptr = &this) return (uint*)ptr; } }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DVec3 : IUniformable3Double
    {
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }

        private double _x, _y, _z;
        public unsafe double* Data { get { fixed (void* ptr = &this) return (double*)ptr; } }
    }
}
