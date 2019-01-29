using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        TrigCategoryName,
        "Cosh",
        "Returns the hyperbolic cosine of the given value.", 
        "hyperbolic cosine value trigonometry")]
    public class CoshFunc : BasicGenFloatFunc
    {
        public CoshFunc() : base() { }
        protected override string GetFuncName() => "cosh";
        protected override EGLSLVersion GetVersion() => EGLSLVersion.Ver_130;
    }
}
