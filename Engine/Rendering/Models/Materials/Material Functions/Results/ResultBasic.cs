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
        GLInput FinalColor;
        GLInput WorldPositionOffset;

        public ResultBasicFunc() : base(true) { }
        protected override List<GLInput> GetInputs()
        {
            FinalColor = new GLInput("FinalColor", GLTypeName._vec4);
            WorldPositionOffset = new GLInput("WorldPositionOffset", GLTypeName._vec3);
            return new List<GLInput>() { FinalColor };
        }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Output",
                "Basic Output",
                "Outputs the given vec4 color as the color for this fragment.", 
                "result output final return");
        }
        protected override string GetOperation()
        {
            return FragmentShaderGenerator.OutputColorName + " = {0};";
        }
    }
}