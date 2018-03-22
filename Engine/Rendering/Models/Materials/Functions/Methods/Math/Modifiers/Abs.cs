using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        CategoryName,
        "Abs",
        "Returns the absolute value of the given value; |value|", 
        "absolute value")]
    public class AbsFunc : ModifierFunc
    {
        public AbsFunc() : base(ShaderVar.SignedTypes) { }
        override funcna
        protected override string GetOperation() => "abs({0})";
        protected override MatFuncValueInput[] GetValueInputs()
        {
            MatFuncValueInput InputValue = new MatFuncValueInput(string.Empty, OutputArguments[0]);
            return new MatFuncValueInput[] { InputValue };
        }
    }
}
