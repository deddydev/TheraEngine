using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLFloat : GLVar
    {
        public GLFloat(string name) : base(GLTypeName._float, name)
        {

        }
    }
}
