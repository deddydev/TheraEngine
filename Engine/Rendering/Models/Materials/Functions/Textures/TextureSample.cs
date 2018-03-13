using System.Collections.Generic;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class TextureSample : MaterialFunction
    {
        public TextureSample() : base(true) { }
        protected override string GetOperation()
        {
            return "texture({0}, {1})";
        }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            return new List<MatFuncValueInput>()
            {
                new MatFuncValueInput("UVs", ShaderVarType._vec2),
            };
        }
        protected override List<MatFuncValueOutput> GetValueOutputs()
        {
            return new List<MatFuncValueOutput>()
            {
                new MatFuncValueOutput("Color", ShaderVarType._vec4),
            };
        }
    }
}
