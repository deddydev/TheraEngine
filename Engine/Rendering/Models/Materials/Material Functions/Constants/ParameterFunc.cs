namespace TheraEngine.Rendering.Models.Materials
{
    public interface IParameterFunc
    {
        void SetUniform(int programBindingId);
    }
    [FunctionDefinition(
                "Constants",
                "Parameter Value",
                "Provides an animatable value to the shader.",
                "constant scalar vector parameter value uniform animatable animate animation")]
    public class ParameterFunc<T> : MaterialFunction, IParameterFunc where T : ShaderVar
    {
        public ParameterFunc() : base(true) { }
        public ParameterFunc(T value) : base(true) { _value = value; }
        
        T _value;

        public T Value
        {
            get => _value;
            set => _value = value;
        }
        protected override string GetOperation()
        {
            if (_value == null)
                return "";
            return _value.ToString();
        }
        public string GetUniformDeclaration(int layoutId = -1)
        {
            if (_value == null)
                return "";
            return _value.GetUniformDeclaration(layoutId);
        }
        public void SetUniform(int programBindingId)
            => _value?.SetProgramUniform(programBindingId);
    }
}
