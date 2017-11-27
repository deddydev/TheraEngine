using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class MatFuncExecOutput : FuncExecOutput<MatFuncExecInput, MaterialFunction>
    {
        public MatFuncExecOutput(string name)
            : base(name) { }
        public MatFuncExecOutput(string name, MaterialFunction parent)
            : base(name, parent) { }
    }
}
