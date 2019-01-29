using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        TrigCategoryName,
        "Sinh",
        "Returns the hyperbolic sine of the given value.",
        "hyperbolic sine value trigonometry")]
    public class SinhFunc : BasicGenFloatFunc
    {
        public SinhFunc() : base() { }
        protected override string GetFuncName() => "sinh";
        protected override EGLSLVersion GetVersion() => EGLSLVersion.Ver_130;
    }
}
