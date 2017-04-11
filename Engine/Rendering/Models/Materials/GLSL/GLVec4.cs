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
        public BoolVec4 Value { get { return _value; } set { _value = value; } }
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public override string GetValueString() { return _value.ToString(); }

        private BoolVec4 _value;

        public GLBVec4(BoolVec4 defaultValue, string name, IGLVarOwner owner) 
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLBool(defaultValue.X, "X", this));
            _fields.Add("y", new GLBool(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLBool(defaultValue.Z, "Z", this));
            _fields.Add("w", new GLBool(defaultValue.W, "W", this));
        }
    }
    public class GLVec4 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._vec4; } }
        public Vec4 Value { get { return _value; } set { _value = value; } }
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public override string GetValueString() { return _value.ToString(); }

        private Vec4 _value;

        public GLVec4(Vec4 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public GLVec4(Vec4 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLFloat(defaultValue.X, "X", this));
            _fields.Add("y", new GLFloat(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLFloat(defaultValue.Z, "Z", this));
            _fields.Add("w", new GLFloat(defaultValue.W, "W", this));
        }
    }
    public class GLDVec4 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._dvec4; } }
        public DVec4 Value { get { return _value; } set { _value = value; } }
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public override string GetValueString() { return _value.ToString(); }

        private DVec4 _value;

        public GLDVec4(DVec4 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLDouble(defaultValue.X, "X", this));
            _fields.Add("y", new GLDouble(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLDouble(defaultValue.Z, "Z", this));
            _fields.Add("w", new GLDouble(defaultValue.W, "W", this));
        }
    }
    public class GLIVec4 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._ivec4; } }
        public IVec4 Value { get { return _value; } set { _value = value; } }
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public override string GetValueString() { return _value.ToString(); }

        private IVec4 _value;

        public GLIVec4(IVec4 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLInt(defaultValue.X, "X", this));
            _fields.Add("y", new GLInt(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLInt(defaultValue.Z, "Z", this));
            _fields.Add("w", new GLInt(defaultValue.W, "W", this));
        }
    }
    public class GLUVec4 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._uvec4; } }
        public UVec4 Value { get { return _value; } set { _value = value; } }
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public override string GetValueString() { return _value.ToString(); }

        private UVec4 _value;

        public GLUVec4(UVec4 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLUInt(defaultValue.X, "X", this));
            _fields.Add("y", new GLUInt(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLUInt(defaultValue.Z, "Z", this));
            _fields.Add("w", new GLUInt(defaultValue.W, "W", this));
        }
    }
}
