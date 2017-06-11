using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Math",
                "A * B",
                "Returns A * B.",
                "multiply multiplied multiplication *")]
    public class MultiplyFunc : OperatorFunc
    {
        public MultiplyFunc() : base() { }
        protected override string GetOperator() => "*";
    }
}
