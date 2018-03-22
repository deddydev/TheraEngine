using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        "Helpers",
        "Ceil",
        "Returns the ceiling of the given value; 0.4 -> 1.0", 
        "ceiling value")]
    public class CeilFunc : ModifierFunc
    {
        public CeilFunc() : base(ShaderVar.FloatingPointTypes) { }
        protected override string GetFuncName() => "ceil";
    }
}
