using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
         CategoryName,
         "Floor",
         "Returns the floor of the given value; 0.4 -> 0.0",
         "floor value")]
    public class FloorFunc : ModifierFunc
    {
        public FloorFunc() : base(ShaderVar.FloatingPointTypes) { }
        protected override string GetFuncName() => "floor";
    }
}
