using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GEqualFunc : TwoArgFunc
    {
        public GEqualFunc(GLTypeName argTypes) : base(
            new GLInput("A", argTypes),
            new GLInput("B", argTypes),
            new GLOutput("Result", GLTypeName._bool))
        {

        }
        protected override string GetOperator() { return ">="; }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Math",
                "A >= B",
                "Returns true bool/bvec if A's individual components are greater than or equal to B's.",
                "greater than or equals to >=");
        }
    }
}
