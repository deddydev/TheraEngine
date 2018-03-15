using System;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public abstract class MaterialFunction
        : Function<MatFuncValueInput, MatFuncValueOutput, MatFuncExecInput, MatFuncExecOutput>
    {
        public MaterialFunction(bool deferControlArrangement = false) : base(true) { }

        public string Two(ShaderVarType type)
        {
            if (!IsType(type, BooleanTypes))
                switch (type)
                {
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

            throw new ArgumentException();
        }
        public string Zero(ShaderVarType type)
        {
            switch (type)
            {
                case ShaderVarType._bool: return "false";
                case ShaderVarType._bvec2: return "bvec2(false)";
                case ShaderVarType._bvec3: return "bvec3(false)";
                case ShaderVarType._bvec4: return "bvec4(false)";
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
            if (IsType(type, FloatingPointTypes))
                switch (type)
                {
                    case ShaderVarType._float: return "0.5f";
                    case ShaderVarType._vec2: return "vec2(0.5f)";
                    case ShaderVarType._vec3: return "vec3(0.5f)";
                    case ShaderVarType._vec4: return "vec4(0.5f)";
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
        
        public static readonly ShaderVarType[] SignedIntTypes = new ShaderVarType[]
        {
            ShaderVarType._int,
            ShaderVarType._ivec2,
            ShaderVarType._ivec3,
            ShaderVarType._ivec4,
        };
        public static readonly ShaderVarType[] UnsignedIntTypes = new ShaderVarType[]
        {
            ShaderVarType._uint,
            ShaderVarType._uvec2,
            ShaderVarType._uvec3,
            ShaderVarType._uvec4,
        };
        public static readonly ShaderVarType[] IntegerTypes = new ShaderVarType[]
        {
            ShaderVarType._int,
            ShaderVarType._uint,
            ShaderVarType._ivec2,
            ShaderVarType._uvec2,
            ShaderVarType._ivec3,
            ShaderVarType._uvec3,
            ShaderVarType._ivec4,
            ShaderVarType._uvec4,
        };
        public static readonly ShaderVarType[] FloatingPointTypes = new ShaderVarType[]
        {
            ShaderVarType._float,
            ShaderVarType._double,
            ShaderVarType._vec2,
            ShaderVarType._dvec2,
            ShaderVarType._vec3,
            ShaderVarType._dvec3,
            ShaderVarType._vec4,
            ShaderVarType._dvec4,
        };
        public static readonly ShaderVarType[] NumericTypes = new ShaderVarType[]
        {
            ShaderVarType._float,
            ShaderVarType._double,
            ShaderVarType._int,
            ShaderVarType._uint,
            ShaderVarType._vec2,
            ShaderVarType._ivec2,
            ShaderVarType._uvec2,
            ShaderVarType._dvec2,
            ShaderVarType._vec3,
            ShaderVarType._ivec3,
            ShaderVarType._uvec3,
            ShaderVarType._dvec3,
            ShaderVarType._vec4,
            ShaderVarType._ivec4,
            ShaderVarType._uvec4,
            ShaderVarType._dvec4,
        };
        public static readonly ShaderVarType[] SignedTypes = new ShaderVarType[]
        {
            ShaderVarType._float,
            ShaderVarType._double,
            ShaderVarType._int,
            ShaderVarType._vec2,
            ShaderVarType._ivec2,
            ShaderVarType._dvec2,
            ShaderVarType._vec3,
            ShaderVarType._ivec3,
            ShaderVarType._dvec3,
            ShaderVarType._vec4,
            ShaderVarType._ivec4,
            ShaderVarType._dvec4,
        };
        public static readonly ShaderVarType[] BooleanTypes = new ShaderVarType[]
        {
            ShaderVarType._bool,
            ShaderVarType._bvec2,
            ShaderVarType._bvec3,
            ShaderVarType._bvec4,
        };
        public static readonly ShaderVarType[] VectorTypes = new ShaderVarType[]
        {
            ShaderVarType._vec2,
            ShaderVarType._ivec2,
            ShaderVarType._uvec2,
            ShaderVarType._dvec2,
            ShaderVarType._bvec2,
            ShaderVarType._vec3,
            ShaderVarType._ivec3,
            ShaderVarType._uvec3,
            ShaderVarType._dvec3,
            ShaderVarType._bvec3,
            ShaderVarType._vec4,
            ShaderVarType._ivec4,
            ShaderVarType._uvec4,
            ShaderVarType._dvec4,
            ShaderVarType._bvec4,
        };
    }
}
