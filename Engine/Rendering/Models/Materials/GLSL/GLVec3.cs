using System;

namespace TheraEngine.Rendering.Models.Materials
{
    public class GLBVec3 : ShaderVar
    {
        public override ShaderType TypeName { get { return ShaderType._bvec3; } }
        public BoolVec3 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private BoolVec3 _value;

        public GLBVec3(BoolVec3 defaultValue, string name, IShaderVarOwner owner) 
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLBool(defaultValue.X, "X", this));
            _fields.Add("y", new GLBool(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLBool(defaultValue.Z, "Z", this));
        }
    }
    public class GLVec3 : ShaderVar
    {
        public override ShaderType TypeName { get { return ShaderType._vec3; } }
        public Vec3 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private Vec3 _value;

        public GLVec3(Vec3 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLFloat(defaultValue.X, "X", this));
            _fields.Add("y", new GLFloat(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLFloat(defaultValue.Z, "Z", this));
        }
    }
    public class GLDVec3 : ShaderVar
    {
        public override ShaderType TypeName { get { return ShaderType._dvec3; } }
        public DVec3 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private DVec3 _value;

        public GLDVec3(DVec3 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLDouble(defaultValue.X, "X", this));
            _fields.Add("y", new GLDouble(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLDouble(defaultValue.Z, "Z", this));
        }
    }
    public class GLIVec3 : ShaderVar
    {
        public override ShaderType TypeName { get { return ShaderType._ivec3; } }
        public IVec3 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private IVec3 _value;

        public GLIVec3(IVec3 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLDouble(defaultValue.X, "X", this));
            _fields.Add("y", new GLDouble(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLDouble(defaultValue.Z, "Z", this));
        }
    }
    public class GLUVec3 : ShaderVar
    {
        public override ShaderType TypeName { get { return ShaderType._uvec3; } }
        public UVec3 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private UVec3 _value;

        public GLUVec3(UVec3 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new GLUInt(defaultValue.X, "X", this));
            _fields.Add("y", new GLUInt(defaultValue.Y, "Y", this));
            _fields.Add("z", new GLUInt(defaultValue.Z, "Z", this));
        }
    }
}
