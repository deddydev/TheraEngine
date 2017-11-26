using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Math",
                "A + B",
                "Returns A + B.",
                "added + concatenate addition")]
    public class AddFunc : OperatorFunc
    {
        public AddFunc() : base() { }
        protected override string GetOperator() => "+";
    }
}
