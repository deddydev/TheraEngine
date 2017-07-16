using TheraEngine.Rendering.HUD.Functions;

namespace TheraEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Math",
                "A / B",
                "Returns A / B.",
                "divided division /")]
    public class DivideFunc : OperatorFunc
    {
        public DivideFunc() : base() { }
        protected override string GetOperator() => "/";
    }
}
