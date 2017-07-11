namespace TheraEngine.Rendering.Models.Materials
{
    public class GLBool : ShaderVar, IUniformable1Bool
    {
        public override ShaderType TypeName { get { return ShaderType._bool; } }
        public bool Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        public unsafe bool* Data { get { fixed (bool* ptr = &_value) return ptr; } }
        internal override string GetValueString() { return _value.ToString(); }

        private bool _value;

        public GLBool(bool defaultValue, string name) : this(defaultValue, name, null) { }
        public GLBool(bool defaultValue, string name, IShaderVarOwner owner) : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class GLFloat : ShaderVar, IUniformable1Float
    {
        public override ShaderType TypeName { get { return ShaderType._float; } }
        public float Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        public unsafe float* Data { get { fixed (float* ptr = &_value) return ptr; } }
        internal override string GetValueString() { return _value.ToString(); }

        private float _value;

        public GLFloat(float defaultValue, string name) : this(defaultValue, name, null) { }
        public GLFloat(float defaultValue, string name, IShaderVarOwner owner) : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class GLDouble : ShaderVar, IUniformable1Double
    {
        public override ShaderType TypeName { get { return ShaderType._double; } }
        public double Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        public unsafe double* Data { get { fixed (double* ptr = &_value) return ptr; } }
        internal override string GetValueString() { return _value.ToString(); }

        private double _value;

        public GLDouble(double defaultValue, string name) : this(defaultValue, name, null) { }
        public GLDouble(double defaultValue, string name, IShaderVarOwner owner) : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class GLInt : ShaderVar, IUniformable1Int
    {
        public override ShaderType TypeName { get { return ShaderType._int; } }
        public int Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        public unsafe int* Data { get { fixed (int* ptr = &_value) return ptr; } }
        internal override string GetValueString() { return _value.ToString(); }

        private int _value;

        public GLInt(int defaultValue, string name) : this(defaultValue, name, null) { }
        public GLInt(int defaultValue, string name, IShaderVarOwner owner) : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class GLUInt : ShaderVar, IUniformable1UInt
    {
        public override ShaderType TypeName { get { return ShaderType._uint; } }
        public uint Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        public unsafe uint* Data { get { fixed (uint* ptr = &_value) return ptr; } }
        internal override string GetValueString() { return _value.ToString(); }

        private uint _value;

        public GLUInt(uint defaultValue, string name) : this(defaultValue, name, null) { }
        public GLUInt(uint defaultValue, string name, IShaderVarOwner owner) : base(name, owner)
        {
            _value = defaultValue;
        }
    }
}
