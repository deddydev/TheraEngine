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
        public GLArgument<GLFloat> Time { get { return (GLArgument<GLFloat>)InputArguments[0]; } }
        
        public LerpFunc(GLTypeName operandTypes) : base() { _inline = true; }
        protected override string GetOperation()
        {
            return "mix({0}, {1}, {2})";
        }
        protected override List<BaseGLArgument> GetArguments()
        {
            return new List<BaseGLArgument>()
            {
                new GLMultiArgument("A"),
                new GLMultiArgument("B"),
                new GLArgument<GLFloat>("Time"),
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
