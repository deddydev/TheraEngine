using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class MatFuncValueOutput : FuncValueOutput<MatFuncValueInput, MaterialFunction>
    {
        public EShaderVarType ArgumentType => (EShaderVarType)CurrentArgumentType;

        internal string OutputVarName { get; set; }

        public EGenShaderVarType[] GetPossibleTypes()
        {
            HashSet<EGenShaderVarType> types = new HashSet<EGenShaderVarType>();
            foreach (int i in ParentSocket.CurrentValidOverloads)
                types.Add(ParentSocket.Overloads[i].Inputs[ArgumentIndex]);
            return types.ToArray();
        }

        public override Vec4 GetTypeColor()
            => ShaderVar.GetTypeColor(ArgumentType);

        public override bool CanConnectTo(MatFuncValueInput other)
            => MaterialFunction.CanConnect(other, this);

        protected override void DetermineBestArgType(MatFuncValueInput connection)
        {
            ParentSocket.RecalcValidOverloads();
        }

        public MatFuncValueOutput(string name, MaterialFunction parent) : base(name, parent) { }
    }
}
