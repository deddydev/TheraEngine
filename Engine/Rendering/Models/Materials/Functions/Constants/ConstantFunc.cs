﻿using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        "Constants",
        "Constant Value",
        "Hardcodes a constant value in the shader.",
        "constant scalar vector parameter value")]
    public class ConstantFunc<T> : BaseConstantFunc where T : ShaderVar, new()
    {
        public ConstantFunc() : this(default) { }
        public ConstantFunc(T value) : base(ShaderVar.TypeAssociations[typeof(T)]) => Value = value;
        
        private T _value;
        public T Value
        {
            get => _value;
            set => _value = value ?? new T();
        }

        protected override string GetOperation() => _value.GetShaderValueString();

        public override ShaderVar GetVar() => Value;
    }
    public abstract class BaseConstantFunc : ShaderMethod
    {
        public BaseConstantFunc() : this(default) { }
        public BaseConstantFunc(params ShaderVarType[] types) : base(types) { }

        public abstract ShaderVar GetVar();
    }
}
