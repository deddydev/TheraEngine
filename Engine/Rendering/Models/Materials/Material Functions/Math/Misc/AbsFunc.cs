using System.Collections.Generic;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    /// <summary>
    /// Returns the absolute value of the input value
    /// </summary>
    [FunctionDefinition(
        "Helpers",
        "Absolute Value",
        "Returns the absolute value of the given value; |A|", 
        "absolute value")]
    public class AbsFunc : MaterialFunction
    {
        MatFuncValueInput InputValue;
        MatFuncValueOutput OutputValue;
        
        public AbsFunc() : base(true) { }
        protected override string GetOperation()
        {
            return "Abs({0})";
        }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            InputValue = new MatFuncValueInput("Value", SignedTypes);
            return new List<MatFuncValueInput>() { InputValue };
        }
        protected override List<MatFuncValueOutput> GetValueOutputs()
        {
            OutputValue = new MatFuncValueOutput("Result", InputValue);
            return new List<MatFuncValueOutput>() { OutputValue };
        }
    }
}
