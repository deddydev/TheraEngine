using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class LerpFunc : TwoArgFunc
    {
        public GLVar Time { get { return InputArguments[0]; } }
        
        public LerpFunc(GLTypeName operandTypes) : base(operandTypes, operandTypes) { }
        protected override string GetOperation()
        {
            return "mix({1}, {2}, {0})";
        }
        protected override List<GLVar> GetArguments()
        {
            List<GLVar> list = new List<GLVar>()
            {
                new GLVar(GLTypeName._float, "Time"),
            };
            list.AddRange(base.GetArguments());
            return list;
        }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Linearly interpolates between A and B using Time: A + (B - A) * Time", 
                "lerp mix linear interpolate blend");
        }
    }
}
