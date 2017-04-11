using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    [FunctionDefinition(
                "Constants",
                "Common Parameter",
                "Provides a commom engine parameter value to the shader.",
                "constant scalar parameter")]
    public class CommonParameterFunc : MaterialFunction
    {
        public CommonParameterFunc() : base(true) { }
        public CommonParameterFunc(ECommonUniform value) : base(true) { _value = value; }

        ECommonUniform _value;
        GLTypeName _type;

        public ECommonUniform Value
        {
            get => _value;
            set
            {
                switch (_value = value)
                {
                    case ECommonUniform.ScreenHeight:
                    case ECommonUniform.ScreenWidth:
                    case ECommonUniform.CameraFovY:
                    case ECommonUniform.CameraFovX:
                    case ECommonUniform.CameraFarZ:
                    case ECommonUniform.CameraNearZ:
                    case ECommonUniform.CameraAspect:
                    case ECommonUniform.RenderDelta:
                        _type = GLTypeName._float;
                        break;

                    case ECommonUniform.ScreenOrigin:
                        _type = GLTypeName._vec2;
                        break;

                    case ECommonUniform.CameraPosition:
                    case ECommonUniform.CameraForward:
                    case ECommonUniform.CameraUp:
                    case ECommonUniform.CameraRight:
                        _type = GLTypeName._vec3;
                        break;
                }
            }
        }
        public string GetDeclaration()
            => _type.ToString().Substring(1) + _value.ToString();
        protected override string GetOperation()
            => _value.ToString();
    }
}
