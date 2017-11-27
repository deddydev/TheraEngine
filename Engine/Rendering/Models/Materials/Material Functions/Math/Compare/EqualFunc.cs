using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
                "Math",
                "A == B",
                "Returns true bool/bvec if A's and B's individual components are equal.",
                "equals ==")]
    public class EqualFunc : ComparableFunc
    {
        public EqualFunc() : base() { }
        protected override string GetOperator() => "==";
    }
}
