using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class AddFunc : TwoArgFunc
    {
        public AddFunc(GLTypeName argTypes) : base(argTypes, argTypes, argTypes) { }
        protected override string GetOperator() { return "+"; }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Returns A + B.",
                "added to +");
        }
    }
}
