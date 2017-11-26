using System;
using System.Collections.Generic;
using TheraEngine.Rendering.UI.Functions;

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
            switch ((ShaderVarType)InputValue.CurrentArgumentType)
            {
                case ShaderVarType._float:
                case ShaderVarType._double:
                    return "1.0 - {0}";
                case ShaderVarType._int:
                case ShaderVarType._uint:
                    return "1 - {0}";
                case ShaderVarType._vec2:
                    return "vec2(1.0) - {0}";
                case ShaderVarType._dvec2:
                    return "dvec2(1.0) - {0}";
                case ShaderVarType._ivec2:
                    return "ivec2(1) - {0}";
                case ShaderVarType._uvec2:
                    return "uvec2(1) - {0}";
                case ShaderVarType._vec3:
                    return "vec3(1.0) - {0}";
                case ShaderVarType._dvec3:
                    return "dvec3(1.0) - {0}";
                case ShaderVarType._ivec3:
                    return "ivec3(1) - {0}";
                case ShaderVarType._uvec3:
                    return "uvec3(1) - {0}";
                case ShaderVarType._vec4:
                    return "vec4(1.0) - {0}";
                case ShaderVarType._dvec4:
                    return "dvec4(1.0) - {0}";
                case ShaderVarType._ivec4:
                    return "ivec4(1) - {0}";
                case ShaderVarType._uvec4:
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
