using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public abstract class MaterialFunction
        : Function<MatFuncValueInput, MatFuncValueOutput, MatFuncExecInput, MatFuncExecOutput>
    {
        public MaterialFunction(bool deferControlArrangement = false) : base(deferControlArrangement) { }

        public string Two(ShaderVarType type)
        {
            if (!IsType(type, ShaderVar.BooleanTypes))
                switch (type)
                {
                    default:
                    case ShaderVarType._int: return "2";
                    case ShaderVarType._ivec2: return "ivec2(2)";
                    case ShaderVarType._ivec3: return "ivec3(2)";
                    case ShaderVarType._ivec4: return "ivec4(2)";
                    case ShaderVarType._uint: return "2";
                    case ShaderVarType._uvec2: return "uvec2(2)";
                    case ShaderVarType._uvec3: return "uvec3(2)";
                    case ShaderVarType._uvec4: return "uvec4(2)";
                    case ShaderVarType._float: return "2.0f";
                    case ShaderVarType._vec2: return "vec2(2.0f)";
                    case ShaderVarType._vec3: return "vec3(2.0f)";
                    case ShaderVarType._vec4: return "vec4(2.0f)";
                    case ShaderVarType._double: return "2.0";
                    case ShaderVarType._dvec2: return "dvec2(2.0)";
                    case ShaderVarType._dvec3: return "dvec3(2.0)";
                    case ShaderVarType._dvec4: return "dvec4(2.0)";
                    case ShaderVarType._mat3: throw new NotImplementedException();
                    case ShaderVarType._mat4: throw new NotImplementedException();
                }

            throw new ArgumentException();
        }
        public string One(ShaderVarType type)
        {
            switch (type)
            {
                case ShaderVarType._bool: return "true";
                case ShaderVarType._bvec2: return "bvec2(true)";
                case ShaderVarType._bvec3: return "bvec3(true)";
                case ShaderVarType._bvec4: return "bvec4(true)";
                default:
                case ShaderVarType._int: return "1";
                case ShaderVarType._ivec2: return "ivec2(1)";
                case ShaderVarType._ivec3: return "ivec3(1)";
                case ShaderVarType._ivec4: return "ivec4(1)";
                case ShaderVarType._uint: return "1";
                case ShaderVarType._uvec2: return "uvec2(1)";
                case ShaderVarType._uvec3: return "uvec3(1)";
                case ShaderVarType._uvec4: return "uvec4(1)";
                case ShaderVarType._float:  return "1.0f";
                case ShaderVarType._vec2: return "vec2(1.0f)";
                case ShaderVarType._vec3: return "vec3(1.0f)";
                case ShaderVarType._vec4: return "vec4(1.0f)";
                case ShaderVarType._double: return "1.0";
                case ShaderVarType._dvec2:  return "dvec2(1.0)";
                case ShaderVarType._dvec3:  return "dvec3(1.0)";
                case ShaderVarType._dvec4:  return "dvec4(1.0)";
                case ShaderVarType._mat3: throw new NotImplementedException();
                case ShaderVarType._mat4: throw new NotImplementedException();
            }

            //throw new ArgumentException();
        }
        public string Zero(ShaderVarType type)
        {
            switch (type)
            {
                case ShaderVarType._bool: return "false";
                case ShaderVarType._bvec2: return "bvec2(false)";
                case ShaderVarType._bvec3: return "bvec3(false)";
                case ShaderVarType._bvec4: return "bvec4(false)";
                default:
                case ShaderVarType._int: return "0";
                case ShaderVarType._ivec2: return "ivec2(0)";
                case ShaderVarType._ivec3: return "ivec3(0)";
                case ShaderVarType._ivec4: return "ivec4(0)";
                case ShaderVarType._uint: return "0";
                case ShaderVarType._uvec2: return "uvec2(0)";
                case ShaderVarType._uvec3: return "uvec3(0)";
                case ShaderVarType._uvec4: return "uvec4(0)";
                case ShaderVarType._float: return "0.0f";
                case ShaderVarType._vec2: return "vec2(0.0f)";
                case ShaderVarType._vec3: return "vec3(0.0f)";
                case ShaderVarType._vec4: return "vec4(0.0f)";
                case ShaderVarType._double: return "0.0";
                case ShaderVarType._dvec2: return "dvec2(0.0)";
                case ShaderVarType._dvec3: return "dvec3(0.0)";
                case ShaderVarType._dvec4: return "dvec4(0.0)";
                case ShaderVarType._mat3: throw new NotImplementedException();
                case ShaderVarType._mat4: throw new NotImplementedException();
            }

            throw new ArgumentException();
        }
        public string Half(ShaderVarType type)
        {
            if (IsType(type, ShaderVar.FloatingPointTypes))
                switch (type)
                {
                    case ShaderVarType._float: return "0.5f";
                    case ShaderVarType._vec2: return "vec2(0.5f)";
                    case ShaderVarType._vec3: return "vec3(0.5f)";
                    case ShaderVarType._vec4: return "vec4(0.5f)";
                    default:
                    case ShaderVarType._double: return "0.5";
                    case ShaderVarType._dvec2: return "dvec2(0.5)";
                    case ShaderVarType._dvec3: return "dvec3(0.5)";
                    case ShaderVarType._dvec4: return "dvec4(0.5)";
                    case ShaderVarType._mat3: throw new NotImplementedException();
                    case ShaderVarType._mat4: throw new NotImplementedException();
                }

            throw new ArgumentException();
        }

        public static bool IsType(ShaderVarType type, ShaderVarType[] comparedTypes)
            => comparedTypes.Contains(type);
        
        public void CollectInputTreeRecursive(HashSet<MaterialFunction> tree)
        {
            if (tree.Add(this))
                foreach (var input in InputArguments)
                    if (input.Connection != null)
                        input.Connection.ParentSocket.CollectInputTreeRecursive(tree);
        }
    }
}
