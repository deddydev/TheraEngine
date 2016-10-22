using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLVec4 : GLVar
    {
        public GLVec4(string name) : base(GLTypeName._vec4, name)
        {
            _fields.Add(new GLFloat("x"));
            _fields.Add(new GLFloat("y"));
            _fields.Add(new GLFloat("z"));
            _fields.Add(new GLFloat("w"));
        }
    }
}
