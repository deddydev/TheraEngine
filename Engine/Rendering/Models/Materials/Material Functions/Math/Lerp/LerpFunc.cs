using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class LerpFunc : MaterialFunction
    {
        public GLVar MixValue { get { return InputArguments[0]; } }

        public LerpFunc() : base()
        {

        }
        protected override string GetOperation()
        {
            return "{3} = mix({1}, {2}, {0})";
        }
        protected override List<GLVar> GetInputArguments()
        {
            return new List<GLVar>()
            {
                new GLVar(GLTypeName._float, "Amount")
            };
        }
        protected override List<string> GetKeywords()
        {
            return new List<string>()
            {
                "lerp",
                "mix",
                "linear",
                "interpolate",
            };
        }
    }
}
