using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
                "Constants",
                "Common Parameter",
                "Provides a commom engine parameter value to the shader.",
                "constant scalar parameter")]
    public class CommonParameterFunc : ShaderMethod
    {
        public CommonParameterFunc() : base() { }
        public CommonParameterFunc(ECommonUniform value) : base()
        {
            Value = value;
        }

        private ECommonUniform _value;
        private ShaderVarType _type;

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
                        _type = ShaderVarType._float;
                        break;

                    case ECommonUniform.ScreenOrigin:
                        _type = ShaderVarType._vec2;
                        break;

                    case ECommonUniform.CameraPosition:
                    //case ECommonUniform.CameraForward:
                    //case ECommonUniform.CameraUp:
                    //case ECommonUniform.CameraRight:
                        _type = ShaderVarType._vec3;
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
