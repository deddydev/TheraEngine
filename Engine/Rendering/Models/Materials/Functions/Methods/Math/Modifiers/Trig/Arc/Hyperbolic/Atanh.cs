using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        TrigCategoryName,
        "Atanh",
        "Returns the hyperbolic arctangent of the given value.", 
        "hyperbolic arc tangent value inverse trigonometry")]
    public class AtanhFunc : BasicGenFloatFunc
    {
        public AtanhFunc() : base() { }
        protected override string GetFuncName() => "atanh";
        protected override EGLSLVersion GetVersion() => EGLSLVersion.Ver_110;
    }
}
