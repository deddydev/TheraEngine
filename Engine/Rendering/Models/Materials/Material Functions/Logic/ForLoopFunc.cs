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
        protected override List<GLInput> GetInputs()
        {
            return new List<GLInput>()
            {
                new GLInput("Start Index", GLTypeName._int),
                //TODO: condition needs to be built into the loop
                new GLInput("Condition", GLTypeName._bool),
                //TODO: material function argument for each loop?
            };
        }
    }
}
