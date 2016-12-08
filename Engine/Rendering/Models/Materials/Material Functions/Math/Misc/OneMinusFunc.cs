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
        public GLInput InputValue { get { return InputArguments[0]; } }
        
        public OneMinusFunc() : base()
        {
            _inline = true;
        }
        protected override string GetOperation()
        {
            switch (InputValue.CurrentArgumentType)
            {
                case GLTypeName._float:
                    return "1.0 - {0}";
                case GLTypeName._vec2:
                    return "vec2(1.0) - {0}";
                case GLTypeName._vec3:
                    return "vec3(1.0) - {0}";
                case GLTypeName._vec4:
                    return "vec4(1.0) - {0}";
            }
            throw new InvalidOperationException();
        }
        protected override List<GLInput> GetInputs()
        {
            return new List<GLInput>()
            {
                new GLInput("Value", GLTypeName._float, GLTypeName._vec2, GLTypeName._vec3, GLTypeName._vec4)
            };
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
