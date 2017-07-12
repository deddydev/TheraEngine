using System.Collections.Generic;

namespace TheraEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
        "Logic",
        "For Loop",
        "Runs code X amount of times.",
        "loop for foreach repeat repetition")]
    public class ForLoopFunc : MaterialFunction
    {
        public ForLoopFunc() : base(true) { }
        protected override string GetOperation()
        {
            return "for (int i = {0}; {1}, ";
        }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            return new List<MatFuncValueInput>()
            {
                new MatFuncValueInput("Start Index", ShaderVarType._int),
                new MatFuncValueInput("Loop Operation", ShaderVarType._bool),
                //TODO: material function argument for each loop?
            };
        }
        protected override List<MatFuncValueOutput> GetValueOutputs()
        {
            return new List<MatFuncValueOutput>()
            {
                new MatFuncValueOutput("Loop Index", ShaderVarType._int),
                new MatFuncValueOutput("Loop Count", ShaderVarType._bool),
                //TODO: material function argument for each loop?
            };
        }
    }
}
