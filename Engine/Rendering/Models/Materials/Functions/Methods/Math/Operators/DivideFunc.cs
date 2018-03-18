using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        CategoryName,
        "A / B",
        "Returns A / B.",
        "divided division /")]
    public class DivideFunc : OperatorFunc
    {
        public DivideFunc() : base() { }
        protected override string GetOperator() => "/";
    }
}
