using CustomEngine.Rendering.HUD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class MaterialFunction : Function<MatFuncValueInput, MatFuncValueOutput>
    {
        /// <summary>
        /// Determines if this function can be contained within one line.
        /// Otherwise written as a method located outside of main().
        /// Ex: type thing = FUNC_OPERATION;
        /// otherwise:
        /// type FUNC_NAME(in/out type args) { FUNC_OPERATION }
        /// </summary>
        protected bool _inline = false;
        
        public MaterialFunction(bool inline)
        {
            AddInput(GetInputs());
            AddOutput(GetOutputs());

            foreach (FuncValueInput input in _inputs)
            {
                input.Arrange(_inputs.Count);
                _children.Add(input);
            }
            foreach (FuncValueOutput output in _outputs)
            {
                output.Arrange(_outputs.Count);
                _children.Add(output);
            }
        }

        internal const float _titleHeight = 10.0f;
        internal const float _textCharWidth = 5.0f;
        internal const float _padding = 3.0f;
        internal const float _maxArgTextWidth = 20.0f;

        protected virtual List<FuncValueInput> GetInputs() => new List<FuncValueInput>();
        protected virtual List<FuncValueOutput> GetOutputs() => new List<FuncValueOutput>();

        /// <summary>
        /// Returns the base operation for string.Format.
        /// </summary>
        protected abstract string GetOperation();

        protected void AddInput(List<FuncValueInput> input)
        {
            if (input != null)
                foreach (FuncValueInput v in input)
                    AddInput(v);
        }
        protected void AddInput(FuncValueInput input)
        {
            input.Arrange(_inputs.Count);
            _inputs.Add(input);
        }
        protected void AddOutput(List<FuncValueOutput> output)
        {
            if (output != null)
                foreach (FuncValueOutput v in output)
                    AddOutput(v);
        }
        protected void AddOutput(FuncValueOutput output)
        {
            _outputs.Add(output);
        }
        public override string ToString() => FunctionName;
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
            foreach (FuncValueInput arg in InputArguments)
            {
                if (first)
                    first = false;
                else
                    s += ", ";
                s += "in " + arg.CurrentArgumentType.ToString().Substring(1) + " " + arg.Name;
            }
            foreach (FuncValueOutput arg in OutputArguments)
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

        public FunctionDefinition Definition => GetType().GetCustomAttribute<FunctionDefinition>();
        public ReadOnlyCollection<string> Keywords => Definition?._keywords.AsReadOnly();
        public string FunctionName => Definition?._name;
        public string Description => Definition?._description;
        public string Category => Definition?._category;

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
