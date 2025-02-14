﻿using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        OperatorCategoryName,
        "Multiply",
        "Returns A * B.",
        "multiply multiplied multiplication *")]
    public class MultiplyFunc : OperatorFunc
    {
        public MultiplyFunc() : base() { }
        protected override string GetOperator() => "*";
    }
}
