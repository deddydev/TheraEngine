using System.Linq;

namespace TheraEngine.Rendering.Models.Materials
{
    public class MatFuncValueInput : FuncValueInput<MatFuncValueOutput, MaterialFunction>
    {
        public MatFuncValueInput(string name, params ShaderType[] types)
            : base(name, types.Select(x => (int)x).ToArray()) { }
        public MatFuncValueInput(string name, MaterialFunction parent, params ShaderType[] types)
            : base(name, parent, types.Select(x => (int)x).ToArray()) { }
        public MatFuncValueInput(string name, IBaseFuncValue linkedMultiArg)
            : base(name, linkedMultiArg) { }
        public MatFuncValueInput(string name, MaterialFunction parent, IBaseFuncValue linkedMultiArg)
            : base(name, parent, linkedMultiArg) { }
    }
}
