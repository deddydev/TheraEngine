using TheraEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheraEngine.Rendering.Models.Materials
{
    public class MatFuncExecOutput : FuncExecOutput<MatFuncExecInput, MaterialFunction>
    {
        public MatFuncExecOutput(string name)
            : base(name) { }
        public MatFuncExecOutput(string name, MaterialFunction parent)
            : base(name, parent) { }
    }
}
