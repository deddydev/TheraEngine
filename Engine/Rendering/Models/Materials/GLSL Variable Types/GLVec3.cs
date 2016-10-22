using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLVec3 : GLVar
    {
        public GLVec3(string name) : base(GLTypeName._vec3, name)
        {
            _fields.Add(new GLFloat("x"));
            _fields.Add(new GLFloat("y"));
            _fields.Add(new GLFloat("z"));
        }
    }
}
