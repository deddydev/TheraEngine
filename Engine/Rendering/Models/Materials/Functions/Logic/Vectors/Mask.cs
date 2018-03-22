using System;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
        "Vectors",
        "Mask",
        "Masks out components from a vector.",
        "mask vector components select remove choose break")]
    public class Mask : ShaderMethod
    {
        protected override string GetOperation()
        {
            throw new NotImplementedException();
        }
        protected override MatFuncValueInput[] GetValueInputs()
        {
            return new MatFuncValueInput[]
            {
                new MatFuncValueInput("Vector", ShaderVarType._vec2),
            };
        }
    }
}
