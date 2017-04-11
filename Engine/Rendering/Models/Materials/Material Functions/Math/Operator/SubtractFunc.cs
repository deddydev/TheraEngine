using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Math",
                "A - B",
                "Returns A - B.",
                "subtracted - subtraction")]
    public class SubtractFunc : OperatorFunc
    {
        public SubtractFunc() : base() { }
        protected override string GetOperator() => "-";
    }
}
