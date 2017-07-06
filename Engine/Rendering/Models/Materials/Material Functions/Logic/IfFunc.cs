using System.Collections.Generic;

namespace TheraEngine.Rendering.Models.Materials
{
    public class IfFunc : MaterialFunction
    {
        public IfFunc() : base(true) { }
        protected override string GetOperation()
        {
            return "if ({0})";
        }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            return base.GetValueInputs();
        }
        protected override List<MatFuncValueOutput> GetValueOutputs()
        {
            return new List<MatFuncValueOutput>()
            {

            };
        }
    }
}
