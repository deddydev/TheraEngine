using System.Collections.Generic;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public abstract class ModifierFunc : ShaderMethod
    {
        public const string CategoryName = "Modifiers";
        
        public ModifierFunc(params ShaderVarType[] outputTypes) : base(outputTypes) { }
        protected override MatFuncValueInput[] GetValueInputs()
        {
            MatFuncValueInput InputValue = new MatFuncValueInput(string.Empty, OutputArguments[0]);
            return new MatFuncValueInput[] { InputValue };
        }
        protected override string GetOperation()
            => GetFuncName() + "({0})";
        protected abstract string GetFuncName();
    }
}
