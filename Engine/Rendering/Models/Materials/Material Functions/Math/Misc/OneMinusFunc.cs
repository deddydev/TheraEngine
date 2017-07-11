using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering.Models.Materials
{
    /// <summary>
    /// 1.0f - input
    /// </summary>
    [FunctionDefinition(
                "Math",
                "One Minus Value",
                "Returns 1.0f - value.",
                "one minus value 1 - subtract")]
    public class OneMinusFunc : MaterialFunction
    {
        MatFuncValueInput InputValue;
        MatFuncValueOutput Result;

        public OneMinusFunc() : base(true) { }
        protected override string GetOperation()
        {
            switch ((ShaderType)InputValue.CurrentArgumentType)
            {
                case ShaderType._float:
                case ShaderType._double:
                    return "1.0 - {0}";
                case ShaderType._int:
                case ShaderType._uint:
                    return "1 - {0}";
                case ShaderType._vec2:
                    return "vec2(1.0) - {0}";
                case ShaderType._dvec2:
                    return "dvec2(1.0) - {0}";
                case ShaderType._ivec2:
                    return "ivec2(1) - {0}";
                case ShaderType._uvec2:
                    return "uvec2(1) - {0}";
                case ShaderType._vec3:
                    return "vec3(1.0) - {0}";
                case ShaderType._dvec3:
                    return "dvec3(1.0) - {0}";
                case ShaderType._ivec3:
                    return "ivec3(1) - {0}";
                case ShaderType._uvec3:
                    return "uvec3(1) - {0}";
                case ShaderType._vec4:
                    return "vec4(1.0) - {0}";
                case ShaderType._dvec4:
                    return "dvec4(1.0) - {0}";
                case ShaderType._ivec4:
                    return "ivec4(1) - {0}";
                case ShaderType._uvec4:
                    return "uvec4(1) - {0}";
            }
            throw new InvalidOperationException();
        }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            InputValue = new MatFuncValueInput("Value", NumericTypes);
            return new List<MatFuncValueInput>() { InputValue };
        }
        protected override List<MatFuncValueOutput> GetValueOutputs()
        {
            Result = new MatFuncValueOutput("Result", InputValue);
            return new List<MatFuncValueOutput>() { Result };
        }
    }
}
