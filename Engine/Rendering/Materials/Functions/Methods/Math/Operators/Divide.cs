using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        OperatorCategoryName,
        "Divide",
        "Returns A / B.",
        "divided division /")]
    public class DivideFunc : OperatorFunc
    {
        public DivideFunc() : base() { }
        protected override string GetOperator() => "/";
    }
}
