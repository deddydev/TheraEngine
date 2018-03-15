using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class BreakVec2 : MaterialFunction
    {
        protected override string GetOperation()
        {
            throw new NotImplementedException();
        }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            return new List<MatFuncValueInput>()
            {
                new MatFuncValueInput("Vector", ShaderVarType._vec2),
            };
        }
    }
    public class BreakVec3 : MaterialFunction
    {
        protected override string GetOperation()
        {
            throw new NotImplementedException();
        }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            return new List<MatFuncValueInput>()
            {
                new MatFuncValueInput("Vector", ShaderVarType._vec3),
            };
        }
    }
    public class BreakVec4 : MaterialFunction
    {
        protected override string GetOperation()
        {

        }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            return new List<MatFuncValueInput>()
            {
                new MatFuncValueInput("Vector", ShaderVarType._vec4),
            };
        }
        protected override List<MatFuncValueOutput> GetValueOutputs()
        {
            return new List<MatFuncValueOutput>()
            {
                new MatFuncValueOutput("X", ShaderVarType._float),
                new MatFuncValueOutput("Y", ShaderVarType._float),
                new MatFuncValueOutput("Z", ShaderVarType._float),
                new MatFuncValueOutput("W", ShaderVarType._float),
            };
        }
    }
}
