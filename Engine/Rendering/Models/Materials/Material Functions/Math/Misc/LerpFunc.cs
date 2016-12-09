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
    public class LerpFunc : MaterialFunction
    {
        GLInput A, B, Time;
        GLOutput Result;

        public LerpFunc() : base(true) { }
        protected override string GetOperation() { return "mix({0}, {1}, {2})"; }
        protected override List<GLInput> GetInputs()
        {
            A = new GLInput("A", FloatingPointTypes);
            B = new GLInput("B", A);
            Time = new GLInput("Time", GLTypeName._float);

            return new List<GLInput>() { A, B, Time };
        }
        protected override List<GLOutput> GetOutputs()
        {
            Result = new GLOutput("Result", A);

            return new List<GLOutput>() { Result };
        }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Math",
                "Linear Interpolate",
                "Linearly interpolates between A and B using Time: A + (B - A) * Time", 
                "lerp mix linear interpolate blend");
        }
    }
}
