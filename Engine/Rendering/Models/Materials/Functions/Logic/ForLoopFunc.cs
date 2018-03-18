using System.Collections.Generic;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        "Logic",
        "For Loop",
        "Runs code X amount of times.",
        "loop for foreach repeat repetition")]
    public class ForLoopFunc : ShaderLogic
    {
        public ForLoopFunc() : base() { }

        public override string GetLogicFormat()
        {
            return @"for (int )";
        }

        //protected override string GetOperation() => "for (int i = {0}; {1}, ";
        
        protected override MatFuncValueInput[] GetValueInputs()
        {
            return new MatFuncValueInput[]
            {
                new MatFuncValueInput("Start Index", ShaderVarType._int),
                new MatFuncValueInput("Loop Operation", ShaderVarType._bool),
                //TODO: material function argument for each loop?
            };
        }
        protected override MatFuncValueOutput[] GetValueOutputs()
        {
            return new MatFuncValueOutput[]
            {
                new MatFuncValueOutput("Loop Index", ShaderVarType._int),
                new MatFuncValueOutput("Loop Count", ShaderVarType._bool),
                //TODO: material function argument for each loop?
            };
        }
    }
}
