using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class ExponentFunc : TwoArgFunc
    {
        public ExponentFunc(GLTypeName argTypes) : base(argTypes, argTypes, argTypes) { }
        protected override string GetOperation()
        {
            return "{2} = pow({0}, {1});";
        }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Math",
                "Returns A + B.",
                "added to +");
        }
    }
}
