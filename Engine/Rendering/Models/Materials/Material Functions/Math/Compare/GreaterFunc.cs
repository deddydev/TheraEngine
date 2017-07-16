﻿using TheraEngine.Rendering.HUD.Functions;

namespace TheraEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Math",
                "A > B",
                "Returns true bool/bvec if A's individual components are greater than B's.",
                "greater than >")]
    public class GreaterFunc : ComparableFunc
    {
        public GreaterFunc() : base() { }
        protected override string GetOperator() { return ">"; }
    }
}
