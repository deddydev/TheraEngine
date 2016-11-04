using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class OneOutFunc : MaterialFunction
    {
        protected GLTypeName _outType;
        public OneOutFunc(GLTypeName outType) : base()
        {
            _outType = outType;
        }
        protected override List<GLVar> GetOutputArguments()
        {
            return new List<GLVar>()
            {
                new GLVar(_outType, "Output")
            };
        }
        protected override string GetOperation()
        {
            return "{" + InputArguments.Count + "} = ";
        }
    }
}
