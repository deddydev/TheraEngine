﻿using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class MatFuncExecInput : FuncExecInput<MatFuncExecOutput, MaterialFunction>
    {
        public MatFuncExecInput(string name)
            : base(name) { }
        public MatFuncExecInput(string name, MaterialFunction parent)
            : base(name, parent) { }
    }
}
