using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials
{
    public class GLBVec2 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._bvec2; } }
        public BoolVec2 Value { get { return _value; } set { _value = value; } }
        internal override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        internal override string GetValueString() { return _value.ToString(); }

        private BoolVec2 _value;

        public GLBVec2(BoolVec2 defaultValue, string name, IGLVarOwner owner) 
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new GLBool(defaultValue.X, "X", this));
            _fields.Add(".y", new GLBool(defaultValue.Y, "Y", this));
        }
    }
    public class GLVec2 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._vec2; } }
        public Vec2 Value { get { return _value; } set { _value = value; } }
        internal override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        internal override string GetValueString() { return _value.ToString(); }

        private Vec2 _value;

        public GLVec2(Vec2 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new GLFloat(defaultValue.X, "X", this));
            _fields.Add(".y", new GLFloat(defaultValue.Y, "Y", this));
        }
    }
    public class GLDVec2 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._dvec2; } }
        public DVec2 Value { get { return _value; } set { _value = value; } }
        internal override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        internal override string GetValueString() { return _value.ToString(); }

        private DVec2 _value;

        public GLDVec2(DVec2 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new GLDouble(defaultValue.X, "X", this));
            _fields.Add(".y", new GLDouble(defaultValue.Y, "Y", this));
        }
    }
    public class GLIVec2 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._ivec2; } }
        public IVec2 Value { get { return _value; } set { _value = value; } }
        internal override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        internal override string GetValueString() { return _value.ToString(); }

        private IVec2 _value;

        public GLIVec2(IVec2 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new GLInt(defaultValue.X, "X", this));
            _fields.Add(".y", new GLInt(defaultValue.Y, "Y", this));
        }
    }
    public class GLUVec2 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._uvec2; } }
        public UVec2 Value { get { return _value; } set { _value = value; } }
        internal override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        internal override string GetValueString() { return _value.ToString(); }

        private UVec2 _value;

        public GLUVec2(UVec2 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new GLUInt(defaultValue.X, "X", this));
            _fields.Add(".y", new GLUInt(defaultValue.Y, "Y", this));
        }
    }
}
