using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class ConstFloatFunc : MaterialFunction
    {
        float _value;

        public float Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public ConstFloatFunc() : base()
        {
            
        }

        protected override List<GLArgument> GetOutputArguments()
        {
            return new List<GLArgument>()
            {
                new GLArgument(GLTypeName._float, "Value", )
            };
        }

        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Hardcodes a consant, unchanging value in the shader.", 
                "constant scalar float single parameter");
        }

        protected override string GetOperation()
        {
            return "{0} = " + _value.ToString() + ";";
        }
    }
}
