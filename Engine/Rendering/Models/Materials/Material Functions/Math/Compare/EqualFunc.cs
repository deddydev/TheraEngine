﻿using TheraEngine.Rendering.HUD.Functions;

namespace TheraEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Math",
                "A == B",
                "Returns true bool/bvec if A's and B's individual components are equal.",
                "equals ==")]
    public class EqualFunc : ComparableFunc
    {
        public EqualFunc() : base() { }
        protected override string GetOperator() { return "=="; }
    }
}
