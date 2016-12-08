using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class TwoArgFunc : MaterialFunction
    {
        protected GLInput _a, _b;
        protected GLOutput _out;
        public TwoArgFunc(GLInput a, GLInput b, GLOutput output)
        {
            _a = a;
            _b = b;
            _out = output;
            _inline = true;
        }

        protected override List<GLInput> GetInputs()
        {
            return new List<GLInput>() { _a, _b };
        }
        protected override List<GLOutput> GetOutputs()
        {
            return new List<GLOutput>() { _out };
        }

        protected abstract string GetOperator();
        protected override string GetOperation()
        {
            return "{0} " + GetOperator() + " {1}";
        }
    }
}
