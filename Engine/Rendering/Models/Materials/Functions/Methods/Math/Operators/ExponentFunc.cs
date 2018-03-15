using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        CategoryName,
        "Exponent",
        "Returns A to the power of B.",
        "exponent ^ power to raise")]
    public class ExponentFunc : OperatorFunc
    {
        public ExponentFunc() : base() { }
        protected override string GetOperation()
            => "pow({0}, {1})";
        protected override string GetOperator() => null;
    }
}
