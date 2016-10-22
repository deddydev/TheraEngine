using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLVec2 : GLVar
    {
        public GLVec2(string name) : base(GLTypeName._vec2, name)
        {
            _fields.Add(new GLFloat("x"));
            _fields.Add(new GLFloat("y"));
        }
    }
}
