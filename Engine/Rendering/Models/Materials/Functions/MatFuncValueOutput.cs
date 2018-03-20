using System.Linq;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class MatFuncValueOutput : FuncValueOutput<MatFuncValueInput, MaterialFunction>
    {
        public ShaderVarType ArgumentType => (ShaderVarType)CurrentArgumentType;

        public override Vec4 GetTypeColor()
            => ShaderVar.GetTypeColor(ArgumentType);

        public MatFuncValueOutput(string name, params ShaderVarType[] types)
            : base(name, types.Select(x => (int)x).ToArray()) { }
        public MatFuncValueOutput(string name, MaterialFunction parent, params ShaderVarType[] types)
            : base(name, parent, types.Select(x => (int)x).ToArray()) { }
        public MatFuncValueOutput(string name, MatFuncValueInput linkedMultiArg)
            : base(name, linkedMultiArg) { }
        public MatFuncValueOutput(string name, MaterialFunction parent, MatFuncValueInput linkedMultiArg)
            : base(name, parent, linkedMultiArg) { }
    }
}
