﻿using System;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class MatFuncValueInput : FuncValueInput<MatFuncValueOutput, MaterialFunction>
    {
        public ShaderVar DefaultValue { get; private set; } = null;
        public EShaderVarType ArgumentType
        {
            get => (EShaderVarType)CurrentArgumentType;
            set => CurrentArgumentType = (int)value;
        }

        protected override void OnCurrentArgTypeChanged()
        {
            if (ArgumentType == EShaderVarType._invalid)
                DefaultValue = null;
            else
                DefaultValue = (ShaderVar)Activator.CreateInstance(ShaderVar.ShaderTypeAssociations[ArgumentType]);
        }
        public override Vec4 GetTypeColor()
            => ShaderVar.GetTypeColor(ArgumentType);

        public MatFuncValueInput(string name, params EShaderVarType[] types)
            : base(name, types.Select(x => (int)x).ToArray()) { }
        public MatFuncValueInput(string name, MaterialFunction parent, params EShaderVarType[] types)
            : base(name, parent, types.Select(x => (int)x).ToArray()) { }
        public MatFuncValueInput(string name, IBaseFuncValue linkedMultiArg)
            : base(name, linkedMultiArg) { }
        public MatFuncValueInput(string name, MaterialFunction parent, IBaseFuncValue linkedMultiArg)
            : base(name, parent, linkedMultiArg) { }
    }
}
