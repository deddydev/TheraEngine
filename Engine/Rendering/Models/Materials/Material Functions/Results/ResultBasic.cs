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
        private GLArgument OutputColor { get { return InputArguments[0]; } }
        protected override List<GLVar> GetArguments()
        {
            return new List<GLVar>()
            {
                new GLArgument(GLTypeName._vec4, "FinalColor", this),
            };
        }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Outputs the given vec4 color as the color for this fragment.", 
                "result output final return");
        }
        protected override string GetOperation()
        {
            return FragmentShaderGenerator.OutputColorName + " = {0};";
        }
    }
}