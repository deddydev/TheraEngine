using TheraEngine.Rendering.HUD.Functions;

namespace TheraEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Constants",
                "Constant Value",
                "Hardcodes a constant value in the shader.",
                "constant scalar vector parameter value")]
    public class ConstantFunc<T> : MaterialFunction where T : ShaderVar
    {
        public ConstantFunc() : base(true) { }
        public ConstantFunc(T value) : base(true) { _value = value; }
        
        T _value;

        public T Value
        {
            get => _value;
            set => _value = value;
        }
        protected override string GetOperation()
            => _value.ToString();
    }
}
