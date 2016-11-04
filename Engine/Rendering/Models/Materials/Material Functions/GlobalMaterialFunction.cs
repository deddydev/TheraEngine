using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class GlobalMaterialFunction : MaterialFunction
    {
        private string _name;

        public override string ToString()
        {
            string s = "void " + _name + "(";
            bool first = true;
            foreach (GLVar v in InputArguments)
            {
                if (first)
                    first = false;
                else
                    s += ", ";
                s += "in " + v.TypeName.ToString().Substring(1) + " " + v.Name;
            }
            foreach (GLVar v in OutputArguments)
            {
                if (first)
                    first = false;
                else
                    s += ", ";
                s += "out " + v.GetDeclaration();
            }
            s += ")";
            return s;
        }
    }
}
