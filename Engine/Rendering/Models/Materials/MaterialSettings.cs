using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class MaterialSettings
    {
        private List<GLVar> _parameters = new List<GLVar>();
        //private VertexAttribInfo[] _attrib;

        public List<GLVar> Parameters { get { return _parameters; } }
        
        public MaterialSettings()
        {

        }
    }
}
