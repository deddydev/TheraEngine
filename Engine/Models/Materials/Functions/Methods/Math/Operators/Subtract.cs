﻿using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        OperatorCategoryName,
        "Subtract",
        "Returns A - B.",
        "subtracted - subtraction minus negative dash")]
    public class SubtractFunc : OperatorFunc
    {
        public SubtractFunc() : base() { }
        protected override string GetOperator() => "-";
    }
}
