using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
                "Constants",
                "Parameter Value",
                "Provides an animatable value to the shader.",
                "constant scalar vector parameter value uniform animatable animate animation")]
    public class ParameterFunc<T> : BaseParameterFunc where T : ShaderVar, new()
    {
        public ParameterFunc() : this(default) { }
        public ParameterFunc(T value) : base(ShaderVar.TypeAssociations[typeof(T)])
        {
            Value = value;
            HasGlobalVarDec = true;
        }
        
        T _value;

        public T Value
        {
            get => _value;
            set => _value = value ?? new T();
        }

        protected override string GetOperation() => _value.GetShaderValueString();
        public override string GetGlobalVarDec() => _value.GetUniformDeclaration();
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
