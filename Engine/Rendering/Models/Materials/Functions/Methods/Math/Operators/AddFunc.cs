using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        CategoryName,
        "A + B",
        "Returns A + B.",
        "added + concatenate addition plus")]
    public class AddFunc : OperatorFunc
    {
        public AddFunc() : base() { }
        protected override string GetOperator() => "+";
    }
}
