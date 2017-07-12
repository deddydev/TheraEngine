using System;

namespace TheraEngine.Rendering.Models.Materials
{
    public abstract class MaterialFunction
        : Function<MatFuncValueInput, MatFuncValueOutput, MatFuncExecInput, MatFuncExecOutput>
    {
        /// <summary>
        /// Determines if this function can be contained within one line.
        /// Otherwise written as a method located outside of main().
        /// Ex: type thing = FUNC_OPERATION;
        /// otherwise:
        /// type FUNC_NAME(in/out type args) { FUNC_OPERATION }
        /// </summary>
        protected bool _inline = false;
        
        public MaterialFunction(bool inline) : base()
        {
            _inline = inline;
        }
        
        /// <summary>
        /// Returns the base operation for string.Format.
        /// </summary>
        protected abstract string GetOperation();
        
        public string GetLineOperation(
            string[] inputNames,
            string[] outputNames,
            bool declareOutputs = false)
        {
            if (inputNames.Length != _inputs.Count ||
                outputNames.Length != _outputs.Count)
                throw new InvalidOperationException();

            if (_inline)
                return string.Format(GetOperation(), inputNames);

            string s = "\n";
            if (declareOutputs)
                for (int i = 0; i < _outputs.Count; ++i)
                {
                    string name = outputNames[i];
                    ShaderVarType type = (ShaderVarType)_outputs[i].CurrentArgumentType;
                    s += type + " " + name + ";\n";
                }

            s += Name + "(";
            for (int i = 0; i < _inputs.Count; ++i)
            {
                s += inputNames[i];
                if (i != _inputs.Count - 1)
                    s += ", ";
            }
            if (_outputs.Count > 0)
                s += ", ";
            for (int i = 0; i < _outputs.Count; ++i)
            {
                s += outputNames[i];
                if (i != _outputs.Count - 1)
                    s += ", ";
            }
            s += ")";

            return s;
        }
        public string GetGlobalMethodDeclaration()
        {
            string s = "void " + FunctionName + "(";
            bool first = true;
            foreach (MatFuncValueInput arg in InputArguments)
            {
                if (first)
                    first = false;
                else
                    s += ", ";
                s += "in " + arg.CurrentArgumentType.ToString().Substring(1) + " " + arg.Name;
            }
            foreach (MatFuncValueOutput arg in OutputArguments)
            {
                if (first)
                    first = false;
                else
                    s += ", ";
                s += "out " + arg.CurrentArgumentType.ToString().Substring(1) + " " + arg.Name;
            }
            s += ")\n{\n" + GetOperation() + "\n}\n";
            return s;
        }
        
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
    }
}
