using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Math",
                "A != B",
                "Returns true bool/bvec if A's and B's individual components are not equal.",
                "does not equals !=")]
    public class NEqualFunc : ComparableFunc
    {
        public NEqualFunc() : base() { }
        protected override string GetOperator() { return "!="; }
    }
}
