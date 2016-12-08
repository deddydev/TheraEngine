using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class DivideFunc : TwoArgFunc
    {
        public DivideFunc(params GLTypeName[] argTypes) : base(
            new GLInput("A", argTypes),
            new GLInput("B", argTypes),
            new GLOutput("Result", argTypes))
        {

        }
        protected override string GetOperator() { return "/"; }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Math",
                "A / B",
                "Returns A / B.",
                "divided by /");
        }
    }
}
