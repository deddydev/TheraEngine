using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
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
        FuncValueInput InputValue;
        FuncValueOutput Result;

        public OneMinusFunc() : base(true) { }
        protected override string GetOperation()
        {
            switch (InputValue.CurrentArgumentType)
            {
                case GLTypeName._float:
                case GLTypeName._double:
                    return "1.0 - {0}";
                case GLTypeName._int:
                case GLTypeName._uint:
                    return "1 - {0}";
                case GLTypeName._vec2:
                    return "vec2(1.0) - {0}";
                case GLTypeName._dvec2:
                    return "dvec2(1.0) - {0}";
                case GLTypeName._ivec2:
                    return "ivec2(1) - {0}";
                case GLTypeName._uvec2:
                    return "uvec2(1) - {0}";
                case GLTypeName._vec3:
                    return "vec3(1.0) - {0}";
                case GLTypeName._dvec3:
                    return "dvec3(1.0) - {0}";
                case GLTypeName._ivec3:
                    return "ivec3(1) - {0}";
                case GLTypeName._uvec3:
                    return "uvec3(1) - {0}";
                case GLTypeName._vec4:
                    return "vec4(1.0) - {0}";
                case GLTypeName._dvec4:
                    return "dvec4(1.0) - {0}";
                case GLTypeName._ivec4:
                    return "ivec4(1) - {0}";
                case GLTypeName._uvec4:
                    return "uvec4(1) - {0}";
            }
            throw new InvalidOperationException();
        }
        protected override List<FuncValueInput> GetInputs()
        {
            InputValue = new FuncValueInput("Value", NumericTypes);
            return new List<FuncValueInput>() { InputValue };
        }
        protected override List<FuncValueOutput> GetOutputs()
        {
            Result = new FuncValueOutput("Result", InputValue);
            return new List<FuncValueOutput>() { Result };
        }
    }
}
