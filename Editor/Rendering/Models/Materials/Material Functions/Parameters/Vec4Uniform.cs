using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Vec4Uniform : MaterialUniform
    {
        public Vec4Uniform(GLVec4 inputUniform) : base(inputUniform)
        {

        }

        protected override List<string> GetKeywords()
        {
            return new List<string>()
            {
                "vector4",
                "vec4",
                "float4",
                "four",
                "parameter",
                "RGBA",
            };
        }
    }
}
