using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class MatFuncValueOutput : FuncValueOutput<MatFuncValueInput, MaterialFunction>
    {
        public EShaderVarType ArgumentType => (EShaderVarType)CurrentArgumentType;

        internal string OutputVarName { get; set; }

        public override Vec4 GetTypeColor()
            => ShaderVar.GetTypeColor(ArgumentType);

        public override bool CanConnectTo(MatFuncValueInput other)
            => MaterialFunction.CanConnect(other, this);

        public MatFuncValueOutput(string name, MaterialFunction parent) : base(name, parent) { }
    }
}
