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
        public GLInput<GLFloat> Time { get { return (GLInput<GLFloat>)InputArguments[0]; } }
        
        public LerpFunc(GLTypeName operandTypes) : base() { _inline = true; }
        protected override string GetOperation() { return "mix({0}, {1}, {2})"; }
        protected override List<BaseGLArgument> GetInputs()
        {
            GLMultiInput a = new GLMultiInput("A", GLTypeName._float, GLTypeName._vec2, GLTypeName._vec3, GLTypeName._vec4);
            GLMultiInput b = new GLMultiInput("B", a);
            return new List<BaseGLArgument>()
            {
                a, b, new GLInput<GLFloat>("Time"),
            };
        }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Math",
                "Linearly interpolates between A and B using Time: A + (B - A) * Time", 
                "lerp mix linear interpolate blend");
        }
    }
}
