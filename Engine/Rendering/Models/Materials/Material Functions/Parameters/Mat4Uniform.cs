using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Mat4Uniform : MaterialUniform
    {
        public Mat4Uniform(GLMat4 inputUniform) : base(inputUniform)
        {

        }

        protected override List<string> GetKeywords()
        {
            return new List<string>()
            {
                "matrix4x4",
                "mat4",
                "mtx4",
                "transform",
                "model",
                "view",
                "projection",
            };
        }
    }
}
