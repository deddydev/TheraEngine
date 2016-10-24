using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class FloatParameter : MaterialParameter
    {
        public FloatParameter(GLVar inputUniform) : base(inputUniform)
        {

        }

        protected override List<string> GetKeywords()
        {
            return new List<string>()
            {
                "scalar",
                "float",
                "parameter",
            };
        }
    }
}
