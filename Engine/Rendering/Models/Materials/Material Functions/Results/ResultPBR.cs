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
    [FunctionDefinition(
                "Output",
                "Output PBR",
                "Outputs the given vec4 color as the color for this fragment.",
                "result output final return physically based rendering PBR")]
    public class ResultPBRFunc : ResultBasicFunc
    {
        protected override string GetOperation()
        {
            return base.GetOperation();
        }

        protected override List<MatFuncValueInput> GetValueInputs()
        {
            return new List<MatFuncValueInput>()
            {
                new MatFuncValueInput("Diffuse", GLTypeName._vec4),
                new MatFuncValueInput("Roughness", GLTypeName._float),
                new MatFuncValueInput("Shininess", GLTypeName._float),
                new MatFuncValueInput("Specularity", GLTypeName._float),
                new MatFuncValueInput("Metallic", GLTypeName._float),
                new MatFuncValueInput("Refraction", GLTypeName._float),
            };
        }
    }
}
