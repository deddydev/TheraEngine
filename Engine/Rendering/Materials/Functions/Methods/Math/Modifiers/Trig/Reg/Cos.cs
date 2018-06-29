using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        TrigCategoryName,
        "Cos",
        "Returns the cosine of the given value.", 
        "cosine value trigonometry")]
    public class CosFunc : BasicGenFloatFunc
    {
        public CosFunc() : base() { }
        protected override string GetFuncName() => "cos";
        protected override string GetInputName() => "Rad";
        protected override string GetOutputName() => "Cos";
        protected override EGLSLVersion GetVersion() => EGLSLVersion.Ver_110;
    }
}
