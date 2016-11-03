using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public enum EQualifier
    {
        _uniform,
        _out,
        _in
    }
    public class GLGlobalVar : GLVar
    {
        int _layoutId;

        public GLGlobalVar(GLTypeName type, string name, int layoutId) : base(type, name)
        {
            _layoutId = layoutId;
        }
    }
}
