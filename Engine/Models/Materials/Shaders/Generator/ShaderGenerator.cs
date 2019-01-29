using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Functions;

namespace TheraEngine.Rendering
{
    public class MaterialGenerator
    {
        private const EGLSLVersion GLSLCurrentVersion = EGLSLVersion.Ver_450;
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
        public void WriteVersion(EGLSLVersion version)
        {
            Line("#version {0}", version.ToString().Substring(4));
        }
        public void WriteVersion()
        {
            WriteVersion(GLSLCurrentVersion);
        }
        public void WriteInVar(int layoutLocation, EShaderVarType type, string name)
        {
            Line("layout (location = {0}) in {1} {2};", layoutLocation, type.ToString().Substring(1), name);
        }
        public void WriteInVar(EShaderVarType type, string name)
        {
            Line("in {0} {1};", type.ToString().Substring(1), name);
        }
        public void WriteOutVar(int layoutLocation, EShaderVarType type, string name)
        {
            Line("layout (location = {0}) out {1} {2};", layoutLocation, type.ToString().Substring(1), name);
        }
        public void WriteOutVar(EShaderVarType type, string name)
        {
            Line("out {0} {1};", type.ToString().Substring(1), name);
        }
        public void WriteUniform(int layoutLocation, EShaderVarType type, string name)
        {
            Line("layout (location = {0}) uniform {1} {2};", layoutLocation, type.ToString().Substring(1), name);
        }
        public void WriteUniform(EShaderVarType type, string name)
        {
            Line("{2}{0} {1};", type.ToString().Substring(1), name, _inBlock ? "" : "uniform ");
        }
        private bool _inBlock = false;
        public void StartUniformBlock(string structName)
        {
            _inBlock = true;
            Line("uniform {0}", structName);
            OpenBracket();
        }
        public void EndUniformBlock(string variableName = null)
        {
            CloseBracket(variableName, true);
            _inBlock = false;
        }
        public void Comment(string comment, params object[] args)
        {
            Line("//" + comment, args);
        }
        public void OpenLoop(int startIndex, int count, string varName = "i")
        {
            Line($"for (int {varName} = {startIndex}; {varName} < {count}; ++{varName})");
            OpenBracket();
        }
        public void OpenLoop(int count, string varName = "i") => OpenLoop(0, count, varName);
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
        public void CloseBracket(string name = null, bool includeSemicolon = false)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (includeSemicolon)
                    Line("} " + name + ";");
                else
                    Line("} " + name);
            }
            else
            {
                if (includeSemicolon)
                    Line("};");
                else
                    Line("}");
            }
        }
        #endregion

        public static bool Generate(
            ResultFunc resultFunction,
            out GLSLScript[] shaderFiles,
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

            SortedDictionary<int, List<MaterialFunction>> deepness = new SortedDictionary<int, List<MaterialFunction>>
            {
                { 0, new List<MaterialFunction>() }
            };

            VarNameGen nameGen = new VarNameGen();
            HashSet<MeshParam> meshParamUsage = new HashSet<MeshParam>();
            HashSet<EEngineUniform> engineParamUsage = new HashSet<EEngineUniform>();
            List<string> globalDeclarations = new List<string>();

            EGLSLVersion maxGLSLVer = EGLSLVersion.Ver_110;
            Prepass(resultFunction, nameGen, 0, deepness, fragGen, meshParamUsage, engineParamUsage, globalDeclarations, ref maxGLSLVer);

            fragGen.WriteVersion();
            foreach (string p in globalDeclarations)
                fragGen.Line(p);
            foreach (MeshParam p in meshParamUsage)
                fragGen.Line(p.GetVariableInDeclaration());
            foreach (EEngineUniform p in engineParamUsage)
                fragGen.WriteUniform(EngineParameterFunc.GetUniformType(p), p.ToString());

            fragGen.StartMain();

            var funcLists = deepness.OrderByDescending(x => x.Key).Select(x => x.Value).ToArray();
            HashSet<MaterialFunction> written = new HashSet<MaterialFunction>();
            foreach (var list in funcLists)
                foreach (var func in list)
                    if (written.Add(func))
                    {
                        if (func is ShaderMethod method)
                        {
                            string syntax = method.GetCodeSyntax(out bool returnsInline);
                            if (returnsInline)
                            {
                                string type = method.OutputArguments[0].ArgumentType.ToString().Substring(1);
                                string name = method.GetOutputNames()[0];
                                fragGen.Line(string.Format("{0} {1} = {2};", type, name, syntax));
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

            string fragStr = fragGen.EndMain();

            Engine.PrintLine("Generated Fragment Shader, " + maxGLSLVer.ToString() + ":\n" + fragStr);

            shaderFiles = new GLSLScript[]
            {
                new GLSLScript(EGLSLType.Fragment, fragStr),
            };
            shaderVars = vars.ToArray();

            return true;
        }

        private static void Prepass(
            MaterialFunction func, 
            VarNameGen nameGen, 
            int deepness,
            SortedDictionary<int, List<MaterialFunction>> deepnessDic,
            MaterialGenerator fragGen,
            HashSet<MeshParam> meshParamUsage,
            HashSet<EEngineUniform> engineParamUsage,
            List<string> globalDeclarations,
            ref EGLSLVersion maxGLSLVer)
        {
            deepnessDic[deepness++].Add(func);
            if (func is ShaderMethod method)
            {
                if (method.HasGlobalVarDec)
                {
                    string s = method.GetGlobalVarDec();
                    if (!string.IsNullOrWhiteSpace(s))
                        globalDeclarations.Add(s);
                }

                foreach (MeshParam p in method.NecessaryMeshParams)
                    meshParamUsage.Add(p);

                foreach (EEngineUniform p in method.NecessaryEngineParams)
                    engineParamUsage.Add(p);

                //maxGLSLVer = (EGLSLVersion)Math.Max((int)maxGLSLVer, (int)method.Overloads[method.CurrentValidOverloads[0]].Version);
            }

            foreach (MatFuncValueOutput output in func.OutputArguments)
                if (output.Connections.Count > 0)
                    output.OutputVarName = nameGen.New();

            foreach (MatFuncValueInput input in func.InputArguments)
                if (input.Connection != null)
                {
                    if (!deepnessDic.ContainsKey(deepness))
                        deepnessDic.Add(deepness, new List<MaterialFunction>());

                    Prepass(
                        input.Connection.ParentSocket,
                        nameGen, deepness, deepnessDic, fragGen, 
                        meshParamUsage, engineParamUsage, globalDeclarations,
                        ref maxGLSLVer);
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
