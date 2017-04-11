using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class IfFunc : MaterialFunction
    {
        public IfFunc() : base(true) { }
        protected override string GetOperation()
        {
            return "if ({0})";
        }
        protected override List<FuncValueInput> GetInputs()
        {
            return base.GetInputs();
        }
        protected override List<FuncValueOutput> GetOutputs()
        {
            return new List<FuncValueOutput>()
            {

            };
        }
    }
}
