using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GreaterFunc : ComparableFunc
    {
        public GreaterFunc() : base() { }
        protected override string GetOperator() { return ">"; }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Math",
                "A > B",
                "Returns true bool/bvec if A's individual components are greater than B's.",
                "greater than >");
        }
    }
}
