using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials
{
    public class GLBVec3 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._bvec3; } }
        public BoolVec3 Value { get { return _value; } set { _value = value; } }
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public override string GetValueString() { return _value.ToString(); }

        private BoolVec3 _value;

        public GLBVec3(BoolVec3 defaultValue, string name, IGLVarOwner owner) 
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
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public override string GetValueString() { return _value.ToString(); }

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
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public override string GetValueString() { return _value.ToString(); }

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
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public override string GetValueString() { return _value.ToString(); }

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
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public override string GetValueString() { return _value.ToString(); }

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
}
