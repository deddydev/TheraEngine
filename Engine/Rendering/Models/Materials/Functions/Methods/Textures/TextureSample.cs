using System.Collections.Generic;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class TextureSample : ShaderMethod
    {
        public TextureSample() : base(ShaderVarType._vec4) { }
        protected override string GetOperation() => "texture({0}, {1})";
        protected override MatFuncValueInput[] GetValueInputs()
        {
            MatFuncValueInput uv = new MatFuncValueInput("UVs", ShaderVarType._vec2);
            return new MatFuncValueInput[] { uv };
        }
    }
}
