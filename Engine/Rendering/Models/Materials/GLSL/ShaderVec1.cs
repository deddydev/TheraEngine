using System.ComponentModel;

namespace TheraEngine.Rendering.Models.Materials
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderBool : ShaderVar, IUniformable1Bool
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._bool;
        public bool Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        [Browsable(false)]
        public unsafe bool* Data { get { fixed (bool* ptr = &_value) return ptr; } }
        internal override string GetValueString() => _value.ToString();

        private bool _value;

        public ShaderBool(bool defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderBool(bool defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderFloat : ShaderVar, IUniformable1Float
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._float;
        public float Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.Uniform(programBindingId, location, _value);
        [Browsable(false)]
        public unsafe float* Data { get { fixed (float* ptr = &_value) return ptr; } }
        internal override string GetValueString() => _value.ToString();

        private float _value;

        public ShaderFloat(float defaultValue, string name) 
            : this(defaultValue, name, null) { }
        public ShaderFloat(float defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderDouble : ShaderVar, IUniformable1Double
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._double;
        public double Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        [Browsable(false)]
        public unsafe double* Data { get { fixed (double* ptr = &_value) return ptr; } }
        internal override string GetValueString() => _value.ToString();

        private double _value;

        public ShaderDouble(double defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderDouble(double defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderInt : ShaderVar, IUniformable1Int
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._int;
        public int Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.Uniform(programBindingId, location, _value);
        [Browsable(false)]
        public unsafe int* Data { get { fixed (int* ptr = &_value) return ptr; } }
        internal override string GetValueString() => _value.ToString();

        private int _value;

        public ShaderInt(int defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderInt(int defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderUInt : ShaderVar, IUniformable1UInt
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._uint;
        public uint Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        [Browsable(false)]
        public unsafe uint* Data { get { fixed (uint* ptr = &_value) return ptr; } }
        internal override string GetValueString() => _value.ToString();

        private uint _value;

        public ShaderUInt(uint defaultValue, string name) 
            : this(defaultValue, name, null) { }
        public ShaderUInt(uint defaultValue, string name, IShaderVarOwner owner) 
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
}
