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
        public List<int> CurrentValidOverloads { get; } = new List<int>();
        
        public abstract void GetDefinition(out string[] inputNames, out string[] outputNames, out MatFuncOverload[] overloads);
        protected override void CollectArguments()
        {
            GetDefinition(out string[] inputNames, out string[] outputNames, out MatFuncOverload[] overloads);

            Overloads = overloads;
            ResetValidOverloads();

            //if (overloads.Where(x => x.Inputs.Length != inputNames.Length || x.Outputs.Length != outputNames.Length).ToArray().Length > 0)
            //    throw new InvalidOperationException();

            foreach (string inputName in inputNames)
                AddValueInput(new MatFuncValueInput(inputName, this));
            foreach (string outputName in outputNames)
                AddValueOutput(new MatFuncValueOutput(outputName, this));

            ArrangeControls();
        }

        public static bool CanConnect(MatFuncValueInput input, MatFuncValueOutput output)
        {
            return true;
            //if (input is null || output is null)
            //    return false;

            //MaterialFunction inFunc = input.ParentSocket;
            //MaterialFunction outFunc = output.ParentSocket;
            //for (int i = 0; i < outFunc.CurrentValidOverloads.Count; ++i)
            //{
            //    MatFuncOverload outOverload = outFunc.Overloads[i];
            //    for (int x = 0; x < inFunc.CurrentValidOverloads.Count; ++x)
            //    {
            //        MatFuncOverload inOverload = inFunc.Overloads[x];
            //        foreach (EGenShaderVarType outGen in outOverload.Outputs)
            //            foreach (EGenShaderVarType inGen in inOverload.Inputs)
            //                if ((outGen & inGen) != 0)
            //                    return true;
            //    }
            //}
            //return false;
        }

        internal void RecalcValidOverloads(bool[] validTypes)
        {
            CurrentValidOverloads.Clear();
            int r;
            for (int i = 0, x = 0; i < validTypes.Length; ++i, ++x)
                if (!validTypes[i] && (r = CurrentValidOverloads.IndexOf(i)) >= 0)
                    CurrentValidOverloads.RemoveAt(r);
        }
        public void ResetValidOverloads()
        {
            CurrentValidOverloads.Clear();
            for (int i = 0; i < Overloads.Length; ++i)
                CurrentValidOverloads.Add(i);
        }
        public void RecalcValidOverloads()
        {
            CurrentValidOverloads.Clear();
            //foreach (MatFuncValueInput input in _valueInputs)
            //{
            //    if (input.HasConnection)
            //    {
                    
            //    }
            //}
            foreach (MatFuncValueOutput output in _valueOutputs)
            {
                if (output.HasConnection)
                {

                }
            }
            if (CurrentValidOverloads.Count == 1)
            {
                MatFuncOverload overload = Overloads[CurrentValidOverloads[0]];
                for (int i = 0; i < _valueInputs.Count; ++i)
                {
                    //_valueInputs[i].CurrentArgumentType = overload.Inputs[i];
                }
                foreach (MatFuncValueOutput output in _valueOutputs)
                {

                }
            }
            else
            {
                foreach (MatFuncValueInput input in _valueInputs)
                {
                    input.CurrentArgumentType = -1;
                }
                foreach (MatFuncValueOutput output in _valueOutputs)
                {
                    output.CurrentArgumentType = -1;
                }
            }
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
    public enum EGLSLVersion
    {
        Invalid = -1,

        /// <summary>
        /// OpenGL 2.0 April 2004
        /// </summary>
        Ver_110,
        /// <summary>
        /// OpenGL 2.1 September 2006
        /// </summary>
        Ver_120,
        /// <summary>
        /// OpenGL 3.0 August 2008
        /// </summary>
        Ver_130,
        /// <summary>
        /// OpenGL 3.1 March 2009
        /// </summary>
        Ver_140,
        /// <summary>
        /// OpenGL 3.2 August 2009
        /// </summary>
        Ver_150,
        /// <summary>
        /// OpenGL 3.3 February 2010
        /// </summary>
        Ver_330,
        /// <summary>
        /// OpenGL 4.0 March 2010
        /// </summary>
        Ver_400,
        /// <summary>
        /// OpenGL 4.1 July 2010
        /// </summary>
        Ver_410,
        /// <summary>
        /// OpenGL 4.2 August 2011
        /// </summary>
        Ver_420,
        /// <summary>
        /// OpenGL 4.3 August 2012
        /// </summary>
        Ver_430,
        /// <summary>
        /// OpenGL 4.4 July 2013
        /// </summary>
        Ver_440,
        /// <summary>
        /// OpenGL 4.5 August 2014
        /// </summary>
        Ver_450,
        /// <summary>
        /// OpenGL 4.6 July 2017
        /// </summary>
        Ver_460,
    }
    public enum EOpenGLVersion
    {
        Invalid = -1,

        /// <summary>
        /// 
        /// </summary>
        Ver_1_0,
        /// <summary>
        /// OpenGL 2.0 April 2004. GLSL 110
        /// </summary>
        Ver_2_0,
        /// <summary>
        /// OpenGL 2.1 September 2006. GLSL 120
        /// </summary>
        Ver_2_1,
        /// <summary>
        /// OpenGL 3.0 August 2008. GLSL 130
        /// </summary>
        Ver_3_0,
        /// <summary>
        /// OpenGL 3.1 March 2009. GLSL 140
        /// </summary>
        Ver_3_1,
        /// <summary>
        /// OpenGL 3.2 August 2009. GLSL 150
        /// </summary>
        Ver_3_2,
        /// <summary>
        /// OpenGL 3.3 February 2010. GLSL 330
        /// </summary>
        Ver_3_3,
        /// <summary>
        /// OpenGL 4.0 March 2010. GLSL 400
        /// </summary>
        Ver_4_0,
        /// <summary>
        /// OpenGL 4.1 July 2010. GLSL 410
        /// </summary>
        Ver_4_1,
        /// <summary>
        /// OpenGL 4.2 August 2011. GLSL 420
        /// </summary>
        Ver_4_2,
        /// <summary>
        /// OpenGL 4.3 August 2012. GLSL 430
        /// </summary>
        Ver_4_3,
        /// <summary>
        /// OpenGL 4.4 July 2013. GLSL 440
        /// </summary>
        Ver_4_4,
        /// <summary>
        /// OpenGL 4.5 August 2014. GLSL 450
        /// </summary>
        Ver_4_5,
        /// <summary>
        /// OpenGL 4.6 July 2017. GLSL 460
        /// </summary>
        Ver_4_6,
    }
}
