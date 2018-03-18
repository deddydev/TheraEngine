using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class BreakVec2 : ShaderMethod
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
    public class BreakVec3 : ShaderMethod
    {
        protected override string GetOperation()
        {
            throw new NotImplementedException();
        }
        protected override MatFuncValueInput[] GetValueInputs()
        {
            return new MatFuncValueInput[]
            {
                new MatFuncValueInput("Vector", ShaderVarType._vec3),
            };
        }
    }
    public class BreakVec4 : ShaderMethod
    {
        protected override string GetOperation()
        {
            throw new NotImplementedException();
        }
        protected override MatFuncValueInput[] GetValueInputs()
        {
            return new MatFuncValueInput[]
            {
                new MatFuncValueInput("Vector", ShaderVarType._vec4),
            };
        }
        protected override MatFuncValueOutput[] GetValueOutputs()
        {
            return new MatFuncValueOutput[]
            {
                new MatFuncValueOutput("X", ShaderVarType._float),
                new MatFuncValueOutput("Y", ShaderVarType._float),
                new MatFuncValueOutput("Z", ShaderVarType._float),
                new MatFuncValueOutput("W", ShaderVarType._float),
            };
        }
    }
}
