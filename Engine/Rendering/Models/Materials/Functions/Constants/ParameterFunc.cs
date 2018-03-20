using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
                "Constants",
                "Parameter Value",
                "Provides an animatable value to the shader.",
                "constant scalar vector parameter value uniform animatable animate animation")]
    public class ParameterFunc<T> : BaseParameterFunc where T : ShaderVar
    {
        public ParameterFunc() : this(default) { }
        public ParameterFunc(T value) : base(ShaderVar.TypeAssociations[typeof(T)]) { _value = value; }
        
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
        public override void SetUniform(int programBindingId)
            => _value?.SetProgramUniform(programBindingId);

        public override ShaderVar GetVar() => Value;
    }
    public abstract class BaseParameterFunc : ShaderMethod
    {
        public BaseParameterFunc() : this(default) { }
        public BaseParameterFunc(params ShaderVarType[] types) : base(types) { }
        
        public abstract void SetUniform(int programBindingId);
        public abstract ShaderVar GetVar();
    }
}
