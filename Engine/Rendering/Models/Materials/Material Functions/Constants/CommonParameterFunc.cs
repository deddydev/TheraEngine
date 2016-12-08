using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class CommonParameterFunc : MaterialFunction
    {
        public CommonParameterFunc() : base() { }
        public CommonParameterFunc(ECommonUniform value) { _value = value; }

        ECommonUniform _value;
        GLTypeName _type;

        public ECommonUniform Value
        {
            get { return _value; }
            set
            {
                switch (_value = value)
                {
                    case ECommonUniform.ScreenHeight:
                    case ECommonUniform.ScreenWidth:
                    case ECommonUniform.FovY:
                    case ECommonUniform.FovX:
                    case ECommonUniform.Aspect:
                    case ECommonUniform.RenderDelta:
                        _type = GLTypeName._float;
                        break;
                }
            }
        }

        public string GetDeclaration()
        {
            return _type.ToString().Substring(1) + _value.ToString();
        }
        
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Constants",
                "Common Parameter",
                "Provides a commom engine parameter value to the shader.", 
                "constant scalar parameter");
        }
        protected override string GetOperation()
        {
            return _value.ToString();
        }
    }
}
