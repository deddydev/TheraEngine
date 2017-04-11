using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    /// <summary>
    /// a * (1 − t) + b * t. 
    /// </summary>
    [FunctionDefinition(
                "Math",
                "Linear Interpolate",
                "Linearly interpolates between A and B using Time: A + (B - A) * Time",
                "lerp mix linear interpolate blend")]
    public class LerpFunc : MaterialFunction
    {
        FuncValueInput A, B, Time;
        FuncValueOutput Result;

        public LerpFunc() : base(true) { }
        protected override string GetOperation() { return "mix({0}, {1}, {2})"; }
        protected override List<FuncValueInput> GetInputs()
        {
            A = new FuncValueInput("A", FloatingPointTypes);
            B = new FuncValueInput("B", A);
            Time = new FuncValueInput("Time", GLTypeName._float);

            return new List<FuncValueInput>() { A, B, Time };
        }
        protected override List<FuncValueOutput> GetOutputs()
        {
            Result = new FuncValueOutput("Result", A);

            return new List<FuncValueOutput>() { Result };
        }
    }
}
