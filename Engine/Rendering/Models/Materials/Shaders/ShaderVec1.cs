using System.ComponentModel;

namespace TheraEngine.Rendering.Models.Materials
{
    public class ShaderBool : ShaderVar, 
        IUniformable1Bool,
        IShaderBooleanType,
        IShaderNonVectorType
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._bool;
        [Category(CategoryName)]
        public bool Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        [Browsable(false)]
        public unsafe bool* Data { get { fixed (bool* ptr = &_value) return ptr; } }
        internal override string GetValueString() => _value.ToString();

        [TSerialize(ValueName, IsXmlElementString = true)]
        private bool _value;

        public ShaderBool() : this(false, NoName) { }
        public ShaderBool(bool defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderBool(bool defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner) => _value = defaultValue;
    }
    public class ShaderFloat : ShaderVar, 
        IUniformable1Float,
        IShaderFloatType,
        IShaderNonVectorType,
        IShaderNumericType,
        IShaderDecimalType,
        IShaderSignedType
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._float;
        [Category(CategoryName)]
        public float Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.Uniform(programBindingId, location, _value);
        [Browsable(false)]
        public unsafe float* Data { get { fixed (float* ptr = &_value) return ptr; } }
        internal override string GetValueString() => _value.ToString();

        [TSerialize(ValueName, IsXmlElementString = true)]
        private float _value;

        public ShaderFloat() : this(0.0f, NoName) { }
        public ShaderFloat(float defaultValue, string name) 
            : this(defaultValue, name, null) { }
        public ShaderFloat(float defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner) => _value = defaultValue;
    }
    public class ShaderDouble : ShaderVar, 
        IUniformable1Double,
        IShaderDoubleType, 
        IShaderNonVectorType,
        IShaderNumericType,
        IShaderDecimalType, 
        IShaderSignedType
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._double;
        [Category(CategoryName)]
        public double Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        [Browsable(false)]
        public unsafe double* Data { get { fixed (double* ptr = &_value) return ptr; } }
        internal override string GetValueString() => _value.ToString();

        [TSerialize(ValueName, IsXmlElementString = true)]
        private double _value;

        public ShaderDouble() : this(0.0, NoName) { }
        public ShaderDouble(double defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderDouble(double defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner) => _value = defaultValue;
    }
    public class ShaderInt : ShaderVar, 
        IUniformable1Int,
        IShaderSignedIntType, 
        IShaderNonVectorType,
        IShaderNumericType,
        IShaderNonDecimalType,
        IShaderSignedType
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._int;
        [Category(CategoryName)]
        public int Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.Uniform(programBindingId, location, _value);
        [Browsable(false)]
        public unsafe int* Data { get { fixed (int* ptr = &_value) return ptr; } }
        internal override string GetValueString() => _value.ToString();

        [TSerialize(ValueName, IsXmlElementString = true)]
        private int _value;

        public ShaderInt() : this(0, NoName) { }
        public ShaderInt(int defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderInt(int defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner) => _value = defaultValue;
    }
    public class ShaderUInt : ShaderVar,
        IUniformable1UInt,
        IShaderUnsignedIntType,
        IShaderUnsignedType,
        IShaderNonDecimalType, 
        IShaderNumericType,
        IShaderNonVectorType
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._uint;
        [Category(CategoryName)]
        public uint Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        [Browsable(false)]
        public unsafe uint* Data { get { fixed (uint* ptr = &_value) return ptr; } }
        internal override string GetValueString() => _value.ToString();

        [TSerialize(ValueName, IsXmlElementString = true)]
        private uint _value;
        
        public ShaderUInt() : this(0u, NoName) { }
        public ShaderUInt(uint defaultValue, string name) 
            : this(defaultValue, name, null) { }
        public ShaderUInt(uint defaultValue, string name, IShaderVarOwner owner) 
            : base(name, owner) => _value = defaultValue;
    }
}
