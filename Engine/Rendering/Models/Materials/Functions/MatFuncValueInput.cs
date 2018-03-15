using System.Linq;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class MatFuncValueInput : FuncValueInput<MatFuncValueOutput, MaterialFunction>
    {
        public object DefaultValue { get; set; }
        public ShaderVarType ArgumentType
        {
            get => (ShaderVarType)CurrentArgumentType;
            set => CurrentArgumentType = (int)value;
        }

        public MatFuncValueInput(string name, params ShaderVarType[] types)
            : base(name, types.Select(x => (int)x).ToArray()) { }
        public MatFuncValueInput(string name, MaterialFunction parent, params ShaderVarType[] types)
            : base(name, parent, types.Select(x => (int)x).ToArray()) { }
        public MatFuncValueInput(string name, IBaseFuncValue linkedMultiArg)
            : base(name, linkedMultiArg) { }
        public MatFuncValueInput(string name, MaterialFunction parent, IBaseFuncValue linkedMultiArg)
            : base(name, parent, linkedMultiArg) { }
    }
}
