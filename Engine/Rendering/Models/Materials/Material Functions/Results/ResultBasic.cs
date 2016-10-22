using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{    
    /// <summary>
    /// Basic rendering result.
    /// </summary>
    public class ResultBasicFunc : MaterialFunction
    {
        protected override List<string> GetKeywords()
        {
            return new List<string>()
            {
                "result",
                "output",
                "final",
                "return",
            };
        }

        protected override string GetOperation()
        {
            return "";
        }

        protected override List<GLVar> GetInputArguments()
        {
            return new List<GLVar>()
            {
                new GLVec4("OutputColor")
            };
        }
    }
}
