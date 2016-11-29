using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class BaseGLOutput
    {
        public string Name { get { return _name; } }
        protected string _name;
        public BaseGLOutput(string name) { _name = name; }
        public abstract GLTypeName GetTypeName();
        public abstract Type GetOutType();
        public abstract Type[] GetPossibleOutTypes();
        public bool CanConnectTo(BaseGLArgument other)
        {
            return other != null && GetOutType() == other.GetArgType();
        }
    }
}
