using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        TrigCategoryName,
        "Tan",
        "Returns the tangent of the given value.", 
        "tangent value trigonometry")]
    public class TanFunc : BasicGenFloatFunc
    {
        public TanFunc() : base() { }
        protected override string GetFuncName() => "tan";
        protected override string GetInputName() => "Rad";
        protected override string GetOutputName() => "Tan";
        protected override EGLSLVersion GetVersion() => EGLSLVersion.Ver_110;
    }
}
