using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public enum CommonParameter
    {
        ScreenWidth,
        ScreenHeight,
        FovY,
        FovX,
        Aspect,
        RenderDelta,
    }
    public class CommonParameterFunc : MaterialFunction
    {
        public CommonParameterFunc() : base() { }
        public CommonParameterFunc(CommonParameter value) { _value = value; }

        CommonParameter _value;
        GLTypeName _type;

        public CommonParameter Value
        {
            get { return _value; }
            set
            {
                switch (_value = value)
                {
                    case CommonParameter.ScreenHeight:
                    case CommonParameter.ScreenWidth:
                    case CommonParameter.FovY:
                    case CommonParameter.FovX:
                    case CommonParameter.Aspect:
                    case CommonParameter.RenderDelta:
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
                "Provides a commom engine parameter value to the shader.", 
                "constant scalar parameter");
        }
        protected override string GetOperation()
        {
            return _value.ToString();
        }
    }
}
