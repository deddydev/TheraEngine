using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Math",
                "A >= B",
                "Returns true bool/bvec if A's individual components are greater than or equal to B's.",
                "greater than or equals to >=")]
    public class GEqualFunc : ComparableFunc
    {
        public GEqualFunc() : base() { }
        protected override string GetOperator() { return ">="; }
    }
}
