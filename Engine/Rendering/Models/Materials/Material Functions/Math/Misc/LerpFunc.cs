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
        public GLInput Time { get { return InputArguments[0]; } }
        
        public LerpFunc(GLTypeName operandTypes) : base() { _inline = true; }
        protected override string GetOperation() { return "mix({0}, {1}, {2})"; }
        protected override List<GLInput> GetInputs()
        {
            GLInput a = new GLInput("A", GLTypeName._float, GLTypeName._vec2, GLTypeName._vec3, GLTypeName._vec4);
            GLInput b = new GLInput("B", a);
            return new List<GLInput>()
            {
                a, b, new GLInput("Time", GLTypeName._float),
            };
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
