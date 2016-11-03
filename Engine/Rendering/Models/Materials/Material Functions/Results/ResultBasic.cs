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
        private GLVec4 OutputColor { get { return InputArguments[0] as GLVec4; } }
        protected override List<GLVar> GetInputArguments()
        {
            return new List<GLVar>()
            {
                new GLVec4("OutputColor", FragmentShaderGenerator.OutputColorName),
            };
        }

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
            GLVec4 output = OutputColor;
            return string.Format("{0} = {1}", output, output.Prev.AccessorTree());
        }
        public string Generate(ShaderMode type)
        {

        }
    }
}
