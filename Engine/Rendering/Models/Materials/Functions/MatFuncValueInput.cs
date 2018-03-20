using System;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class MatFuncValueInput : FuncValueInput<MatFuncValueOutput, MaterialFunction>
    {
        public ShaderVar DefaultValue { get; private set; } = null;
        public ShaderVarType ArgumentType
        {
            get => (ShaderVarType)CurrentArgumentType;
            set => CurrentArgumentType = (int)value;
        }

        protected override void OnCurrentArgTypeChanged()
        {
            if (ArgumentType == ShaderVarType.Invalid)
                DefaultValue = null;
            else
                DefaultValue = (ShaderVar)Activator.CreateInstance(ShaderVar.ShaderTypeAssociations[ArgumentType]);
        }
        public override Vec4 GetTypeColor()
            => ShaderVar.GetTypeColor(ArgumentType);

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
