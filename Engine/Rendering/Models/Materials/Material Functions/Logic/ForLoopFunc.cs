using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class ForLoopFunc : MaterialFunction
    {
        protected override string GetOperation()
        {
            throw new NotImplementedException();
        }
        protected override List<BaseGLArgument> GetInputs()
        {
            return new List<BaseGLArgument>()
            {
                new GLInput<GLInt>("Start Index"),
                //TODO: condition needs to be built into the loop
                new GLInput<GLBool>("Condition"),
                //TODO: material function argument for each loop?
            };
        }
    }
}
