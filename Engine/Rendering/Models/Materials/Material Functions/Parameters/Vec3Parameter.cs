using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Vec3Parameter : MaterialParameter
    {
        public Vec3Parameter(GLVar inputUniform) : base(inputUniform)
        {

        }

        protected override List<string> GetKeywords()
        {
            return new List<string>()
            {
                "vector3",
                "vec3",
                "float3",
                "three",
                "parameter"
            };
        }
    }
}
