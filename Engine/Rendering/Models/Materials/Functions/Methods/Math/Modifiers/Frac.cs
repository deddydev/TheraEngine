using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        CategoryName,
        "Frac",
        "Returns the fractional part of the given value; 3.46 -> 0.46", 
        "fractional decimal float double single value")]
    public class FracFunc : ModifierFunc
    {
        public FracFunc() : base(ShaderVar.FloatingPointTypes) { }
        protected override string GetFuncName() => "frac";
    }
}
