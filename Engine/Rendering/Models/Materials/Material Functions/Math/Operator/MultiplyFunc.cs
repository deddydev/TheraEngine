using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class MultiplyFunc : OperatorFunc
    {
        public MultiplyFunc() : base() { }
        protected override string GetOperator() { return "*"; }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Math",
                "A * B",
                "Returns A * B.",
                "multiply multiplied multiplication *");
        }
    }
}
