using CustomEngine.Rendering.Models.Materials.ShaderGenerator;
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
        private GLArgument<GLVec4> OutputColor { get { return InputArguments[0] as GLArgument<GLVec4>; } }
        protected override List<IGLArgument> GetArguments()
        {
            return new List<IGLArgument>()
            {
                new GLArgument<GLVec4>("FinalColor"),
            };
        }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
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