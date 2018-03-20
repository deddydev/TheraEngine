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
        public CommonParameterFunc() : base(ShaderVarType._float)
        {

        }

        public CommonParameterFunc(ECommonUniform value) : base(ShaderVarType._float) => Value = value;
        
        private ECommonUniform _value;
        private ShaderVarType Type
        {
            get => (ShaderVarType)OutputArguments[0].CurrentArgumentType;
            set => OutputArguments[0].AllowedArgumentTypes = new int[] { (int)value };
        }

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
                    case ECommonUniform.UpdateDelta:
                        Type = ShaderVarType._float;
                        break;

                    case ECommonUniform.ScreenOrigin:
                        Type = ShaderVarType._vec2;
                        break;

                    case ECommonUniform.CameraPosition:
                        //case ECommonUniform.CameraForward:
                        //case ECommonUniform.CameraUp:
                        //case ECommonUniform.CameraRight:
                        Type = ShaderVarType._vec3;
                        break;
                }
            }
        }
        public string GetDeclaration()
            => Type.ToString().Substring(1) + _value.ToString();
        protected override string GetOperation()
            => _value.ToString();
    }
}
