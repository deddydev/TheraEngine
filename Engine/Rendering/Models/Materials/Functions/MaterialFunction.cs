using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public abstract class MaterialFunction
        : Function<MatFuncValueInput, MatFuncValueOutput, MatFuncExecInput, MatFuncExecOutput>
    {
        public MaterialFunction(bool deferControlArrangement = false) : base(deferControlArrangement) { }

        public MatFuncOverload[] Overloads { get; private set; }
        public int CurrentOverloadIndex { get; protected set; }

        public abstract void GetDefinition(out string[] inputNames, out string[] outputNames, out MatFuncOverload[] overloads);
        protected override void AddArguments()
        {
            GetDefinition(out string[] inputNames, out string[] outputNames, out MatFuncOverload[] overloads);
            Overloads = overloads;
            if (overloads.Where(x => x.Inputs.Length != inputNames.Length || x.Outputs.Length != outputNames.Length).ToArray().Length > 0)
                throw new InvalidOperationException();
        }

        public void CollectInputTreeRecursive(HashSet<MaterialFunction> tree)
        {
            if (tree.Add(this))
                foreach (var input in InputArguments)
                    if (input.Connection != null)
                        input.Connection.ParentSocket.CollectInputTreeRecursive(tree);
        }

        #region Statics
        public static string Two(EShaderVarType type)
        {
            if (!IsType(type, ShaderVar.BooleanTypes))
                switch (type)
                {
                    default:
                    case EShaderVarType._int: return "2";
                    case EShaderVarType._ivec2: return "ivec2(2)";
                    case EShaderVarType._ivec3: return "ivec3(2)";
                    case EShaderVarType._ivec4: return "ivec4(2)";
                    case EShaderVarType._uint: return "2";
                    case EShaderVarType._uvec2: return "uvec2(2)";
                    case EShaderVarType._uvec3: return "uvec3(2)";
                    case EShaderVarType._uvec4: return "uvec4(2)";
                    case EShaderVarType._float: return "2.0f";
                    case EShaderVarType._vec2: return "vec2(2.0f)";
                    case EShaderVarType._vec3: return "vec3(2.0f)";
                    case EShaderVarType._vec4: return "vec4(2.0f)";
                    case EShaderVarType._double: return "2.0";
                    case EShaderVarType._dvec2: return "dvec2(2.0)";
                    case EShaderVarType._dvec3: return "dvec3(2.0)";
                    case EShaderVarType._dvec4: return "dvec4(2.0)";
                    case EShaderVarType._mat3: throw new NotImplementedException();
                    case EShaderVarType._mat4: throw new NotImplementedException();
                }

            throw new ArgumentException();
        }
        public static string One(EShaderVarType type)
        {
            switch (type)
            {
                case EShaderVarType._bool: return "true";
                case EShaderVarType._bvec2: return "bvec2(true)";
                case EShaderVarType._bvec3: return "bvec3(true)";
                case EShaderVarType._bvec4: return "bvec4(true)";
                default:
                case EShaderVarType._int: return "1";
                case EShaderVarType._ivec2: return "ivec2(1)";
                case EShaderVarType._ivec3: return "ivec3(1)";
                case EShaderVarType._ivec4: return "ivec4(1)";
                case EShaderVarType._uint: return "1";
                case EShaderVarType._uvec2: return "uvec2(1)";
                case EShaderVarType._uvec3: return "uvec3(1)";
                case EShaderVarType._uvec4: return "uvec4(1)";
                case EShaderVarType._float: return "1.0f";
                case EShaderVarType._vec2: return "vec2(1.0f)";
                case EShaderVarType._vec3: return "vec3(1.0f)";
                case EShaderVarType._vec4: return "vec4(1.0f)";
                case EShaderVarType._double: return "1.0";
                case EShaderVarType._dvec2: return "dvec2(1.0)";
                case EShaderVarType._dvec3: return "dvec3(1.0)";
                case EShaderVarType._dvec4: return "dvec4(1.0)";
                case EShaderVarType._mat3: throw new NotImplementedException();
                case EShaderVarType._mat4: throw new NotImplementedException();
            }

            //throw new ArgumentException();
        }
        public static string Zero(EShaderVarType type)
        {
            switch (type)
            {
                case EShaderVarType._bool: return "false";
                case EShaderVarType._bvec2: return "bvec2(false)";
                case EShaderVarType._bvec3: return "bvec3(false)";
                case EShaderVarType._bvec4: return "bvec4(false)";
                default:
                case EShaderVarType._int: return "0";
                case EShaderVarType._ivec2: return "ivec2(0)";
                case EShaderVarType._ivec3: return "ivec3(0)";
                case EShaderVarType._ivec4: return "ivec4(0)";
                case EShaderVarType._uint: return "0";
                case EShaderVarType._uvec2: return "uvec2(0)";
                case EShaderVarType._uvec3: return "uvec3(0)";
                case EShaderVarType._uvec4: return "uvec4(0)";
                case EShaderVarType._float: return "0.0f";
                case EShaderVarType._vec2: return "vec2(0.0f)";
                case EShaderVarType._vec3: return "vec3(0.0f)";
                case EShaderVarType._vec4: return "vec4(0.0f)";
                case EShaderVarType._double: return "0.0";
                case EShaderVarType._dvec2: return "dvec2(0.0)";
                case EShaderVarType._dvec3: return "dvec3(0.0)";
                case EShaderVarType._dvec4: return "dvec4(0.0)";
                case EShaderVarType._mat3: throw new NotImplementedException();
                case EShaderVarType._mat4: throw new NotImplementedException();
            }

            throw new ArgumentException();
        }
        public static string Half(EShaderVarType type)
        {
            if (IsType(type, ShaderVar.DecimalTypes))
                switch (type)
                {
                    case EShaderVarType._float: return "0.5f";
                    case EShaderVarType._vec2: return "vec2(0.5f)";
                    case EShaderVarType._vec3: return "vec3(0.5f)";
                    case EShaderVarType._vec4: return "vec4(0.5f)";
                    default:
                    case EShaderVarType._double: return "0.5";
                    case EShaderVarType._dvec2: return "dvec2(0.5)";
                    case EShaderVarType._dvec3: return "dvec3(0.5)";
                    case EShaderVarType._dvec4: return "dvec4(0.5)";
                    case EShaderVarType._mat3: throw new NotImplementedException();
                    case EShaderVarType._mat4: throw new NotImplementedException();
                }

            throw new ArgumentException();
        }

        public static bool IsType(EShaderVarType type, EShaderVarType[] comparedTypes)
            => comparedTypes.Contains(type);

        #endregion
    }
    public class MatFuncOverload
    {
        public EGLSLVersion Version { get; }
        public EGenShaderVarType[] Inputs { get; }
        public EGenShaderVarType[] Outputs { get; }
        public MatFuncOverload(EGLSLVersion version, EGenShaderVarType[] outputs, EGenShaderVarType[] inputs)
        {
            Version = version;
            Inputs = inputs;
            Outputs = outputs;
        }
        public MatFuncOverload(EGLSLVersion version, EGenShaderVarType output, EGenShaderVarType[] inputs)
        {
            Version = version;
            Inputs = inputs;
            Outputs = new EGenShaderVarType[] { output };
        }
        public MatFuncOverload(EGLSLVersion version, EGenShaderVarType[] inout)
        {
            Version = version;
            Inputs = inout;
            Outputs = inout;
        }
        public MatFuncOverload(EGLSLVersion version, EGenShaderVarType inout)
        {
            Version = version;
            Inputs = new EGenShaderVarType[] { inout };
            Outputs = new EGenShaderVarType[] { inout };
        }
        public MatFuncOverload(EGLSLVersion version, EGenShaderVarType[] inOrOutOnly, bool isIn)
        {
            Version = version;
            Inputs = isIn ? inOrOutOnly : new EGenShaderVarType[0];
            Outputs = !isIn ? inOrOutOnly : new EGenShaderVarType[0];
        }
        public MatFuncOverload(EGLSLVersion version, EGenShaderVarType inOrOutOnly, bool isIn)
        {
            Version = version;
            Inputs = isIn ? new EGenShaderVarType[] { inOrOutOnly } : new EGenShaderVarType[0];
            Outputs = !isIn ? new EGenShaderVarType[] { inOrOutOnly } : new EGenShaderVarType[0];
        }
    }
}
