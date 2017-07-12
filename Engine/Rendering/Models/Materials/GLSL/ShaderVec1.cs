namespace TheraEngine.Rendering.Models.Materials
{
    public class ShaderBool : ShaderVar, IUniformable1Bool
    {
        public override ShaderVarType TypeName { get { return ShaderVarType._bool; } }
        public bool Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        public unsafe bool* Data { get { fixed (bool* ptr = &_value) return ptr; } }
        internal override string GetValueString() { return _value.ToString(); }

        private bool _value;

        public ShaderBool(bool defaultValue, string name) : this(defaultValue, name, null) { }
        public ShaderBool(bool defaultValue, string name, IShaderVarOwner owner) : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class ShaderFloat : ShaderVar, IUniformable1Float
    {
        public override ShaderVarType TypeName { get { return ShaderVarType._float; } }
        public float Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        public unsafe float* Data { get { fixed (float* ptr = &_value) return ptr; } }
        internal override string GetValueString() { return _value.ToString(); }

        private float _value;

        public ShaderFloat(float defaultValue, string name) : this(defaultValue, name, null) { }
        public ShaderFloat(float defaultValue, string name, IShaderVarOwner owner) : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class ShaderDouble : ShaderVar, IUniformable1Double
    {
        public override ShaderVarType TypeName { get { return ShaderVarType._double; } }
        public double Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        public unsafe double* Data { get { fixed (double* ptr = &_value) return ptr; } }
        internal override string GetValueString() { return _value.ToString(); }

        private double _value;

        public ShaderDouble(double defaultValue, string name) : this(defaultValue, name, null) { }
        public ShaderDouble(double defaultValue, string name, IShaderVarOwner owner) : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class ShaderInt : ShaderVar, IUniformable1Int
    {
        public override ShaderVarType TypeName { get { return ShaderVarType._int; } }
        public int Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        public unsafe int* Data { get { fixed (int* ptr = &_value) return ptr; } }
        internal override string GetValueString() { return _value.ToString(); }

        private int _value;

        public ShaderInt(int defaultValue, string name) : this(defaultValue, name, null) { }
        public ShaderInt(int defaultValue, string name, IShaderVarOwner owner) : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class ShaderUInt : ShaderVar, IUniformable1UInt
    {
        public override ShaderVarType TypeName { get { return ShaderVarType._uint; } }
        public uint Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        public unsafe uint* Data { get { fixed (uint* ptr = &_value) return ptr; } }
        internal override string GetValueString() { return _value.ToString(); }

        private uint _value;

        public ShaderUInt(uint defaultValue, string name) : this(defaultValue, name, null) { }
        public ShaderUInt(uint defaultValue, string name, IShaderVarOwner owner) : base(name, owner)
        {
            _value = defaultValue;
        }
    }
}
