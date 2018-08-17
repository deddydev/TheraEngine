using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Core;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.Models.Materials
{
    public class ShaderBVec4 : ShaderVar
    {
        [Browsable(false)]
        public override EShaderVarType TypeName => EShaderVarType._bvec4;
        [Category(CategoryName)]
        public BoolVec4 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(RenderProgram program, int location)
            => program.Uniform(location, _value);
        internal override string GetShaderValueString()
            => string.Format("bvec4({0}, {1}, {2}, {3})",
                _value.X.ToString().ToLowerInvariant(),
                _value.Y.ToString().ToLowerInvariant(),
                _value.Z.ToString().ToLowerInvariant(),
                _value.W.ToString().ToLowerInvariant());
        [Browsable(false)]
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsXmlElementString = true)]
        private BoolVec4 _value;

        public ShaderBVec4() : this(new BoolVec4(), NoName) { }
        public ShaderBVec4(BoolVec4 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderBVec4(BoolVec4 defaultValue, string name, IShaderVarOwner owner) 
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderBool(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderBool(defaultValue.Y, "Y", this));
            _fields.Add(".z", new ShaderBool(defaultValue.Z, "Z", this));
            _fields.Add(".w", new ShaderBool(defaultValue.W, "W", this));
        }
    }
    public class ShaderVec4 : ShaderVar, IByteColor
    {
        [Browsable(false)]
        public override EShaderVarType TypeName => EShaderVarType._vec4;
        [Category(CategoryName)]
        public Vec4 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(RenderProgram program, int location) 
            => program.Uniform(location, _value);
        internal override string GetShaderValueString()
            => $"vec4({_value.X:0.0######}f, {_value.Y:0.0######}f, {_value.Z:0.0######}f, {_value.W:0.0######}f)";
        [Browsable(false)]
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsXmlElementString = true)]
        private Vec4 _value;

        public ShaderVec4() : this(new Vec4(), NoName) { }
        public ShaderVec4(Vec4 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderVec4(Vec4 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderFloat(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderFloat(defaultValue.Y, "Y", this));
            _fields.Add(".z", new ShaderFloat(defaultValue.Z, "Z", this));
            _fields.Add(".w", new ShaderFloat(defaultValue.W, "W", this));
        }

        public Color Color { get => (Color)this; set => Value = value; }

        public ShaderVec4(float x, float y, float z, float w)
            : this(x, y, z, w, NoName) { }
        public ShaderVec4(float x, float y, float z, float w, string name)
            : this(x, y, z, w, name, null) { }
        public ShaderVec4(float x, float y, float z, float w, string name, IShaderVarOwner owner)
            : this(new Vec4(x, y, z, w), name, owner) { }

        public static implicit operator ShaderVec4(Color p)
            => new ShaderVec4(p.R * THelpers.ByteToFloat, p.G * THelpers.ByteToFloat, p.B * THelpers.ByteToFloat, p.A * THelpers.ByteToFloat);
        public static explicit operator Color(ShaderVec4 p)
            => Color.FromArgb(p.Value.W.ToByte(), p.Value.X.ToByte(), p.Value.Y.ToByte(), p.Value.Z.ToByte());
    }
    public class ShaderDVec4 : ShaderVar
    {
        [Browsable(false)]
        public override EShaderVarType TypeName => EShaderVarType._dvec4;
        [Category(CategoryName)]
        public DVec4 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(RenderProgram program, int location)
            => program.Uniform(location, _value);
        internal override string GetShaderValueString()
            => $"dvec4({_value.X:0.0######}, {_value.Y:0.0######}, {_value.Z:0.0######}, {_value.W:0.0######})";
        [Browsable(false)]
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsXmlElementString = true)]
        private DVec4 _value;

        public ShaderDVec4() : this(new DVec4(), NoName) { }
        public ShaderDVec4(DVec4 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderDVec4(DVec4 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderDouble(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderDouble(defaultValue.Y, "Y", this));
            _fields.Add(".z", new ShaderDouble(defaultValue.Z, "Z", this));
            _fields.Add(".w", new ShaderDouble(defaultValue.W, "W", this));
        }
    }
    public class ShaderIVec4 : ShaderVar
    {
        [Browsable(false)]
        public override EShaderVarType TypeName => EShaderVarType._ivec4;
        [Category(CategoryName)]
        public IVec4 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(RenderProgram program, int location) 
            => program.Uniform(location, _value);
        internal override string GetShaderValueString()
          => string.Format("ivec4({0}, {1}, {2}, {3})",
              _value.X.ToString(),
              _value.Y.ToString(),
              _value.Z.ToString(),
              _value.W.ToString());
        [Browsable(false)]
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsXmlElementString = true)]
        private IVec4 _value;

        public ShaderIVec4() : this(new IVec4(), NoName) { }
        public ShaderIVec4(IVec4 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderIVec4(IVec4 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderInt(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderInt(defaultValue.Y, "Y", this));
            _fields.Add(".z", new ShaderInt(defaultValue.Z, "Z", this));
            _fields.Add(".w", new ShaderInt(defaultValue.W, "W", this));
        }
    }
    public class ShaderUVec4 : ShaderVar
    {
        [Browsable(false)]
        public override EShaderVarType TypeName => EShaderVarType._uvec4;
        [Category(CategoryName)]
        public UVec4 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(RenderProgram program, int location) 
            => program.Uniform(location, _value);
        internal override string GetShaderValueString() 
            => string.Format("uvec4({0}, {1}, {2}, {3})",
              _value.X.ToString(),
              _value.Y.ToString(),
              _value.Z.ToString(),
              _value.W.ToString());
        [Browsable(false)]
        public override object GenericValue => Value;

        [TSerialize(ValueName, IsXmlElementString = true)]
        private UVec4 _value;

        public ShaderUVec4() : this(new UVec4(), NoName) { }
        public ShaderUVec4(UVec4 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderUVec4(UVec4 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderUInt(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderUInt(defaultValue.Y, "Y", this));
            _fields.Add(".z", new ShaderUInt(defaultValue.Z, "Z", this));
            _fields.Add(".w", new ShaderUInt(defaultValue.W, "W", this));
        }
    }
}
