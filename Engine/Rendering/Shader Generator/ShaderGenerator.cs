using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Functions;

namespace TheraEngine.Rendering
{
    public class ShaderGenerator
    {
        public static readonly string OutputColorName = "OutColor";

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

        public static ShaderFile[] GenerateShaders(ResultFunc resultFunction)
        {
            if (resultFunction == null)
                return null;

            ShaderGenerator fragGen = new ShaderGenerator();
            fragGen.WriteVersion();
            fragGen.StartMain();

            SortedDictionary<int, MaterialFunction> deepness = new SortedDictionary<int, MaterialFunction>();

            VarNameGen nameGen = new VarNameGen();
            FuncGen(resultFunction, nameGen);
            
            ShaderFile frag = new ShaderFile(ShaderMode.Fragment, fragGen.EndMain());

            return new ShaderFile[]
            {
                frag,
            };
        }

        private static void FuncGen(MaterialFunction func, VarNameGen nameGen)
        {
            if (func is ShaderMethod m)
            {
                //string op = m.GetLineSyntax();
            }
            else if (func is ShaderLogic l)
            {
                string format = l.GetLogicFormat();
            }
            foreach (MatFuncValueInput arg in func.InputArguments)
            {
                if (arg.Connection != null)
                {
                    MaterialFunction f = arg.Connection.ParentSocket;
                    FuncGen(f, nameGen);
                }
                else
                {
                    Convert.ToSingle(arg.DefaultValue);
                }
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
