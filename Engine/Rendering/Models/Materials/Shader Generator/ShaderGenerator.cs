using System;
using System.Collections.Generic;
using System.Linq;

namespace TheraEngine.Rendering.Models.Materials
{
    public class ShaderGenerator
    {
        private const string GLSLVersion = "450";
        private string NewLine = Environment.NewLine;
        
        private string _shaderCode = "";
        private int tabCount = 0;

        #region String Helpers
        private string Tabs
        {
            get
            {
                string t = "";
                for (int i = 0; i < tabCount; i++)
                    t += "\t";
                return t;
            }
        }
        public void Reset()
        {
            _shaderCode = "";
            tabCount = 0;
        }
        public void WriteVersion()
        {
            wl("#version {0}", GLSLVersion);
            wl();
        }
        public void WriteInVar(int layoutLocation, GLTypeName type, string name)
        {
            wl("layout(location = {0}) in {1} {2};", layoutLocation, type.ToString().Substring(1), name);
        }
        public void WriteInVar(GLTypeName type, string name)
        {
            wl("in {0} {1};", type.ToString().Substring(1), name);
        }
        public void WriteUniform(int layoutLocation, GLTypeName type, string name)
        {
            wl("layout(location = {0}) uniform {1} {2};", layoutLocation, type.ToString().Substring(1), name);
        }
        public void WriteUniform(GLTypeName type, string name)
        {
            wl("uniform {0} {1};", type.ToString().Substring(1), name);
        }
        public void Comment(string comment, params object[] args)
        {
            wl("//" + comment, args);
        }
        public void Begin()
        {
            wl("void main()");
            OpenBracket();
        }
        public string Finish()
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
        public void wl(string str = "", params object[] args)
        {
            str += NewLine;

            //Decrease tabs for every close bracket
            if (args.Length == 0)
                tabCount -= str.Count(x => x == '}');

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
                tabCount += str.Count(x => x == '{');
        }
        public void OpenBracket()
        {
            wl("{");
        }
        public void CloseBracket()
        {
            wl("}");
        }
        #endregion

        public static Material GenerateMaterial(string name, ResultBasicFunc resultFunction)
        {
            if (resultFunction == null)
                return null;

            Material m = new Material(name, new List<GLVar>(), new List<TextureReference>());

            //TODO: determine shader types needed
            foreach (MatFuncValueInput arg in resultFunction.InputArguments)
            {

            }
            
            return m;
        }
    }
}
