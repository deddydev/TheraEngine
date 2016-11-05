using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class LEqualFunc : TwoArgFunc
    {
        public LEqualFunc(GLTypeName argTypes) : base(argTypes, argTypes, GLTypeName._bool) { }
        protected override string GetOperator() { return ">"; }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Returns true bool/bvec if A's individual components are less than or equal to B's.",
                "less than or equals to <=");
        }
    }
}
