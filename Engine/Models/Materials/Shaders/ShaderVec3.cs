using Extensions;
using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.ComponentModel;
using TheraEngine.Core;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.Models.Materials
{
    public class ShaderBVec3 : ShaderVar
    {
        [Browsable(false)]
        public override EShaderVarType TypeName => EShaderVarType._bvec3;
        [Category(CategoryName)]
        public BoolVec3 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(RenderProgram program, int location)
            => program.Uniform(location, _value);
        internal override string GetShaderValueString()
              => string.Format("bvec3({0}, {1}, {2})",
                  _value.X.ToString().ToLowerInvariant(),
                  _value.Y.ToString().ToLowerInvariant(),
                  _value.Z.ToString().ToLowerInvariant());
        [Browsable(false)]
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsElementString = true)]
        private BoolVec3 _value;

        public ShaderBVec3() : this(new BoolVec3(), NoName) { }
        public ShaderBVec3(BoolVec3 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderBVec3(BoolVec3 defaultValue, string name, IShaderVarOwner owner) 
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderBool(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderBool(defaultValue.Y, "Y", this));
            _fields.Add(".z", new ShaderBool(defaultValue.Z, "Z", this));
        }
    }
    public class ShaderVec3 : ShaderVar, IByteColor
    {
        [Browsable(false)]
        public override EShaderVarType TypeName => EShaderVarType._vec3;
        [Category(CategoryName)]
        public Vec3 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(RenderProgram program, int location)
            => program.Uniform(location, _value);
        internal override string GetShaderValueString()
             => $"vec3({_value.X:0.0######}f, {_value.Y:0.0######}f, {_value.Z:0.0######}f)";
        [Browsable(false)]
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsElementString = true)]
        private Vec3 _value;

        public ShaderVec3() : this(new Vec3(), NoName) { }
        public ShaderVec3(Vec3 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderVec3(Vec3 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderFloat(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderFloat(defaultValue.Y, "Y", this));
            _fields.Add(".z", new ShaderFloat(defaultValue.Z, "Z", this));
        }

        public Color Color { get => (Color)this; set => Value = (Vec3)value; }

        public ShaderVec3(float x, float y, float z)
            : this(x, y, z, NoName) { }
        public ShaderVec3(float x, float y, float z, string name)
            : this(x, y, z, name, null) { }
        public ShaderVec3(float x, float y, float z, string name, IShaderVarOwner owner)
            : this(new Vec3(x, y, z), name, owner) { }

        public static implicit operator ShaderVec3(Color p)
            => new ShaderVec3(p.R * THelpers.ByteToFloat, p.G * THelpers.ByteToFloat, p.B * THelpers.ByteToFloat);
        public static explicit operator Color(ShaderVec3 p)
            => Color.FromArgb(p.Value.X.ToByte(), p.Value.Y.ToByte(), p.Value.Z.ToByte());
    }
    public class ShaderDVec3 : ShaderVar
    {
        [Browsable(false)]
        public override EShaderVarType TypeName => EShaderVarType._dvec3;
        [Category(CategoryName)]
        public DVec3 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(RenderProgram program, int location)
            => program.Uniform(location, _value);
        internal override string GetShaderValueString()
            => $"dvec3({_value.X:0.0######}, {_value.Y:0.0######}, {_value.Z:0.0######})";
        [Browsable(false)]
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsElementString = true)]
        private DVec3 _value;

        public ShaderDVec3() : this(new DVec3(), NoName) { }
        public ShaderDVec3(DVec3 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderDVec3(DVec3 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderDouble(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderDouble(defaultValue.Y, "Y", this));
            _fields.Add(".z", new ShaderDouble(defaultValue.Z, "Z", this));
        }
    }
    public class ShaderIVec3 : ShaderVar
    {
        [Browsable(false)]
        public override EShaderVarType TypeName => EShaderVarType._ivec3;
        [Category(CategoryName)]
        public IVec3 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(RenderProgram program, int location)
            => program.Uniform(location, _value);
        internal override string GetShaderValueString()
        => string.Format("ivec3({0}, {1}, {2})",
            _value.X.ToString(),
            _value.Y.ToString(),
            _value.Z.ToString());
        [Browsable(false)]
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsElementString = true)]
        private IVec3 _value;

        public ShaderIVec3() : this(new IVec3(), NoName) { }
        public ShaderIVec3(IVec3 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderIVec3(IVec3 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderDouble(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderDouble(defaultValue.Y, "Y", this));
            _fields.Add(".z", new ShaderDouble(defaultValue.Z, "Z", this));
        }
    }
    public class ShaderUVec3 : ShaderVar
    {
        [Browsable(false)]
        public override EShaderVarType TypeName => EShaderVarType._uvec3;
        [Category(CategoryName)]
        public UVec3 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(RenderProgram program, int location)
            => program.Uniform(location, _value);
        internal override string GetShaderValueString()
            => string.Format("uvec3({0}, {1}, {2})",
              _value.X.ToString(),
              _value.Y.ToString(),
              _value.Z.ToString());
        [Browsable(false)]
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsElementString = true)]
        private UVec3 _value;

        public ShaderUVec3() : this(new UVec3(), NoName) { }
        public ShaderUVec3(UVec3 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderUVec3(UVec3 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderUInt(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderUInt(defaultValue.Y, "Y", this));
            _fields.Add(".z", new ShaderUInt(defaultValue.Z, "Z", this));
        }
    }
}
