using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Functions;

namespace TheraEngine.Rendering
{
    public class MaterialGenerator
    {
        private const string GLSLVersion = "450";
        private string NewLine = Environment.NewLine;
        
        private string _shaderCode = "";
        private int _tabCount = 0;

        #region String Helpers
        private string Tabs
        {
            get
            {
                string t = "";
                for (int i = 0; i < _tabCount; i++)
                    t += "\t";
                return t;
            }
        }
        public void Reset()
        {
            _shaderCode = "";
            _tabCount = 0;
        }
        public void WriteVersion()
        {
            Line("#version {0}", GLSLVersion);
        }
        public void WriteInVar(int layoutLocation, ShaderVarType type, string name)
        {
            Line("layout (location = {0}) in {1} {2};", layoutLocation, type.ToString().Substring(1), name);
        }
        public void WriteInVar(ShaderVarType type, string name)
        {
            Line("in {0} {1};", type.ToString().Substring(1), name);
        }
        public void WriteUniform(int layoutLocation, ShaderVarType type, string name)
        {
            Line("layout (location = {0}) uniform {1} {2};", layoutLocation, type.ToString().Substring(1), name);
        }
        public void WriteUniform(ShaderVarType type, string name)
        {
            Line("uniform {0} {1};", type.ToString().Substring(1), name);
        }
        public void Comment(string comment, params object[] args)
        {
            Line("//" + comment, args);
        }
        public void Loop(int startIndex, int count, string varName = "i")
        {
            Line($"for (int {varName} = {startIndex}; {varName} < {count}; ++{varName})");
        }
        public void Loop(int count, string varName = "i") => Loop(0, count, varName);
        public void StartMain()
        {
            Line("void main()");
            OpenBracket();
        }
        public string EndMain()
        {
            CloseBracket();
            string s = _shaderCode;
            Reset();
            return s;
        }
        /// <summary>
        /// Writes the current line and increments to the next line.
        /// Do not use arguments if you need to include brackets in the string.
        /// </summary>
        public void Line(string str = "", params object[] args)
        {
            str += NewLine;

            //Decrease tabs for every close bracket
            if (args.Length == 0)
                _tabCount -= str.Count(x => x == '}');

            bool s = false;
            int r = str.LastIndexOf(NewLine);
            if (r == str.Length - NewLine.Length)
            {
                str = str.Substring(0, str.Length - NewLine.Length);
                s = true;
            }
            str = str.Replace(NewLine, NewLine + Tabs);
            if (s)
                str += NewLine;

            _shaderCode += Tabs + (args != null && args.Length > 0 ? string.Format(str, args) : str);

            //Increase tabs for every open bracket
            if (args.Length == 0)
                _tabCount += str.Count(x => x == '{');
        }
        public void OpenBracket()
        {
            Line("{");
        }
        public void CloseBracket(bool includeSemicolon = false)
        {
            if (includeSemicolon)
                Line("};");
            else
                Line("}");
        }
        #endregion

        public static bool Generate(
            ResultFunc resultFunction,
            out ShaderFile[] shaderFiles,
            out ShaderVar[] shaderVars)
        {
            if (resultFunction == null)
            {
                shaderFiles = null;
                shaderVars = null;
                return false;
            }

            List<ShaderVar> vars = new List<ShaderVar>();
            MaterialGenerator fragGen = new MaterialGenerator();
            fragGen.WriteVersion();

            SortedDictionary<int, List<MaterialFunction>> deepness = new SortedDictionary<int, List<MaterialFunction>>
            {
                { 0, new List<MaterialFunction>() }
            };

            VarNameGen nameGen = new VarNameGen();
            Prepass(resultFunction, nameGen, 0, deepness, fragGen);

            fragGen.StartMain();

            var funcLists = deepness.OrderByDescending(x => x.Key).Select(x => x.Value).ToArray();
            HashSet<MaterialFunction> written = new HashSet<MaterialFunction>();
            foreach (var list in funcLists)
            {
                foreach (var func in list)
                {
                    if (written.Add(func))
                    {
                        if (func is ShaderMethod method)
                        {
                            string syntax = method.GetCodeSyntax(out bool returnsInline);
                            if (returnsInline)
                            {
                                fragGen.Line(syntax);
                            }
                            else
                            {
                                fragGen.Line(syntax);
                            }
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
            }
            
            shaderFiles = new ShaderFile[]
            {
                new ShaderFile(ShaderMode.Fragment, fragGen.EndMain()),
            };
            shaderVars = vars.ToArray();

            return true;
        }

        private static void Prepass(
            MaterialFunction func, 
            VarNameGen nameGen, 
            int deepness,
            SortedDictionary<int, List<MaterialFunction>> deepnessDic,
            MaterialGenerator fragGen)
        {
            deepnessDic[deepness++].Add(func);
            if (func is ShaderMethod method && method.HasGlobalVarDec)
                fragGen.Line(method.GetGlobalVarDec());
            foreach (MatFuncValueOutput output in func.OutputArguments)
                if (output.Connections.Count > 0)
                    output.OutputVarName = nameGen.New();
            foreach (MatFuncValueInput input in func.InputArguments)
                if (input.Connection != null)
                {
                    if (!deepnessDic.ContainsKey(deepness))
                        deepnessDic.Add(deepness, new List<MaterialFunction>());
                    Prepass(input.Connection.ParentSocket, nameGen, deepness, deepnessDic, fragGen);
                }
        }
        public sealed class MatNode
        {
            public MaterialFunction Func { get; set; }
            public string[] OutputNames { get; set; }
            public MatNode[] Children { get; set; }
            public int Deepness { get; set; } = 0;
        }
    }
}
