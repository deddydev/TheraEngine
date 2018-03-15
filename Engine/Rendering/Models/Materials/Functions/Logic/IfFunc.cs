using System.Collections.Generic;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        "Logic",
        "If Branch",
        "Branches code execution if a condition is true.",
        "if branch condition")]
    public class IfFunc : ShaderLogic
    {
        public IfFunc() : base() { }
        protected override string GetOperation()
        {
            return "if ({0})";
        }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            return base.GetValueInputs();
        }
        protected override List<MatFuncValueOutput> GetValueOutputs()
        {
            return new List<MatFuncValueOutput>()
            {

            };
        }
    }
}
