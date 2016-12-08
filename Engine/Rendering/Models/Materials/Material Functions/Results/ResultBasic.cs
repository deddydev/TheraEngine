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
        private GLInput OutputColor { get { return InputArguments[0]; } }
        protected override List<GLInput> GetInputs()
        {
            return new List<GLInput>()
            {
                new GLInput("FinalColor", GLTypeName._vec4),
            };
        }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Output",
                "Output",
                "Outputs the given vec4 color as the color for this fragment.", 
                "result output final return");
        }
        protected override string GetOperation()
        {
            return "{0}";
        }
    }
}