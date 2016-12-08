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
        public static new MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Output",
                "Output PBR",
                "Outputs the given vec4 color as the color for this fragment.",
                "result output final return physically based rendering PBR");
        }

        protected override string GetOperation()
        {
            return base.GetOperation();
        }

        protected override List<GLInput> GetInputs()
        {
            return new List<GLInput>()
            {
                new GLInput("Diffuse", GLTypeName._vec4),
                new GLInput("Roughness", GLTypeName._float),
                new GLInput("Shininess", GLTypeName._float),
                new GLInput("Specularity", GLTypeName._float),
                new GLInput("Metallic", GLTypeName._float),
                new GLInput("Refraction", GLTypeName._float),
            };
        }
    }
}
