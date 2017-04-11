using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
            "Math",
            "Exponent",
            "Returns A to the power of B.",
            "exponent ^ power")]
    public class ExponentFunc : OperatorFunc
    {
        public ExponentFunc() : base() { }
        protected override string GetOperation()
            => "pow({0}, {1})";
        protected override string GetOperator() => null;
    }
}
