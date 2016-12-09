using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class ExponentFunc : OperatorFunc
    {
        public ExponentFunc() : base() { }
        protected override string GetOperation()
        {
            return "pow({0}, {1})";
        }
        protected override string GetOperator()
        {
            throw new NotImplementedException();
        }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Math",
                "Exponent",
                "Returns A to the power of B.",
                "exponent ^ power");
        }
    }
}
