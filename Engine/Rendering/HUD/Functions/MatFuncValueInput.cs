using System.Linq;

namespace CustomEngine.Rendering.Models.Materials
{
    public class MatFuncValueInput : FuncValueInput<MatFuncValueOutput, MaterialFunction>
    {
        public MatFuncValueInput(string name, params GLTypeName[] types)
            : base(name, types.Select(x => (int)x).ToArray()) { }
        public MatFuncValueInput(string name, MaterialFunction parent, params GLTypeName[] types)
            : base(name, parent, types.Select(x => (int)x).ToArray()) { }
        public MatFuncValueInput(string name, MatFuncValueOutput linkedMultiArg)
            : base(name, linkedMultiArg) { }
        public MatFuncValueInput(string name, MaterialFunction parent, MatFuncValueOutput linkedMultiArg)
            : base(name, parent, linkedMultiArg) { }
    }
}
