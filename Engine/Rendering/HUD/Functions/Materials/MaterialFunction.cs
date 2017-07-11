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
                    ShaderType type = (ShaderType)_outputs[i].CurrentArgumentType;
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
        
        public static readonly ShaderType[] SignedIntTypes = new ShaderType[]
        {
            ShaderType._int,
            ShaderType._ivec2,
            ShaderType._ivec3,
            ShaderType._ivec4,
        };
        public static readonly ShaderType[] UnsignedIntTypes = new ShaderType[]
        {
            ShaderType._uint,
            ShaderType._uvec2,
            ShaderType._uvec3,
            ShaderType._uvec4,
        };
        public static readonly ShaderType[] IntegerTypes = new ShaderType[]
        {
            ShaderType._int,
            ShaderType._uint,
            ShaderType._ivec2,
            ShaderType._uvec2,
            ShaderType._ivec3,
            ShaderType._uvec3,
            ShaderType._ivec4,
            ShaderType._uvec4,
        };
        public static readonly ShaderType[] FloatingPointTypes = new ShaderType[]
        {
            ShaderType._float,
            ShaderType._double,
            ShaderType._vec2,
            ShaderType._dvec2,
            ShaderType._vec3,
            ShaderType._dvec3,
            ShaderType._vec4,
            ShaderType._dvec4,
        };
        public static readonly ShaderType[] NumericTypes = new ShaderType[]
        {
            ShaderType._float,
            ShaderType._double,
            ShaderType._int,
            ShaderType._uint,
            ShaderType._vec2,
            ShaderType._ivec2,
            ShaderType._uvec2,
            ShaderType._dvec2,
            ShaderType._vec3,
            ShaderType._ivec3,
            ShaderType._uvec3,
            ShaderType._dvec3,
            ShaderType._vec4,
            ShaderType._ivec4,
            ShaderType._uvec4,
            ShaderType._dvec4,
        };
        public static readonly ShaderType[] SignedTypes = new ShaderType[]
        {
            ShaderType._float,
            ShaderType._double,
            ShaderType._int,
            ShaderType._vec2,
            ShaderType._ivec2,
            ShaderType._dvec2,
            ShaderType._vec3,
            ShaderType._ivec3,
            ShaderType._dvec3,
            ShaderType._vec4,
            ShaderType._ivec4,
            ShaderType._dvec4,
        };
        public static readonly ShaderType[] BooleanTypes = new ShaderType[]
        {
            ShaderType._bool,
            ShaderType._bvec2,
            ShaderType._bvec3,
            ShaderType._bvec4,
        };
    }
}
