using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Vec2Parameter : MaterialParameter
    {
        public Vec2Parameter(GLVar inputUniform) : base(inputUniform)
        {

        }

        protected override List<string> GetKeywords()
        {
            return new List<string>()
            {
                "vector2",
                "vec2",
                "float2",
                "two",
                "parameter"
            };
        }
    }
}
