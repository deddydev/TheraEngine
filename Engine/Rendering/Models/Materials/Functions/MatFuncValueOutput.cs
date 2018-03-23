using System.Linq;
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

        public MatFuncValueOutput(string name, params EShaderVarType[] types)
            : base(name, types.Select(x => (int)x).ToArray()) { }
        public MatFuncValueOutput(string name, MaterialFunction parent, params EShaderVarType[] types)
            : base(name, parent, types.Select(x => (int)x).ToArray()) { }
        public MatFuncValueOutput(string name, MatFuncValueInput linkedMultiArg)
            : base(name, linkedMultiArg) { }
        public MatFuncValueOutput(string name, MaterialFunction parent, MatFuncValueInput linkedMultiArg)
            : base(name, parent, linkedMultiArg) { }
    }
}
