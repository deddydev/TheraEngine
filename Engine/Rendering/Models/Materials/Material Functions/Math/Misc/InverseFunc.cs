using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials
{
    /// <summary>
    /// Returns 1.0f divided by the input value
    /// </summary>
    [FunctionDefinition(
                "Math",
                "Inverse",
                "Returns 1.0 / value.",
                "inverse divided divison one 1 over value")]
    public class InverseFunc : MaterialFunction
    {
        MatFuncValueInput InputValue;
        MatFuncValueOutput Result;
        
        public InverseFunc() : base(true) { }
        protected override string GetOperation()
        {
            switch ((GLTypeName)InputValue.CurrentArgumentType)
            {
                case GLTypeName._float:
                case GLTypeName._double:
                    return "1.0 / {0}";
                case GLTypeName._vec2:
                    return "vec2(1.0) / {0}";
                case GLTypeName._dvec2:
                    return "dvec2(1.0) / {0}";
                case GLTypeName._vec3:
                    return "vec3(1.0) / {0}";
                case GLTypeName._dvec3:
                    return "dvec3(1.0) / {0}";
                case GLTypeName._vec4:
                    return "vec4(1.0) / {0}";
                case GLTypeName._dvec4:
                    return "dvec4(1.0) / {0}";
            }
            throw new InvalidOperationException();
        }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            InputValue = new MatFuncValueInput("Value", FloatingPointTypes);
            return new List<MatFuncValueInput>() { InputValue };
        }
        protected override List<MatFuncValueOutput> GetValueOutputs()
        {
            Result = new MatFuncValueOutput("Result", InputValue);
            return new List<MatFuncValueOutput>() { Result };
        }
    }
}
