using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
                "Math",
                "A - B",
                "Returns A - B.",
                "subtracted - subtraction")]
    public class SubtractFunc : OperatorFunc
    {
        public SubtractFunc() : base() { }
        protected override string GetOperator() => "-";
    }
}
