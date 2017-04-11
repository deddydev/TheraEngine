using System;

namespace CustomEngine.Rendering.Models.Materials
{
    public class MatFuncExecInput : FuncExecInput<MatFuncExecOutput, MaterialFunction>
    {
        public MatFuncExecInput(string name)
            : base(name) { }
        public MatFuncExecInput(string name, MaterialFunction parent)
            : base(name, parent) { }
    }
}
