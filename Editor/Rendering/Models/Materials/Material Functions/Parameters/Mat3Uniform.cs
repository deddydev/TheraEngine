using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Mat3Uniform : MaterialUniform
    {
        public Mat3Uniform(GLMat3 inputUniform) : base(inputUniform)
        {

        }

        protected override List<string> GetKeywords()
        {
            return new List<string>()
            {
                "matrix3x3",
                "mat3",
                "mtx3",
                "rotation",
                "normal",
            };
        }
    }
}
