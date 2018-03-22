using System;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models.Materials
{
    public class ShaderBVec3 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._bvec3;
        [Category(CategoryName)]
        public BoolVec3 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetShaderValueString()
              => string.Format("bvec3({0}, {1}, {2})",
                  _value.X.ToString().ToLowerInvariant(),
                  _value.Y.ToString().ToLowerInvariant(),
                  _value.Z.ToString().ToLowerInvariant());
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsXmlElementString = true)]
        private BoolVec3 _value;

        public ShaderBVec3() : this(new BoolVec3(), NoName) { }
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
    public class ShaderVec3 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._vec3;
        [Category(CategoryName)]
        public Vec3 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetShaderValueString()
             => $"vec3({_value.X:.0######}f, {_value.Y:.0######}f, {_value.Z:.0######}f)";
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsXmlElementString = true)]
        private Vec3 _value;

        public ShaderVec3() : this(new Vec3(), NoName) { }
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
    public class ShaderDVec3 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._dvec3;
        [Category(CategoryName)]
        public DVec3 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location) 
            => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetShaderValueString()
            => $"dvec3({_value.X:.0######}, {_value.Y:.0######}, {_value.Z:.0######})";
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsXmlElementString = true)]
        private DVec3 _value;

        public ShaderDVec3() : this(new DVec3(), NoName) { }
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
    public class ShaderIVec3 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._ivec3;
        [Category(CategoryName)]
        public IVec3 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetShaderValueString()
        => string.Format("ivec3({0}, {1}, {2})",
            _value.X.ToString(),
            _value.Y.ToString(),
            _value.Z.ToString());
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsXmlElementString = true)]
        private IVec3 _value;

        public ShaderIVec3() : this(new IVec3(), NoName) { }
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
    public class ShaderUVec3 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._uvec3;
        [Category(CategoryName)]
        public UVec3 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetShaderValueString()
            => string.Format("uvec3({0}, {1}, {2})",
              _value.X.ToString(),
              _value.Y.ToString(),
              _value.Z.ToString());
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsXmlElementString = true)]
        private UVec3 _value;

        public ShaderUVec3() : this(new UVec3(), NoName) { }
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
