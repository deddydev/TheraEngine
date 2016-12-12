using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class ForLoopFunc : MaterialFunction
    {
        public ForLoopFunc() : base(true) { }
        protected override string GetOperation()
        {
            return "for (int i = {0}; i < ";
        }
        protected override List<GLInput> GetInputs()
        {
            return new List<GLInput>()
            {
                new GLInput("Start Index", GLTypeName._int),
                new GLInput("Loop Count", GLTypeName._bool),
                //TODO: material function argument for each loop?
            };
        }
    }
}
