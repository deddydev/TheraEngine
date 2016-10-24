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
        protected override List<string> GetKeywords()
        {
            return new List<string>()
            {
                "result",
                "output",
                "final",
                "return",
                "physically",
                "based",
                "rendering",
                "PBR",
            };
        }

        protected override string GetOperation()
        {
            return base.GetOperation();
        }

        protected override List<GLVar> GetInputArguments()
        {
            return new List<GLVar>()
            {
                new GLVec4("Diffuse"),
                new GLFloat("Roughness"),
                new GLFloat("Shininess"),
                new GLFloat("Specularity"),
                new GLFloat("Metallic"),
                new GLFloat("Refraction"),
            };
        }
    }
}
