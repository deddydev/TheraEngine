using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Math",
                "A + B",
                "Returns A + B.",
                "added + concatenate addition")]
    public class AddFunc : OperatorFunc
    {
        public AddFunc() : base() { }
        protected override string GetOperator() => "+";
    }
}
