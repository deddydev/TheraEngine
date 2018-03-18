using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        CategoryName,
        "A != B",
        "Returns true bool/bvec if A's and B's individual components are not equal.",
        "does not equals != equality")]
    public class NEqualFunc : ComparableFunc
    {
        public NEqualFunc() : base() { }
        protected override string GetOperator() => "!=";
    }
}
