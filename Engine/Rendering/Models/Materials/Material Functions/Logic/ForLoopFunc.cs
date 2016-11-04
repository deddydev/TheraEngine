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
            
        }

        protected override List<GLArgument> GetArguments()
        {
            return new List<GLArgument>()
            {
                new GLArgument(GLTypeName._int, "StartIndex", this),
                new GLArgument(GLTypeName._bool, "Condition", this),
            };
        }
    }
}
