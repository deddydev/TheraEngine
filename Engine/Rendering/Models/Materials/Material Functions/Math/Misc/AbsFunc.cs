using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    /// <summary>
    /// Returns the absolute value of the input value
    /// </summary>
    [FunctionDefinition(
                "Math",
                "Absolute Value",
                "Returns the absolute value of the given value.", 
                "absolute value")]
    public class AbsFunc : MaterialFunction
    {
        FuncValueInput InputValue;
        FuncValueOutput OutputValue;
        
        public AbsFunc() : base(true) { }
        protected override string GetOperation()
        {
            return "Abs({0})";
        }
        protected override List<FuncValueInput> GetInputs()
        {
            InputValue = new FuncValueInput("Value", SignedTypes);
            return new List<FuncValueInput>() { InputValue };
        }
        protected override List<FuncValueOutput> GetOutputs()
        {
            OutputValue = new FuncValueOutput("Result", InputValue);
            return new List<FuncValueOutput>() { OutputValue };
        }
    }
}
