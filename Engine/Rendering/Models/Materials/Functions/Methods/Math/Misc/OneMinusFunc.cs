using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    /// <summary>
    /// 1.0f - input
    /// </summary>
    [FunctionDefinition(
        "Helpers",
        "One Minus",
        "Returns 1 - value.",
        "one minus value 1 - subtract")]
    public class OneMinusFunc : ShaderMethod
    {
        public OneMinusFunc() : base(NumericTypes) { }
        protected override string GetOperation()
            => One(InputArguments[0].ArgumentType) + " - {0}";
        protected override MatFuncValueInput[] GetValueInputs()
        {
            MatFuncValueInput InputValue = new MatFuncValueInput(string.Empty, OutputArguments[0]);
            return new MatFuncValueInput[] { InputValue };
        }
    }
}
