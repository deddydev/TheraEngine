﻿using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        CategoryName,
        "A == B",
        "Returns true bool/bvec if A's and B's individual components are equal.",
        "equals == equality")]
    public class EqualFunc : ComparableFunc
    {
        public EqualFunc() : base() { }
        protected override string GetOperator() => "==";
    }
}
