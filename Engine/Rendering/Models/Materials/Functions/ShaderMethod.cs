using System;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public abstract class ShaderMethod : MaterialFunction
    {
        /// <summary>
        /// If true, writes within the given scope.
        /// If false, writes a method before main.
        /// </summary>
        [Browsable(false)]
        public bool Inline { get; protected set; } = true;
        /// <summary>
        /// If true, writes a variable declaration before main using GetGlobalVarDec().
        /// </summary>
        [Browsable(false)]
        public bool HasGlobalVarDec { get; protected set; } = false;
        /// <summary>
        /// If true, can be written within a single line (do not include a semicolon in GetOperation()).
        /// If false, uses its own space. Use as many lines as necessary and include semicolons.
        /// </summary>
        [Browsable(false)]
        public bool ReturnsInline => Inline || OutputArguments.Count == 1;
        
        public ShaderMethod() : base() { }
        public ShaderMethod(bool inline) : base() { Inline = inline; }
        public ShaderMethod(params ShaderVarType[] outputTypes) : base(true)
        {
            AddValueOutput(new MatFuncValueOutput(string.Empty, outputTypes));
            Inline = true;
            AddArguments();
        }

        /// <summary>
        /// Returns the base operation for string.Format.
        /// </summary>
        protected abstract string GetOperation();
        protected virtual string GetGlobalVarDec() => null;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputNames"></param>
        /// <param name="outputNames"></param>
        /// <returns></returns>
        public string GetLineSyntax(
            string[] inputNames,
            string[] outputNames)
        {
            if (inputNames.Length != _valueInputs.Count ||
                outputNames.Length != _valueOutputs.Count)
                throw new ArgumentException();

            if (Inline)
                return string.Format(GetOperation(), inputNames);
            
            //Write method call syntax
            string s = FunctionName + "(";
            if (_valueInputs.Count > 0)
            {
                s += inputNames[0];
                for (int i = 1; i < _valueInputs.Count; ++i)
                    s += ", " + inputNames[i];
            }
            if (_valueOutputs.Count > 0)
            {
                s += outputNames[0];
                for (int i = 1; i < _valueOutputs.Count; ++i)
                    s += ", " + outputNames[i];
            }
            s += ")";

            return s;
        }
        public string GetMethodSyntax()
        {
            string type;

            if (OutputArguments.Count == 1)
                type = OutputArguments[0].ArgumentType.ToString().Substring(1);
            else
                type = "void";

            string s = type + " " + FunctionName + "(";
            bool first = true;

            foreach (MatFuncValueInput arg in InputArguments)
            {
                if (first)
                    first = false;
                else
                    s += ", ";
                s += "in " + arg.ArgumentType.ToString().Substring(1) + " " + arg.Name;
            }

            if (OutputArguments.Count > 1)
                foreach (MatFuncValueOutput arg in OutputArguments)
                {
                    if (first)
                        first = false;
                    else
                        s += ", ";
                    s += "out " + arg.ArgumentType.ToString().Substring(1) + " " + arg.Name;
                }

            s += ")" + 
                Environment.NewLine + "{" + Environment.NewLine + 
                GetOperation() +
                Environment.NewLine + "}" + Environment.NewLine;

            return s;
        }
    }
}
