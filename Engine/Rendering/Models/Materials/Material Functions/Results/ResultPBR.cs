using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    /// <summary>
    /// Physically-based rendering result.
    /// </summary>
    public class ResultPBRFunc : ResultBasicFunc
    {
        public static new FunctionDefinition GetInfo()
        {
            return new FunctionDefinition(
                "Output",
                "Output PBR",
                "Outputs the given vec4 color as the color for this fragment.",
                "result output final return physically based rendering PBR");
        }

        protected override string GetOperation()
        {
            return base.GetOperation();
        }

        protected override List<FuncValueInput> GetInputs()
        {
            return new List<FuncValueInput>()
            {
                new FuncValueInput("Diffuse", GLTypeName._vec4),
                new FuncValueInput("Roughness", GLTypeName._float),
                new FuncValueInput("Shininess", GLTypeName._float),
                new FuncValueInput("Specularity", GLTypeName._float),
                new FuncValueInput("Metallic", GLTypeName._float),
                new FuncValueInput("Refraction", GLTypeName._float),
            };
        }
    }
}
