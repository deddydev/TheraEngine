using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class ShaderGenerator
    {
        private const string GLSLVersion = "410";
        private const string NewLine = "\n";

        private string _shaderCode;
        protected PrimitiveBufferInfo _info;

        public ShaderGenerator(PrimitiveBufferInfo info) { _info = info; }

        public abstract string Generate(ResultBasicFunc end);

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
        private int tabCount = 0;
        public void Reset()
        {
            _shaderCode = "";
            tabCount = 0;
        }
        protected void WriteVersion()
        {
            wl("#version {0}", GLSLVersion);
            wl();
        }
        protected void WriteInVar(int layoutLocation, string type, string name)
        {
            wl("layout (location = {0}) in {1} {2};", layoutLocation, type, name);
        }
        protected void WriteInVar(string type, string name)
        {
            wl("in {0} {1};", type, name);
        }
        protected void WriteUniform(int layoutLocation, string type, string name)
        {
            wl("layout (location = {0}) uniform {1} {2};", layoutLocation, type, name);
        }
        protected void WriteUniform(string type, string name)
        {
            wl("uniform {0} {1};", type, name);
        }
        protected void Comment(string comment, params object[] args)
        {
            wl("//" + comment, args);
        }
        protected void Begin()
        {
            wl("void main()");
            OpenBracket();
        }
        protected string Finish()
        {
            CloseBracket();
            return _shaderCode;
        }
        /// <summary>
        /// Writes the current line and increments to the next line.
        /// Do not use arguments if you need to include brackets in the string.
        /// </summary>
        protected void wl(string str = "", params object[] args)
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
        protected void OpenBracket()
        {
            wl("{");
        }
        protected void CloseBracket()
        {
            wl("}");
        }
        #endregion

        public static Material GenerateMaterial(string name, ResultBasicFunc resultFunction)
        {
            if (resultFunction == null)
                return null;

            Material m = new Material(name, new MaterialSettings());

            //TODO: determine shader types needed
            foreach (BaseGLArgument arg in resultFunction.InputArguments)
            {

            }
            
            return m;
        }
    }
}
