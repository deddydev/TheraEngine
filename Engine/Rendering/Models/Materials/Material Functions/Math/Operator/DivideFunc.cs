using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Math",
                "A / B",
                "Returns A / B.",
                "divided division /")]
    public class DivideFunc : OperatorFunc
    {
        public DivideFunc() : base() { }
        protected override string GetOperator() => "/";
    }
}
