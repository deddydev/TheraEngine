using System;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models.Materials
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderBVec3 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName { get { return ShaderVarType._bvec3; } }
        public BoolVec3 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private BoolVec3 _value;

        public ShaderBVec3(BoolVec3 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderBVec3(BoolVec3 defaultValue, string name, IShaderVarOwner owner) 
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new ShaderBool(defaultValue.X, "X", this));
            _fields.Add("y", new ShaderBool(defaultValue.Y, "Y", this));
            _fields.Add("z", new ShaderBool(defaultValue.Z, "Z", this));
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderVec3 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName { get { return ShaderVarType._vec3; } }
        public Vec3 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private Vec3 _value;

        public ShaderVec3(Vec3 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderVec3(Vec3 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new ShaderFloat(defaultValue.X, "X", this));
            _fields.Add("y", new ShaderFloat(defaultValue.Y, "Y", this));
            _fields.Add("z", new ShaderFloat(defaultValue.Z, "Z", this));
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderDVec3 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName { get { return ShaderVarType._dvec3; } }
        public DVec3 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private DVec3 _value;

        public ShaderDVec3(DVec3 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderDVec3(DVec3 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new ShaderDouble(defaultValue.X, "X", this));
            _fields.Add("y", new ShaderDouble(defaultValue.Y, "Y", this));
            _fields.Add("z", new ShaderDouble(defaultValue.Z, "Z", this));
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderIVec3 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName { get { return ShaderVarType._ivec3; } }
        public IVec3 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private IVec3 _value;

        public ShaderIVec3(IVec3 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderIVec3(IVec3 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new ShaderDouble(defaultValue.X, "X", this));
            _fields.Add("y", new ShaderDouble(defaultValue.Y, "Y", this));
            _fields.Add("z", new ShaderDouble(defaultValue.Z, "Z", this));
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderUVec3 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName { get { return ShaderVarType._uvec3; } }
        public UVec3 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private UVec3 _value;

        public ShaderUVec3(UVec3 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderUVec3(UVec3 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new ShaderUInt(defaultValue.X, "X", this));
            _fields.Add("y", new ShaderUInt(defaultValue.Y, "Y", this));
            _fields.Add("z", new ShaderUInt(defaultValue.Z, "Z", this));
        }
    }
}
