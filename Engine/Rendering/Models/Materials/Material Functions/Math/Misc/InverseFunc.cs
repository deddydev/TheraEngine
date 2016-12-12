using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    /// <summary>
    /// returns the absolute value of the input value
    /// </summary>
    public class InverseFunc : MaterialFunction
    {
        GLInput InputValue;
        GLOutput Result;
        
        public InverseFunc() : base(true) { }
        protected override string GetOperation()
        {
            switch (InputValue.CurrentArgumentType)
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
        protected override List<GLInput> GetInputs()
        {
            InputValue = new GLInput("Value", FloatingPointTypes);
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
                "Inverse",
                "Returns 1.0 / value.", 
                "inverse divided divison one 1 over value");
        }
    }
}
