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
    public class OneMinusFunc : MaterialFunction
    {
        GLInput InputValue;
        GLOutput Result;

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
        protected override List<GLInput> GetInputs()
        {
            InputValue = new GLInput("Value", NumericTypes);
            return new List<GLInput>() { InputValue };
        }
        protected override List<GLOutput> GetOutputs()
        {
            Result = new GLOutput("Result", InputValue);
            return new List<GLOutput>() { Result };
        }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Math",
                "One Minus Value",
                "Returns 1.0f - value.", 
                "one minus value 1 - subtract");
        }
    }
}
