﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class MultiplyFunc : TwoArgFunc
    {
        public MultiplyFunc(GLTypeName argTypes) : base(argTypes, argTypes, argTypes) { }
        protected override string GetOperator() { return "*"; }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Math",
                "Returns A * B.",
                "multiply multiplied by *");
        }
    }
}
