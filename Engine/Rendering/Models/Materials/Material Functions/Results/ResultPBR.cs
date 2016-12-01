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
                "Outputs the given vec4 color as the color for this fragment.",
                "result output final return physically based rendering PBR");
        }

        protected override string GetOperation()
        {
            return base.GetOperation();
        }

        protected override List<BaseGLArgument> GetArguments()
        {
            return new List<BaseGLArgument>()
            {
                new GLArgument<GLVec4>("Diffuse"),
                new GLArgument<GLFloat>("Roughness"),
                new GLArgument<GLFloat>("Shininess"),
                new GLArgument<GLFloat>("Specularity"),
                new GLArgument<GLFloat>("Metallic"),
                new GLArgument<GLFloat>("Refraction"),
            };
        }
    }
}
