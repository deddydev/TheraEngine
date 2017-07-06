using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering.Models.Materials
{
    /// <summary>
    /// returns the absolute value of the input value
    /// </summary>
    [FunctionDefinition(
                "Math",
                "Negate",
                "Returns the negated value of the given value.",
                "negate negative value")]
    public class NegateFunc : MaterialFunction
    {
        MatFuncValueInput InputValue;
        MatFuncValueOutput Result;
        
        public NegateFunc() : base(true) { }
        protected override string GetOperation()
        {
            switch ((GLTypeName)InputValue.CurrentArgumentType)
            {
                case GLTypeName._float:
                case GLTypeName._double:
                    return "-1.0 * {0}";
                case GLTypeName._int:
                    return "-1 * {0}";
                case GLTypeName._vec2:
                    return "vec2(-1.0) * {0}";
                case GLTypeName._dvec2:
                    return "dvec2(-1.0) * {0}";
                case GLTypeName._ivec2:
                    return "ivec2(-1) * {0}";
                case GLTypeName._vec3:
                    return "vec3(-1.0) * {0}";
                case GLTypeName._dvec3:
                    return "dvec3(-1.0) * {0}";
                case GLTypeName._ivec3:
                    return "ivec3(-1) * {0}";
                case GLTypeName._vec4:
                    return "vec4(-1.0) * {0}";
                case GLTypeName._dvec4:
                    return "dvec4(-1.0) * {0}";
                case GLTypeName._ivec4:
                    return "ivec4(-1) * {0}";
            }
            throw new InvalidOperationException();
        }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            InputValue = new MatFuncValueInput("Value", SignedTypes);
            return new List<MatFuncValueInput>() { InputValue };
        }
        protected override List<MatFuncValueOutput> GetValueOutputs()
        {
            Result = new MatFuncValueOutput("Result", InputValue);
            return new List<MatFuncValueOutput>() { Result };
        }
    }
}
