using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        CategoryName,
        "A < B",
        "Returns true bool/bvec if A's individual components are less than B's.",
        "less than <")]
    public class LessFunc : ComparableFunc
    {
        public LessFunc() : base() { }
        protected override string GetOperator() => "<";
    }
}
