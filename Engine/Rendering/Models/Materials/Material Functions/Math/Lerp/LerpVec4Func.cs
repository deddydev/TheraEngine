using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class LerpVec4Func : MaterialFunction
    {
        public GLFloat MixValue { get { return (GLFloat)InputArguments[0]; } }

        public LerpVec4Func() : base()
        {

        }

        protected override List<string> GetKeywords()
        {
            return new List<string>()
            {
                "lerp",
                "mix",
                "linear",
                "interpolate",
                "4",
                "four"
            };
        }

        protected override List<GLVar> GetInputArguments()
        {
            return new List<GLVar>()
            {
                new GLFloat("Amount", "")
            };
        }

        protected override List<GLVar> GetOutputArguments()
        {
            return new List<GLVar>()
            {
                new GLVec4("mixResult", "")
            };
        }

        protected override string GetOperation()
        {
            return "mix({1}, {2}, {0})";
        }
    }
}
