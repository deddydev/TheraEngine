using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class ExponentFunc : TwoArgFunc
    {
        public ExponentFunc() : base(
            new GLInput("A", GLTypeName._float), 
            new GLInput("B", GLTypeName._float), 
            new GLOutput("Result", GLTypeName._float))
        {

        }
        protected override string GetOperation()
        {
            return "pow({0}, {1})";
        }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Math",
                "Exponent",
                "Returns A to the power of B.",
                "exponent ^ power");
        }

        protected override string GetOperator()
        {
            throw new NotImplementedException();
        }
    }
}
