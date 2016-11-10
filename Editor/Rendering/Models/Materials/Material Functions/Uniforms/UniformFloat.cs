using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class UniformFloatFunc : MaterialFunction
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
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Constants",
                "Hardcodes a consant, unchanging value in the shader.", 
                "constant scalar float single parameter");
        }

        protected override string GetOperation()
        {
            return _value.ToString();
        }
    }
}
