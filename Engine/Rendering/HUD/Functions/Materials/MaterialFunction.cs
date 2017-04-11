using System;

namespace CustomEngine.Rendering.Models.Materials
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
                    GLTypeName type = (GLTypeName)_outputs[i].CurrentArgumentType;
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
        
        public static readonly GLTypeName[] SignedIntTypes = new GLTypeName[]
        {
            GLTypeName._int,
            GLTypeName._ivec2,
            GLTypeName._ivec3,
            GLTypeName._ivec4,
        };
        public static readonly GLTypeName[] UnsignedIntTypes = new GLTypeName[]
        {
            GLTypeName._uint,
            GLTypeName._uvec2,
            GLTypeName._uvec3,
            GLTypeName._uvec4,
        };
        public static readonly GLTypeName[] IntegerTypes = new GLTypeName[]
        {
            GLTypeName._int,
            GLTypeName._uint,
            GLTypeName._ivec2,
            GLTypeName._uvec2,
            GLTypeName._ivec3,
            GLTypeName._uvec3,
            GLTypeName._ivec4,
            GLTypeName._uvec4,
        };
        public static readonly GLTypeName[] FloatingPointTypes = new GLTypeName[]
        {
            GLTypeName._float,
            GLTypeName._double,
            GLTypeName._vec2,
            GLTypeName._dvec2,
            GLTypeName._vec3,
            GLTypeName._dvec3,
            GLTypeName._vec4,
            GLTypeName._dvec4,
        };
        public static readonly GLTypeName[] NumericTypes = new GLTypeName[]
        {
            GLTypeName._float,
            GLTypeName._double,
            GLTypeName._int,
            GLTypeName._uint,
            GLTypeName._vec2,
            GLTypeName._ivec2,
            GLTypeName._uvec2,
            GLTypeName._dvec2,
            GLTypeName._vec3,
            GLTypeName._ivec3,
            GLTypeName._uvec3,
            GLTypeName._dvec3,
            GLTypeName._vec4,
            GLTypeName._ivec4,
            GLTypeName._uvec4,
            GLTypeName._dvec4,
        };
        public static readonly GLTypeName[] SignedTypes = new GLTypeName[]
        {
            GLTypeName._float,
            GLTypeName._double,
            GLTypeName._int,
            GLTypeName._vec2,
            GLTypeName._ivec2,
            GLTypeName._dvec2,
            GLTypeName._vec3,
            GLTypeName._ivec3,
            GLTypeName._dvec3,
            GLTypeName._vec4,
            GLTypeName._ivec4,
            GLTypeName._dvec4,
        };
        public static readonly GLTypeName[] BooleanTypes = new GLTypeName[]
        {
            GLTypeName._bool,
            GLTypeName._bvec2,
            GLTypeName._bvec3,
            GLTypeName._bvec4,
        };
    }
}
