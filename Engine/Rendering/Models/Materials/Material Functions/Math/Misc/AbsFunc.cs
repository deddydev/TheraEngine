using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    /// <summary>
    /// returns the absolute value of the input value
    /// </summary>
    public class AbsFunc : MaterialFunction
    {
        GLInput InputValue;
        GLOutput OutputValue;
        
        public AbsFunc() : base(true) { }
        protected override string GetOperation()
        {
            return "Abs({0})";
        }
        protected override List<GLInput> GetInputs()
        {
            InputValue = new GLInput("Value", SignedTypes);
            return new List<GLInput>() { InputValue };
        }
        protected override List<GLOutput> GetOutputs()
        {
            OutputValue = new GLOutput("Result", InputValue);
            return new List<GLOutput>() { OutputValue };
        }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Math",
                "Absolute Value",
                "Returns the absolute value of the given value.", 
                "absolute value");
        }
    }
}
