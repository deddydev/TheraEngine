using TheraEngine.Rendering.HUD.Functions;

namespace TheraEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Math",
                "A <= B",
                "Returns true bool/bvec if A's individual components are less than or equal to B's.",
                "less than or equals to <=")]
    public class LEqualFunc : ComparableFunc
    {
        public LEqualFunc() : base() { }
        protected override string GetOperator() { return "<="; }
    }
}
