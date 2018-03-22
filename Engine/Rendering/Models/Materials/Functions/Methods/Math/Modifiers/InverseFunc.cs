using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        CategoryName,
        "Inverse",
        "Returns 1 / value.",
        "inverse divided divison one 1 over value")]
    public class InverseFunc : ModifierFunc
    {
        public InverseFunc() : base(ShaderVar.FloatingPointTypes) { }
        protected override string GetFuncName() => null;
        protected override string GetOperation()
            => One(InputArguments[0].ArgumentType) + " / {0}";
    }
}
