using System.Linq;

namespace CustomEngine.Rendering.Models.Materials
{
    public class MatFuncValueOutput : FuncValueOutput<MatFuncValueInput, MaterialFunction>
    {
        public MatFuncValueOutput(string name, params GLTypeName[] types)
            : base(name, types.Select(x => (int)x).ToArray()) { }
        public MatFuncValueOutput(string name, MaterialFunction parent, params GLTypeName[] types)
            : base(name, parent, types.Select(x => (int)x).ToArray()) { }
        public MatFuncValueOutput(string name, MatFuncValueInput linkedMultiArg)
            : base(name, linkedMultiArg) { }
        public MatFuncValueOutput(string name, MaterialFunction parent, MatFuncValueInput linkedMultiArg)
            : base(name, parent, linkedMultiArg) { }
    }
}
