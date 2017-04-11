using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
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
        protected override List<FuncValueInput> GetInputs()
        {
            return new List<FuncValueInput>()
            {
                new FuncValueInput("Start Index", false, GLTypeName._int),
                new FuncValueInput("Loop Operation", true, GLTypeName._bool),
                //TODO: material function argument for each loop?
            };
        }
        protected override List<FuncValueOutput> GetOutputs()
        {
            return new List<FuncValueOutput>()
            {
                new FuncValueOutput("Loop Index", false, GLTypeName._int),
                new FuncValueOutput("Loop Count", false, GLTypeName._bool),
                //TODO: material function argument for each loop?
            };
        }
    }
}
