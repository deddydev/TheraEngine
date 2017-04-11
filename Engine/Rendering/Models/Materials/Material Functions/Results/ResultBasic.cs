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
    [FunctionDefinition(
                "Output",
                "Basic Output",
                "Outputs the given vec4 color as the color for this fragment.",
                "result output final return")]
    public class ResultBasicFunc : MaterialFunction
    {
        FuncValueInput FinalColor;
        FuncValueInput WorldPositionOffset;

        public ResultBasicFunc() : base(true) { }
        protected override List<FuncValueInput> GetInputs()
        {
            FinalColor = new FuncValueInput("FinalColor", GLTypeName._vec4);
            WorldPositionOffset = new FuncValueInput("WorldPositionOffset", GLTypeName._vec3);
            return new List<FuncValueInput>() { FinalColor };
        }
        public static FunctionDefinition GetInfo()
        {
            return new;
        }
        protected override string GetOperation()
        {
            return FragmentShaderGenerator.OutputColorName + " = {0};";
        }
    }
}